using System;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public abstract class VisualGenerator : Generator
    {
        public int width = 256;
        public int height = 256;
        
        public abstract Color[,] GenerateColors();

        public Color[] GenerateFlatColors()
        {
            return FlattenColorArray(GenerateColors());
        }

        public Texture2D GenerateTexture()
        {
            Color[,] colors = GenerateColors();
            Texture2D texture2D = new Texture2D(colors.GetLength(0), colors.GetLength(1));
            texture2D.SetPixels(FlattenColorArray(colors));
            texture2D.Apply();
            return texture2D;
        }

        public override void Generate()
        {
            throw new System.NotImplementedException();
        }
        
        public static Color[] FlattenColorArray(Color[,] colors)
        {
            Color[] flatColors = new Color[colors.GetLength(0) * colors.GetLength(1)];

            for (int y = 0; y < colors.GetLength(1); y++)
            {
                for (int x = 0; x < colors.GetLength(0); x++)
                {
                    flatColors[y * colors.GetLength(0) + x] = colors[x, y];
                }
            }

            return flatColors;
        }
    }
}