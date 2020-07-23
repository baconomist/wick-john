using UnityEngine;

namespace GameAssets.WorldGen.Scripts.GeneratorData
{
    [CreateAssetMenu(fileName = "GradientData", menuName = "WorldGen/GradientData", order = 0)]
    public class GradientData : SpriteGeneratorData
    {
        public Color fromColor = Color.black;
        public Color toColor = Color.white;
        [Range(0, 1)] public float start = 0;
        [Range(0, 1)] public float end = 1;

        public bool horizontal = false;
    }
}