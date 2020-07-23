using System;
using System.Collections.Generic;
using GameAssets.Enemy.Scripts;
using GameAssets.Scripts;
using GameAssets.WorldGen.Scripts.GeneratorData;
using GameAssets.WorldGen.Scripts.Generators;
using UnityEngine;

namespace GameAssets.World.Scripts
{
    public class PlayableBuilding : MonoBehaviour
    {
        public float minOpacity;
        
        public float playerFadeDistanceMultiplier;
        public float wallThickness;
        public float size;
        public float height;
        public int floorsCount;

        public PlayableBuildingGenerator.BuildingObject leftWall;
        public PlayableBuildingGenerator.BuildingObject rightWall;
        public PlayableBuildingGenerator.BuildingObject topWall;

        public PlayableBuildingGenerator.BuildingObject[] floors;
        public PlayableBuildingGenerator.BuildingObject[] windows;
        public List<GameObject> enemies;
        
        public GameObject background;
        public PlayableBuildingGenerator.BuildingObject platform;
        public List<PlayableBuildingGenerator.BuildingObject> enemyCovers;

        private SpriteRenderer _bgSpriteRenderer;

        private float _enemyCenterWorldX;
        private bool _engagedEnemies = false;

        private void Start()
        {
            _bgSpriteRenderer = background.GetComponent<SpriteRenderer>();

            foreach (GameObject g in enemies)
            {
                _enemyCenterWorldX += g.transform.position.x / enemies.Count;
            }
        }

        public void Update()
        {
            float sqrDistance = (transform.position - GameManager.PlayerController.transform.position).sqrMagnitude;
            float alpha =
                Mathf.Clamp(minOpacity, Mathf.InverseLerp(
                    0,
                    playerFadeDistanceMultiplier *
                    (_bgSpriteRenderer.bounds.extents.x * _bgSpriteRenderer.bounds.extents.x), sqrDistance), 1);

            _bgSpriteRenderer.color = new Color(_bgSpriteRenderer.color.r, _bgSpriteRenderer.color.g,
                _bgSpriteRenderer.color.b, alpha);

            if (!_engagedEnemies && _enemyCenterWorldX - GameManager.PlayerController.transform.position.x < 0.1f)
            {
                foreach (GameObject g in enemies)
                {
                    g.GetComponent<EnemyController>().BeginEngage();
                    SlowMotionManager.StartSlowMotion();
                }

                _engagedEnemies = true;
            }

            if (_engagedEnemies)
            {
                foreach (PlayableBuildingGenerator.BuildingObject cover in enemyCovers)
                {
                    // Make cover "go into the ground"
                    float movementRate = Time.deltaTime;
                    cover.GameObject.transform.localPosition -= new Vector3(0, movementRate);
                    cover.GameObject.transform.localScale -= new Vector3(0, cover.GameObject.transform.localScale.y / cover.SpriteRenderer.bounds.extents.y * movementRate);
                }
            }
        }
    }
}