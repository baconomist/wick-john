using System;
using GameAssets.Scripts;
using GameAssets.World.Scripts;
using GameAssets.WorldGen.Scripts.GeneratorData;
using GameAssets.WorldGen.Scripts.Generators.BaseGenerators;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class PlayableBuildingGenerator : Generator, IChunkableGenerator
    {
        public PlayableBuildingData playableBuildingData;
        public float buildingSeparation = 2.0f;

        private bool _regeneratePreviewQueued = false;
        private PlayableBuilding _lastPb;

        private float _enemyWidth;
        private float _enemyHeight;

        private void OnValidate()
        {
            if (playableBuildingData != null)
            {
                // playableBuildingData.OnDataUpdate -= QueueGeneratePreview;
                // playableBuildingData.OnDataUpdate += QueueGeneratePreview;
            }
        }

        private void QueueGeneratePreview()
        {
            _regeneratePreviewQueued = true;
        }

        private void Update()
        {
            if (!Application.isPlaying && _regeneratePreviewQueued && autoUpdate)
            {
                GeneratePreview();
                _regeneratePreviewQueued = false;
            }
        }

        public object GenerateChunkData()
        {
            return null;
        }

        public void ApplyChunkData(ChunkGenerator.MapChunk mapChunk)
        {
            // pass
        }

        public float PreGenerateChunk(GameObject g)
        {
            GenerateOn(g);
            return _lastPb.background.GetComponent<SpriteRenderer>().bounds.size.x + buildingSeparation;
        }

        public override void GeneratePreview()
        {
            // Looping many times works for some reason
            for (int i = 0; i < 10; i++)
            {
                foreach (Transform child in transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            GenerateOn(gameObject);
        }

        public override void GenerateOn(GameObject parentGameObject)
        {
            GameObject tempEnemy = Instantiate(playableBuildingData.enemyPrefab);
            _enemyWidth = tempEnemy.GetComponentInChildren<BoxCollider2D>().bounds.size.x;
            _enemyHeight = tempEnemy.GetComponentInChildren<BoxCollider2D>().bounds.size.y;
            DestroyImmediate(tempEnemy);

            PlayableBuilding pb = parentGameObject.GetComponent<PlayableBuilding>();
            if (pb == null)
                pb = parentGameObject.AddComponent<PlayableBuilding>();

            pb.minOpacity = playableBuildingData.minOpacity;
            pb.playerFadeDistanceMultiplier = playableBuildingData.playerFadeDistanceMultiplier;
            pb.wallThickness = ThreadSafeRandom.Range(playableBuildingData.minWallThickness,
                playableBuildingData.maxWallThickness);
            pb.size = ThreadSafeRandom.Range(playableBuildingData.minSize, playableBuildingData.maxSize);
            pb.height = ThreadSafeRandom.Range(playableBuildingData.minHeight, playableBuildingData.maxHeight);
            pb.floorsCount = ThreadSafeRandom.Range(playableBuildingData.minFloors, playableBuildingData.maxFloors + 1);

            InstantiateBuilding(parentGameObject, pb);
            ConstructBuilding(parentGameObject, pb);
            _lastPb = pb;

            GenerateEnemies(pb);
        }

        private void InstantiateBuilding(GameObject parentGameObject, PlayableBuilding pb)
        {
            pb.leftWall = new Wall("Left", parentGameObject, playableBuildingData.renderIndex);
            pb.rightWall = new Wall("Right", parentGameObject, playableBuildingData.renderIndex);
            pb.topWall = new Wall("Top", parentGameObject, playableBuildingData.renderIndex);

            pb.floors = new Wall[pb.floorsCount];
            for (int i = 0; i < pb.floorsCount; i++)
            {
                pb.floors[i] = new Wall("Floor " + (i + 1), parentGameObject, playableBuildingData.renderIndex);
            }

            pb.windows = new Wall[2];
            for (int i = 0; i < 2; i++)
            {
                pb.windows[i] = new Wall("Window " + (i + 1), parentGameObject, playableBuildingData.renderIndex);
                Window windowComp = pb.windows[i].GameObject.AddComponent<Window>();
                windowComp.windowParticles = playableBuildingData.windowParticlesPrefab;
            }

            pb.background = new GameObject("Background");
            pb.background.transform.parent = parentGameObject.transform;

            pb.platform = new Wall("Platform", parentGameObject, playableBuildingData.renderIndex);
        }

        private void ConstructBuilding(GameObject parentGameObject, PlayableBuilding pb)
        {
            pb.topWall.GameObject.transform.localScale =
                new Vector3(pb.size / pb.topWall.SpriteRenderer.bounds.size.x, pb.wallThickness, 1);
            pb.topWall.GameObject.transform.localPosition = new Vector3(0, pb.height, 1);

            pb.leftWall.GameObject.transform.localScale = new Vector3(pb.wallThickness,
                pb.height / pb.leftWall.SpriteRenderer.bounds.size.y + pb.wallThickness, 1);
            pb.leftWall.GameObject.transform.localPosition =
                new Vector3(-pb.topWall.SpriteRenderer.bounds.extents.x, pb.height / 2f, 1);

            pb.rightWall.GameObject.transform.localScale = pb.leftWall.GameObject.transform.localScale;
            pb.rightWall.GameObject.transform.localPosition =
                new Vector3(pb.topWall.SpriteRenderer.bounds.extents.x, pb.height / 2f, 1);

            pb.platform.GameObject.transform.localPosition = new Vector3(0, 0);
            pb.platform.GameObject.transform.localScale = pb.topWall.GameObject.transform.localScale;
            pb.platform.GameObject.AddComponent<EdgeCollider2D>().points = new[]
            {
                new Vector2(
                    pb.leftWall.GameObject.transform.localPosition.x / pb.platform.GameObject.transform.localScale.x,
                    0),
                new Vector2(
                    pb.rightWall.GameObject.transform.localPosition.x / pb.platform.GameObject.transform.localScale.x,
                    0)
            };

            ConstructFloors(pb);
            ConstructBackground(pb);
            ConstructWindows(pb);
        }

        private void ConstructFloors(PlayableBuilding pb)
        {
            for (int i = 0; i < pb.floorsCount; i++)
            {
                Wall floor = pb.floors[i];

                // Middle placement default
                Vector3 localScale = pb.topWall.GameObject.transform.localScale;
                float xPos = 0;
                float yPos = (float) (i + 1) / (pb.floorsCount + 1) * pb.height;

                // Make sure enemies can fit, otherwise don't create the floor since it can cause overlapping floors if we adjust
                if (pb.topWall.GameObject.transform.localPosition.y - yPos < _enemyHeight)
                {
                    continue;
                }

                // < -50 = left
                // -50 < x < 50 = middle
                // > 50 = right
                int floorPlacement = ThreadSafeRandom.Range(-100, 100);
                if (floorPlacement < -50 || floorPlacement > 50)
                {
                    Vector3 topWallScale = pb.topWall.GameObject.transform.localScale;
                    localScale = new Vector3(ThreadSafeRandom.Range(0.25f, 0.7f) * topWallScale.x, topWallScale.y,
                        topWallScale.z);
                }

                floor.GameObject.transform.localScale = localScale;

                if (floorPlacement < -50)
                {
                    xPos = pb.leftWall.GameObject.transform.localPosition.x + floor.SpriteRenderer.bounds.extents.x;
                }
                else if (floorPlacement > 50)
                {
                    xPos = pb.rightWall.GameObject.transform.localPosition.x - floor.SpriteRenderer.bounds.extents.x;
                }

                floor.GameObject.transform.localPosition = new Vector3(xPos, yPos);
            }
        }

        private void ConstructBackground(PlayableBuilding pb)
        {
            SpriteRenderer sp = pb.background.AddComponent<SpriteRenderer>();
            sp.sortingOrder = playableBuildingData.renderIndex;

            Texture2D bgTexture = new Texture2D(1, 1);
            sp.sprite = Sprite.Create(bgTexture, new Rect(0, 0, bgTexture.width, bgTexture.height),
                new Vector2(0.5f, 0.5f));

            Color[,] bgColors = new Color[256, 256];
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    bgColors[x, y] = Color.black;
                }
            }

            sp.sprite.texture.SetPixels(SpriteGenerator.FlattenColorArray(bgColors));
            sp.sprite.texture.Apply();

            // sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, playableBuildingData.backgroundOpacity);

            pb.background.transform.localPosition =
                new Vector3(pb.topWall.GameObject.transform.localPosition.x, pb.height / 2f, 1);
            pb.background.transform.localScale =
                new Vector3(pb.size / sp.sprite.bounds.size.x, pb.height / sp.sprite.bounds.size.y);
        }

        private void ConstructWindows(PlayableBuilding pb)
        {
            Texture2D windowTexture = new Texture2D(1, 1);
            windowTexture.SetPixels(new[] {playableBuildingData.windowColor});
            windowTexture.Apply();

            for (int i = 0; i < 2; i++)
            {
                Wall window = pb.windows[i];

                // Make windows render on top of building... kinda sketch solution but ok...
                window.SpriteRenderer.sortingOrder = playableBuildingData.renderIndex + 1;

                window.SpriteRenderer.sprite =
                    Sprite.Create(windowTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                window.GameObject.transform.localScale = pb.leftWall.GameObject.transform.localScale;
                window.GameObject.transform.localScale = new Vector3(window.GameObject.transform.localScale.x,
                    window.GameObject.transform.localScale.y * ThreadSafeRandom.Range(0.5f, 0.75f));


                Window windowComp = window.GameObject.GetComponent<Window>();
                if (i == 0)
                {
                    window.GameObject.transform.localPosition = pb.leftWall.GameObject.transform.localPosition;
                    windowComp.isLeftWindow = true;
                }
                else
                {
                    window.GameObject.transform.localPosition = pb.rightWall.GameObject.transform.localPosition;
                    windowComp.isLeftWindow = false;
                }

                windowComp.worldWidth = window.SpriteRenderer.bounds.size.x;
            }
        }

        private void GenerateEnemies(PlayableBuilding pb)
        {
            for (int i = 0; i < pb.floorsCount; i++)
            {
                Wall floor = pb.floors[i];

                float enemySpacing = _enemyWidth * 1.1f;
                
                int numEnemies = ThreadSafeRandom.Range(1, (int) ((floor.SpriteRenderer.bounds.size.x) / enemySpacing));

                float lastEnemyX = pb.leftWall.SpriteRenderer.bounds.size.x + floor.GameObject.transform.localPosition.x -
                                   floor.SpriteRenderer.bounds.extents.x;
                
                for (int j = 0; j < numEnemies; j++)
                {
                    GameObject enemyG = Instantiate(playableBuildingData.enemyPrefab);
                    enemyG.transform.parent = pb.gameObject.transform;
                    
                    enemyG.transform.localPosition = new Vector3(lastEnemyX + enemySpacing,
                        floor.GameObject.transform.localPosition.y);

                    lastEnemyX = enemyG.transform.localPosition.x;
                }
            }
        }

        public class Wall
        {
            public string Name;
            public GameObject GameObject;
            public SpriteRenderer SpriteRenderer;

            public Wall(string name, GameObject parentGameObject, int renderIndex)
            {
                Name = name;

                GameObject = new GameObject(name);
                GameObject.transform.parent = parentGameObject.transform;
                GameObject.transform.localPosition = Vector3.zero;

                SpriteRenderer = GameObject.AddComponent<SpriteRenderer>();
                SpriteRenderer.sprite =
                    Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                SpriteRenderer.sprite.texture.SetPixels(new Color[] {Color.black});
                SpriteRenderer.sprite.texture.Apply();
                SpriteRenderer.sortingOrder = renderIndex;
            }
        }
    }
}