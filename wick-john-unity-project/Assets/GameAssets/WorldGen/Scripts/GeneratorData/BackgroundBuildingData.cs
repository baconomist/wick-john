using GameAssets.WorldGen.Scripts.Generators;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameAssets.WorldGen.Scripts.GeneratorData
{
    [CreateAssetMenu(fileName = "BackgroundBuildingData", menuName = "WorldGen/BackgroundBuildingData", order = 0)]
    public class BackgroundBuildingData : SpriteGeneratorData
    {
        [Header("Random Color Range")] public Color fromColor = Color.grey;
        public Color toColor = Color.black;
        public bool useBackgroundGradient = false;
        public GradientData[] backgroundGradientData;

        [Header("Windows")] public int minWindows;
        public float maxWindowsToBuildingSizeRatio = 2.0f;
        public Color staticWindowColor;
        public bool randomWindowColor = false;
        public Color fromWindowColor;
        public Color toWindowColor;
        public bool useWindowGradient = false;
        public GradientData windowGradientData;

        [FormerlySerializedAs("minWidth")] [Header("Width")] public int minTextWidth;
        [FormerlySerializedAs("maxWidth")] public int maxTextWidth;

        [FormerlySerializedAs("minHeight")] [Header("Height")] public int minTextHeight;
        [FormerlySerializedAs("maxHeight")] public int maxTextHeight;
    }
}