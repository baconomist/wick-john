using System;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public abstract class SpriteGenerator : Generator
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

        public override void GeneratePreview()
        {
            SpriteRenderer sp = gameObject.GetComponent<SpriteRenderer>();
            if (sp == null)
                sp = gameObject.AddComponent<SpriteRenderer>();
            GenerateOn(gameObject);
        }

        public override void GenerateOn(GameObject parentGameObject)
        {
            Texture2D generatedTexture = GenerateTexture();
            Sprite sprite = Sprite.Create(generatedTexture,
                new Rect(0, 0, generatedTexture.width, generatedTexture.height),
                new Vector2(0.5f, 0.5f));

            gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
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