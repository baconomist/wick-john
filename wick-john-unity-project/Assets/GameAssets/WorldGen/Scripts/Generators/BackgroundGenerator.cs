using System;
using System.Collections.Generic;
using System.Threading;
using GameAssets.WorldGen.Scripts.GeneratorData;
using GameAssets.WorldGen.Scripts.Generators.BaseGenerators;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class BackgroundGenerator : SpriteGenerator, IChunkableGenerator
    {
        [Range(1, 10)] public int buildingDensity = 1;
        
        public BackgroundBuildingGenerator backgroundBuildingGenerator;
        public GradientData gradientData;
        public GradientGenerator gradientGenerator;
        public int renderIndex = 1;

        private const int BuildingsPerLayer = 6;

        public object GenerateChunkData()
        {
            return GenerateFlatColors();
        }

        public void ApplyChunkData(ChunkGenerator.MapChunk mapChunk)
        {
            SpriteRenderer sp = mapChunk.GameObject.GetComponent<SpriteRenderer>();
            sp.sprite.texture.SetPixels((Color[]) mapChunk.Data);
            sp.sprite.texture.Apply();
        }

        public float PreGenerateChunk(GameObject g)
        {
            Texture2D chunkTexture = new Texture2D(gradientData.textureWidth, gradientData.textureHeight);

            SpriteRenderer sp = g.AddComponent<SpriteRenderer>();
            sp.sortingOrder = renderIndex;
            
            sp.sprite = Sprite.Create(chunkTexture,
                new Rect(0, 0, gradientData.textureWidth, gradientData.textureHeight), new Vector2(0.5f, 0.5f));

            GameObject sky = new GameObject("Sky");
            sky.transform.parent = g.transform;
            SpriteRenderer skySp = sky.AddComponent<SpriteRenderer>();
            skySp.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            skySp.sprite.texture.SetPixels(new [] {Color.black});
            skySp.sprite.texture.Apply();
            skySp.transform.localScale = new Vector3(sp.bounds.size.x / skySp.bounds.size.x, 100);
            sky.transform.position = new Vector3(g.transform.position.x, g.transform.position.y + sp.bounds.extents.y + skySp.bounds.extents.y);
            
            return sp.bounds.size.x;
        }

        public override Color[,] GenerateColors()
        {
            gradientGenerator.gradientData.textureWidth = gradientData.textureWidth;
            gradientGenerator.gradientData.textureHeight = gradientData.textureHeight;

            gradientGenerator.gradientData = gradientData;
            Color[,] chunk = gradientGenerator.GenerateColors();

            float layerXOffset = 0;
            for (int i = 0; i < buildingDensity; i++)
            {
                for (int j = 0; j < BuildingsPerLayer; j++)
                {
                    Color[,] building = backgroundBuildingGenerator.GenerateColors();
                    chunk = TextureCombiner.Combine(chunk, building,
                        new Vector2(layerXOffset + j / (float) BuildingsPerLayer,
                            ((float) building.GetLength(1) / gradientGenerator.gradientData.textureHeight) / 2f));
                }

                layerXOffset += 0.1f;
            }

            return chunk;
        }
    }
}