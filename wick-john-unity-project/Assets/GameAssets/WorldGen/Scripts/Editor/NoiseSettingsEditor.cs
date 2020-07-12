using GameAssets.WorldGen.Scripts.Generators;
using UnityEditor;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Editor
{
    [CustomEditor(typeof(NoiseSettings))]
    public class NoiseViewerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(100, 100, (target as NoiseSettings), Vector2.zero);
            
            GUILayout.Label("Preview:");
            GUILayout.Label(TextureGenerator.TextureFromNoiseMap(noiseMap));

            base.OnInspectorGUI();
        }
    }
}