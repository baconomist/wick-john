using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class PlayableBuildingGenerator : Generator
    {
        public const float BackgroundOpacity = 0.3f;
        
        public int renderIndex = 5;

        public float minWallThickness = 1;
        public float maxWallThickness = 1;

        // Basically the size of the roof in length
        public float minSize = 10;
        public float maxSize = 10;
        
        public float minHeight = 2;
        public float maxHeight = 5;
        
        public int minFloors = 1;
        public int maxFloors = 2;

        private float _wallThickness;
        private float _size;
        private float _height;
        private int _floorsCount;

        private Wall _leftWall;
        private Wall _rightWall;
        private Wall _topWall;

        [HideInInspector]
        public Wall[] floors;

        private Wall[] _obstacles;

        private GameObject _background;

        public override void Generate()
        {
            _wallThickness = ThreadSafeRandom.Range(minWallThickness, maxWallThickness);
            _size = ThreadSafeRandom.Range(minSize, maxSize);
            _height = ThreadSafeRandom.Range(minHeight, maxHeight);
            _floorsCount = ThreadSafeRandom.Range(minFloors, maxFloors + 1);

            InstantiateBuilding();
            ConstructRandomBuilding();
        }

        private void InstantiateBuilding()
        {
            // Looping many times works for some reason
            for (int i = 0; i < 10; i++)
            {
                foreach (Transform child in transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            _leftWall = new Wall("Left", this);
            _rightWall = new Wall("Right", this);
            _topWall = new Wall("Top", this);

            floors = new Wall[_floorsCount];
            for (int i = 0; i < _floorsCount; i++)
            {
                floors[i] = new Wall("Floor " + (i + 1), this);
            }

            _background = new GameObject("Background");
            _background.transform.parent = transform;
        }

        private void ConstructRandomBuilding()
        {
            _topWall.GameObject.transform.localScale =
                new Vector3(_size / _topWall.SpriteRenderer.bounds.size.x, _wallThickness, 1);
            _topWall.GameObject.transform.localPosition = new Vector3(0, _height, 1);

            _leftWall.GameObject.transform.localScale = new Vector3(_wallThickness,
                _height / _leftWall.SpriteRenderer.bounds.size.y + _wallThickness, 1);
            _leftWall.GameObject.transform.localPosition =
                new Vector3(-_topWall.SpriteRenderer.bounds.extents.x, _height / 2f, 1);

            _rightWall.GameObject.transform.localScale = _leftWall.GameObject.transform.localScale;
            _rightWall.GameObject.transform.localPosition =
                new Vector3(_topWall.SpriteRenderer.bounds.extents.x, _height / 2f, 1);

            ConstructFloors();
            ConstructBackground();
        }

        private void ConstructFloors()
        {
            for (int i = 0; i < _floorsCount; i++)
            {
                Wall floor = floors[i];

                // Middle placement default
                Vector3 localScale = _topWall.GameObject.transform.localScale;
                float xPos = 0;
                float yPos = (float) (i + 1) / (_floorsCount + 1) * _height;

                // < -50 = left
                // -50 < x < 50 = middle
                // > 50 = right
                int floorPlacement = ThreadSafeRandom.Range(-100, 100);
                if (floorPlacement < -50 || floorPlacement > 50)
                {
                    Vector3 topWallScale = _topWall.GameObject.transform.localScale;
                    localScale = new Vector3(ThreadSafeRandom.Range(0.25f, 0.7f) * topWallScale.x, topWallScale.y,
                        topWallScale.z);
                }

                floor.GameObject.transform.localScale = localScale;

                if (floorPlacement < -50)
                {
                    xPos = _leftWall.GameObject.transform.localPosition.x + floor.SpriteRenderer.bounds.extents.x;
                }
                else if (floorPlacement > 50)
                {
                    xPos = _rightWall.GameObject.transform.localPosition.x - floor.SpriteRenderer.bounds.extents.x;
                }

                floor.GameObject.transform.localPosition = new Vector3(xPos, yPos);
            }
        }

        private void ConstructBackground()
        {
            SpriteRenderer sp = _background.AddComponent<SpriteRenderer>();
            sp.sortingOrder = renderIndex;
            
            Texture2D bgTexture = new Texture2D(256, 256);
            sp.sprite = Sprite.Create(bgTexture, new Rect(0, 0, bgTexture.width, bgTexture.height), new Vector2(0.5f, 0.5f));
            
            Color[,] bgColors = new Color[256, 256];
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    bgColors[x, y] = new Color(0, 0, 0, BackgroundOpacity);
                }
            }
            
            sp.sprite.texture.SetPixels(VisualGenerator.FlattenColorArray(bgColors));
            sp.sprite.texture.Apply();
            
            _background.transform.localPosition = new Vector3(_topWall.GameObject.transform.localPosition.x, _height / 2f, 1);
            _background.transform.localScale = new Vector3(_size / sp.sprite.bounds.size.x, _height / sp.sprite.bounds.size.y);
        }

        public class Wall
        {
            public string Name;
            public GameObject GameObject;
            public SpriteRenderer SpriteRenderer;

            public Wall(string name, PlayableBuildingGenerator pbg)
            {
                Name = name;

                GameObject = new GameObject(name + " Wall");
                GameObject.transform.parent = pbg.transform;
                GameObject.transform.localPosition = Vector3.zero;

                SpriteRenderer = GameObject.AddComponent<SpriteRenderer>();
                SpriteRenderer.sprite =
                    Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                SpriteRenderer.sprite.texture.SetPixels(new Color[] {Color.black});
                SpriteRenderer.sprite.texture.Apply();
                SpriteRenderer.sortingOrder = pbg.renderIndex;
            }
        }
    }
}