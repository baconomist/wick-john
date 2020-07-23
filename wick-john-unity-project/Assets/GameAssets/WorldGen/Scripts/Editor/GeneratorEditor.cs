using System;
using System.IO;
using System.Reflection;
using GameAssets.WorldGen.Scripts.Generators;
using UnityEditor;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Editor
{
    [CustomEditor(typeof(Generator), true)]
    public class GeneratorEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            Generator.OnValidateEvent -= OnGeneratorValidate;
            Generator.OnValidateEvent += OnGeneratorValidate;
        }

        private void OnGeneratorValidate(Generator generator)
        {
            if (generator.autoUpdate && target as Generator == generator)
            {
                GeneratePreview();
            }
        }

        public void GeneratePreview()
        {
            (target as Generator)?.GeneratePreview();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Generator generator = target as Generator;

            if (generator != null && GUILayout.Button("Generate Preview"))
            {
                GeneratePreview();
            }

            if (generator is SpriteGenerator visualGenerator && GUILayout.Button("Save Texture"))
            {
                var path = EditorUtility.SaveFilePanel(
                    "Save texture as PNG",
                    "",
                    visualGenerator.name + ".png",
                    "png");

                if (path.Length != 0)
                {
                    var pngData = generator.GetComponent<SpriteRenderer>().sprite.texture.EncodeToPNG();
                    if (pngData != null)
                        File.WriteAllBytes(path, pngData);
                }
            }
        }
    }
}