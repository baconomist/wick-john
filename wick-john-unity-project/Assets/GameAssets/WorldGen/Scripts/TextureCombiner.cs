using System;
using GameAssets.WorldGen.Scripts.Generators;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts
{
    public static class TextureCombiner
    {
        public static Texture2D Combine(Texture2D background, Texture2D foreground, Vector2 foregroundPercentPosCenter)
        {
            if (background.width < foreground.width || background.height < foreground.height)
                throw new Exception("Background must be >= foreground");

            Color[] bgPixels = background.GetPixels();
            Color[] fgPixels = foreground.GetPixels();
            Color[] resultPixels = Combine(new Vector2Int(background.width, background.height), bgPixels,
                new Vector2Int(foreground.width, foreground.height), fgPixels, foregroundPercentPosCenter);

            Texture2D resultTex = new Texture2D(background.width, background.height);
            resultTex.SetPixels(resultPixels);
            resultTex.Apply();
            return resultTex;
        }

        public static Color[,] Combine(Color[,] background, Color[,] foreground, Vector2 foregroundPercentPosCenter)
        {
            int bgWidth = background.GetLength(0);
            int bgHeight = background.GetLength(1);
            
            int fgWidth = foreground.GetLength(0);
            int fgHeight = foreground.GetLength(1);
            
            int fg_y = 0;
            for (int y = 0; y < bgHeight; y++)
            {
                int fg_x = 0;
                if (y > (int) (foregroundPercentPosCenter.y * bgHeight - fgHeight / 2f)
                    && y < (int) (foregroundPercentPosCenter.y * bgHeight + fgHeight / 2f))
                {
                    for (int x = 0; x < bgWidth; x++)
                    {
                        if (x > (int) (foregroundPercentPosCenter.x * bgWidth - fgWidth / 2f)
                            && x < (int) (foregroundPercentPosCenter.x * bgWidth + fgWidth / 2f))
                        {
                            background[x, y] = foreground[fg_x, fg_y];
                            fg_x++;
                        }
                    }

                    fg_y++;
                }
            }

            return background;
        }

        public static Color[] Combine(Vector2Int bg_size, Color[] background, Vector2Int fg_size, Color[] foreground,
            Vector2 foregroundPercentPosCenter)
        {
            return Combine(new Vector2Int(background.GetLength(0), background.GetLength(1)),
                background, new Vector2Int(foreground.GetLength(0), foreground.GetLength(1)),
                foreground, foregroundPercentPosCenter);
        }
    }
}