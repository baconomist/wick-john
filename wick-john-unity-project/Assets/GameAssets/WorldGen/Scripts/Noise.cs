using UnityEngine;

namespace GameAssets.WorldGen.Scripts
{
    public static class Noise
    {
        public enum NormalizeMode
        {
            Local,
            GlobalEstimate
        }

        /**
     * width, height: Just the size of the map
     * scale: just the scale
     * octaves: # of maps to generate which are then combined into the final map to produce
     *         a more natural landscape. Ie an octave for the terrain, an octave for rocks, for trees...
     * persistance: controls decrease in amplitude of octaves. octave_amplitude *= persistance ^ octave#
     * lacunarity: controls increase in frequency of each octave. octave_frequency *= lacunarity ^ octave#
     */
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            float amplitude = 1;
            float frequency = 1;

            float maxPossibleHeight = 0;

            // Generate map offsets for each octave based on our map seed
            System.Random prng = new System.Random(settings.seed);
            Vector2[] octaveOffsets = new Vector2[settings.octaves];
            for (int i = 0; i < settings.octaves; i++)
            {
                // Largest range offset can be in, otherwise PerlinNoise breaks and returns the same value
                float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
                float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCenter.y;

                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                // Used for global normalization
                maxPossibleHeight += amplitude;
                amplitude *= settings.persistance;
            }

            // Arbitrary values
            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;

                    // Process octaves
                    for (int i = 0; i < settings.octaves; i++)
                    {
                        // x - halfWidth to scale from the center of the map rather than corner
                        // * frequency since sin(x * frequency)... increases x values and makes y values farther apart
                        // (/scale) since in functions, af[(x - d) / k] + c is the transformation formula
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                        // * 2 - 1 to make it in the range [-1, 1] so our noiseHeight can sometimes decrease
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        // Add to the current noiseHeight value(adds octaves)
                        noiseHeight += perlinValue * amplitude;

                        // Amplitude decreases each octave (persistance is in range 0 - 1)
                        amplitude *= settings.persistance;
                        // Frequency increases each octave (lacunarity is > 1)
                        frequency *= settings.lacunarity;
                    }

                    // Keep track of what our min/max noise heights are
                    if (noiseHeight > maxLocalNoiseHeight)
                        maxLocalNoiseHeight = noiseHeight;
                    if (noiseHeight < minLocalNoiseHeight)
                        minLocalNoiseHeight = noiseHeight;

                    // Set the noise height value after adding together the octaves
                    noiseMap[x, y] = noiseHeight;
                
                    // Estimates normalization to remove chunk seems
                    if (settings.normalizeMode == NormalizeMode.GlobalEstimate)
                    {
                        // Reverse above operation in perlinValue=
                        float normalizedHeight = (noiseMap[x, y] + 1) / 2f;
                        // Normalize by finding a "percentage" or dividing by maxPossibleHeight
                        normalizedHeight /= maxPossibleHeight;
                        // Some arbitrary estimated value to multiply by to get good results,
                        // without it everything is too flat since our maxPossibleHeight is extremely high
                        normalizedHeight *= 1.75f;

                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            // Only works for generating a single map/chunk
            if (settings.normalizeMode == NormalizeMode.Local)
            {
                // Normalize noiseMap into values [0, 1]
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        // returns a value [0, 1](a percentage) from [min, max] from our noiseHeight value
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    }
                }
            }

            return noiseMap;
        }
    }
}