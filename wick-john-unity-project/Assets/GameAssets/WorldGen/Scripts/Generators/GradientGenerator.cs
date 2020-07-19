using GameAssets.WorldGen.Scripts.GeneratorData;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class GradientGenerator : SpriteGenerator
    {
        public GradientData gradientData;

        public override Color[,] GenerateColors()
        {
            Color[,] gradientPixels = new Color[gradientData.textureWidth, gradientData.textureHeight];

            gradientPixels = GenerateVerticalGradient(gradientPixels);
            
            return gradientPixels;
        }

        private Color[,] GenerateVerticalGradient(Color[,] gradientPixels)
        {
            for (int y = 0; y < gradientData.textureHeight; y++)
            {
                if ((float) y / gradientData.textureHeight > gradientData.start && (float) y / gradientData.textureHeight < gradientData.end)
                {
                    for (int x = 0; x < gradientData.textureWidth; x++)
                    {
                        gradientPixels[x, y] = Color.Lerp(gradientData.toColor, gradientData.fromColor,
                            (float) (y - gradientData.start * gradientData.textureHeight + (1 - gradientData.end) * gradientData.textureHeight) / gradientData.textureHeight);
                    }
                }
                else
                {
                    for (int x = 0; x < gradientData.textureWidth; x++)
                    {
                        if ((float) y / gradientData.textureHeight < gradientData.start)
                            gradientPixels[x, y] = gradientData.toColor;
                        else
                            gradientPixels[x, y] = gradientData.fromColor;
                    }
                }
            }

            return gradientPixels;
        }
    }
}