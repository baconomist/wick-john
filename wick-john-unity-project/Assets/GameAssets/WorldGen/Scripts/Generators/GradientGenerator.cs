using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class GradientGenerator : VisualGenerator
    {
        public Color fromColor = Color.black;
        public Color toColor = Color.white;
        [Range(0, 1)] public float start = 0;
        [Range(0, 1)] public float end = 1;

        public bool horizontal = false;

        public override Color[,] GenerateColors()
        {
            Color[,] gradientPixels = new Color[width, height];

            gradientPixels = GenerateVerticalGradient(gradientPixels);
            
            return gradientPixels;
        }

        private Color[,] GenerateVerticalGradient(Color[,] gradientPixels)
        {
            for (int y = 0; y < height; y++)
            {
                if ((float) y / height > start && (float) y / height < end)
                {
                    for (int x = 0; x < width; x++)
                    {
                        gradientPixels[x, y] = Color.Lerp(toColor, fromColor,
                            (float) (y - start * height + (1 - end) * height) / height);
                    }
                }
                else
                {
                    for (int x = 0; x < width; x++)
                    {
                        if ((float) y / height < start)
                            gradientPixels[x, y] = toColor;
                        else
                            gradientPixels[x, y] = fromColor;
                    }
                }
            }

            return gradientPixels;
        }
    }
}