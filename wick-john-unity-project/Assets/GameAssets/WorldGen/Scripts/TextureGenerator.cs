using UnityEngine;

namespace GameAssets.WorldGen.Scripts
{
    public static class TextureGenerator
    {
        public static Texture2D TextureFromNoiseMap(float[,] noiseMap)
        {
            Color[] colors = new Color[noiseMap.GetLength(0) * noiseMap.GetLength(1)];
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                for (int x = 0; x < noiseMap.GetLength(0); x++)
                {
                    colors[y * noiseMap.GetLength(0) + x] = Color.black * noiseMap[x, y];
                }
            }

            Texture2D texture2D = new Texture2D(noiseMap.GetLength(0), noiseMap.GetLength(1));
            texture2D.SetPixels(colors);
            texture2D.Apply();

            return texture2D;
        }
    }
}