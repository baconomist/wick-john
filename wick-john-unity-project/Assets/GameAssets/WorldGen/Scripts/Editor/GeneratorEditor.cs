using System;
using System.IO;
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
                UpdateGeneratorPreview(generator);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Generator generator = target as Generator;

            if (GUILayout.Button("Generate"))
            {
                UpdateGeneratorPreview(generator);
            }

            if (generator is VisualGenerator visualGenerator && GUILayout.Button("Save Texture"))
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

        private void UpdateGeneratorPreview(Generator generator)
        {
            if (generator is VisualGenerator visualGenerator)
            {
                Texture2D generatedTexture = visualGenerator.GenerateTexture();
                
                if (visualGenerator.gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    Sprite sprite = Sprite.Create(generatedTexture,
                        new Rect(0, 0, generatedTexture.width, generatedTexture.height), new Vector2(0.5f, 0.5f));
                    visualGenerator.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
                }
            }
            else
            {
                generator.Generate();
            }
        }
    }
}