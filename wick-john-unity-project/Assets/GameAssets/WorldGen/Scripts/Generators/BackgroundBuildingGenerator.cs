using System;
using System.Diagnostics;
using GameAssets.WorldGen.Scripts.GeneratorData;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameAssets.WorldGen.Scripts.Generators
{
    // TODO: use Perlin noise?
    public class BackgroundBuildingGenerator : SpriteGenerator
    {
        public BackgroundBuildingData backgroundBuildingData;
        public GradientGenerator gradientGenerator;

        public override Color[,] GenerateColors()
        {
            int width = ThreadSafeRandom.Range(backgroundBuildingData.minTextWidth, backgroundBuildingData.maxTextWidth);
            int height = ThreadSafeRandom.Range(backgroundBuildingData.minTextHeight, backgroundBuildingData.maxTextHeight);

            Color[,] pixels = new Color[width, height];
            
            if (backgroundBuildingData.useBackgroundGradient)
            {
                int backgroundGenIndex =
                    ThreadSafeRandom.Range(0, backgroundBuildingData.backgroundGradientData.Length);
                backgroundBuildingData.backgroundGradientData[backgroundGenIndex].textureWidth = width;
                backgroundBuildingData.backgroundGradientData[backgroundGenIndex].textureHeight = height;

                gradientGenerator.gradientData = backgroundBuildingData.backgroundGradientData[backgroundGenIndex];
                pixels = gradientGenerator.GenerateColors();
            }
            else
            {
                Color randColor =
                    ThreadSafeRandom.Color(backgroundBuildingData.fromColor, backgroundBuildingData.toColor);
                
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[x, y] = randColor;
                    }
                }
            }

            pixels = GenerateWindows(pixels,
                ThreadSafeRandom.Range(backgroundBuildingData.minWindows, (int) (width / backgroundBuildingData.minTextWidth * backgroundBuildingData.maxWindowsToBuildingSizeRatio)),
                ThreadSafeRandom.Range(2, 5));

            return pixels;
        }

        /**
         * Window size calculated based on numCols, building filled in based on numRows
         */
        private Color[,] GenerateWindows(Color[,] pixels, int numCols, int numRows)
        {
            // TODO: add exception?
            if (numCols <= 0 || numRows <= 0)
                throw new Exception("Something isn't right...");

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

            Color windowColor = backgroundBuildingData.staticWindowColor;
            if (backgroundBuildingData.randomWindowColor)
                windowColor = ThreadSafeRandom.Color(backgroundBuildingData.fromWindowColor,
                    backgroundBuildingData.toWindowColor, true);

            if (backgroundBuildingData.useWindowGradient)
            {
                backgroundBuildingData.windowGradientData.textureWidth = width;
                backgroundBuildingData.windowGradientData.textureHeight = height;

                backgroundBuildingData.windowGradientData.fromColor = windowColor;
                backgroundBuildingData.windowGradientData.toColor =
                    ThreadSafeRandom.Color(backgroundBuildingData.fromWindowColor, backgroundBuildingData.toWindowColor,
                        true);

                gradientGenerator.gradientData = backgroundBuildingData.windowGradientData;
                return gradientGenerator.GenerateColors();
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