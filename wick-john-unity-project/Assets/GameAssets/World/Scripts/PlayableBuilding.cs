using System;
using GameAssets.Scripts;
using GameAssets.WorldGen.Scripts.GeneratorData;
using GameAssets.WorldGen.Scripts.Generators;
using UnityEngine;

namespace GameAssets.World.Scripts
{
    public class PlayableBuilding : MonoBehaviour
    {
        public float playerFadeDistanceMultiplier;
        public float wallThickness;
        public float size;
        public float height;
        public int floorsCount;

        public PlayableBuildingGenerator.Wall leftWall;
        public PlayableBuildingGenerator.Wall rightWall;
        public PlayableBuildingGenerator.Wall topWall;

        public PlayableBuildingGenerator.Wall[] floors;
        public PlayableBuildingGenerator.Wall[] windows;
        public PlayableBuildingGenerator.Wall[] obstacles;

        public GameObject background;
        public PlayableBuildingGenerator.Wall platform;

        private SpriteRenderer _bgSpriteRenderer;

        private void Start()
        {
            _bgSpriteRenderer = background.GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            float sqrDistance = (transform.position - GameManager.PlayerController.transform.position).sqrMagnitude;
            float alpha =
                Mathf.InverseLerp(
                    0,
                    playerFadeDistanceMultiplier *
                    (_bgSpriteRenderer.bounds.extents.x * _bgSpriteRenderer.bounds.extents.x), sqrDistance);

            _bgSpriteRenderer.color = new Color(_bgSpriteRenderer.color.r, _bgSpriteRenderer.color.g,
                _bgSpriteRenderer.color.b, alpha);
        }
    }
}