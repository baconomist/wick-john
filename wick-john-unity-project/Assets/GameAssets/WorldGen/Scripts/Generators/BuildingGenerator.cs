using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameAssets.WorldGen.Scripts.Generators
{
    // TODO: use Perlin noise?
    public class BuildingGenerator : SpriteGenerator
    {
        [Header("Random Color Range")] public Color fromColor = Color.grey;
        public Color toColor = Color.black;
        public bool useBackgroundGradient = false;
        public GradientGenerator[] backgroundGradientGenerators;

        [Header("Windows")] public int minWindows;
        public Color staticWindowColor;
        public bool randomWindowColor = false;
        public Color fromWindowColor;
        public Color toWindowColor;
        public bool useWindowGradient = false;
        public GradientGenerator windowGradientGenerator;

        [Header("Width")] public int minWidth;
        public int maxWidth;

        [Header("Height")] public int minHeight;
        public int maxHeight;

        public override Color[,] GenerateColors()
        {
            int width = ThreadSafeRandom.Range(minWidth, maxWidth);
            int height = ThreadSafeRandom.Range(minHeight, maxHeight);

            Color randColor = ThreadSafeRandom.Color(fromColor, toColor);

            Color[,] pixels = new Color[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[x, y] = randColor;
                }
            }

            int backgroundGenIndex = ThreadSafeRandom.Range(0, backgroundGradientGenerators.Length);
            backgroundGradientGenerators[backgroundGenIndex].width = width;
            backgroundGradientGenerators[backgroundGenIndex].height = height;

            pixels = backgroundGradientGenerators[backgroundGenIndex].GenerateColors();

            pixels = GenerateWindows(pixels, ThreadSafeRandom.Range(minWindows, width / minWidth * 2), ThreadSafeRandom.Range(2, 5));

            return pixels;
        }

        /**
         * Window size calculated based on numCols, building filled in based on numRows
         */
        private Color[,] GenerateWindows(Color[,] pixels, int numCols, int numRows)
        {
            // TODO: add exception?
            if (numCols <= 0 || numRows <= 0)
                return new Color[0, 0];

            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            float borderPercent = 0.05f;
            float windowSizePercent = (1.0f - numCols * borderPercent - borderPercent) / (numCols);

            int windowWidth = (int) (width * windowSizePercent);
            int windowHeight = (int) (height * windowSizePercent);

            Color[,] windowColors = GenerateWindow(windowWidth, windowHeight);
            for (int y = 0; y < numRows; y++)
            {
                for (int x = 0; x < numCols; x++)
                {
                    pixels = TextureCombiner.Combine(pixels, windowColors, new Vector2(
                        borderPercent + windowSizePercent / 2f + x * (windowSizePercent + borderPercent),
                        1 - (borderPercent + windowSizePercent / 2f + y * (windowSizePercent + borderPercent))));
                }
            }

            return pixels;
        }

        private Color[,] GenerateWindow(int width, int height)
        {
            Color[,] pixels = new Color[width, height];

            Color windowColor = staticWindowColor;
            if (randomWindowColor)
                windowColor = ThreadSafeRandom.Color(fromWindowColor, toWindowColor, true);

            if (useWindowGradient)
            {
                windowGradientGenerator.width = width;
                windowGradientGenerator.height = height;

                windowGradientGenerator.fromColor = windowColor;
                windowGradientGenerator.toColor = ThreadSafeRandom.Color(fromWindowColor, toWindowColor, true);

                return windowGradientGenerator.GenerateColors();
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[x, y] = windowColor;
                }
            }

            return pixels;
        }
    }
}