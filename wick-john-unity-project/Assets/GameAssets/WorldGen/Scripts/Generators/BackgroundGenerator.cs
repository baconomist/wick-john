using System;
using System.Collections.Generic;
using System.Threading;
using GameAssets.WorldGen.Scripts.Generators.BaseGenerators;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class BackgroundGenerator : SpriteGenerator, IChunkedGenerator
    {
        [Range(1, 10)] public int buildingDensity = 1;

        public NoiseSettings noiseSettings;
        public BuildingGenerator buildingGenerator;
        public GradientGenerator gradientGenerator;

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

        public int GetChunkWidth()
        {
            return width;
        }

        public int GetChunkHeight()
        {
            return height;
        }

        public override Color[,] GenerateColors()
        {
            gradientGenerator.width = width;
            gradientGenerator.height = height;
            Color[,] chunk = gradientGenerator.GenerateColors();

            float layerXOffset = 0;
            for (int i = 0; i < buildingDensity; i++)
            {
                for (int j = 0; j < BuildingsPerLayer; j++)
                {
                    Color[,] building = buildingGenerator.GenerateColors();
                    chunk = TextureCombiner.Combine(chunk, building,
                        new Vector2(layerXOffset + j / (float) BuildingsPerLayer,
                            ((float) building.GetLength(1) / gradientGenerator.height) / 2f));
                }

                layerXOffset += 0.1f;
            }

            return chunk;
        }
    }
}