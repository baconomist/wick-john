using UnityEngine;

namespace GameAssets.WorldGen.Scripts
{
    [System.Serializable]
    [CreateAssetMenu()]
    public class NoiseSettings : ScriptableObject
    {
        [Header("IMPORTANT: Set Normalize Mode To Global To Fix Seems")]
        public Noise.NormalizeMode normalizeMode = Noise.NormalizeMode.Local;

        public float scale = 50;

        // Just arbitrary max of 50
        [Range(0, 50)]
        public int octaves = 6;

        // 0 <= persistance <= 1, control rate of decrease of amplitude of detail
        [Range(0, 1)] public float persistance = 0.6f;

        // Lacunarity >= 1, control amount of detail/frequency
        public float lacunarity = 2f;

        public int seed;
        public Vector2 offset;

        public void ValidateValues()
        {
            // noiseScale > 0
            scale = Mathf.Max(scale, 0.01f);

            // lacunarity >= 1
            lacunarity = Mathf.Max(lacunarity, 1);

            // octaves >= 1
            Mathf.Max(octaves, 1);

            persistance = Mathf.Clamp01(persistance);
        }
    }
}