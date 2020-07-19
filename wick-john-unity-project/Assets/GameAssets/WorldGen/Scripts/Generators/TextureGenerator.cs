using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class TextureGenerator : SpriteGenerator
    {
        public int width = 100;
        public int height = 100;
        public NoiseSettings noiseSettings;
        
        public override Color[,] GenerateColors()
        {
            return ColorsFromNoiseMap(Noise.GenerateNoiseMap(width, height, noiseSettings, Vector2.zero));
        }

        public static Color[,] ColorsFromNoiseMap(float[,] noiseMap)
        {
            Color[,] colors = new Color[noiseMap.GetLength(0), noiseMap.GetLength(1)];
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                for (int x = 0; x < noiseMap.GetLength(0); x++)
                {
                    colors[x, y] = Color.black * noiseMap[x, y];
                }
            }

            return colors;
        }

        public static Texture2D TextureFromNoiseMap(float[,] noiseMap)
        {
            Texture2D texture2D = new Texture2D(noiseMap.GetLength(0), noiseMap.GetLength(1));
            texture2D.SetPixels(FlattenColorArray(ColorsFromNoiseMap(noiseMap)));
            texture2D.Apply();

            return texture2D;
        }
    }
}