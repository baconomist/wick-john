using System;
using GameAssets.WorldGen.Scripts.Generators.BaseGenerators;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class PlatformGenerator : SpriteGenerator, IChunkableGenerator
    {
        [NonSerialized] public int textureWidth = 1;
        [NonSerialized] public int textureHeight = 1;
        public EdgeCollider2D edgeCollider2D;

        public object GenerateChunkData()
        {
            return GenerateFlatColors();
        }

        public void ApplyChunkData(ChunkGenerator.MapChunk mapChunk)
        {
            SpriteRenderer sp = mapChunk.GameObject.GetComponent<SpriteRenderer>();
            sp.sprite.texture.SetPixels((Color[]) mapChunk.Data);
            sp.sprite.texture.Apply();
            
            mapChunk.GameObject.AddComponent<EdgeCollider2D>().points = new[] {new Vector2(-0.01f, 0), new Vector2(0.01f, 0)};
        }

        public float PreGenerateChunk(GameObject g)
        {
            Texture2D chunkTexture = new Texture2D(textureWidth, textureHeight);

            SpriteRenderer sp = g.AddComponent<SpriteRenderer>();
            sp.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
            
            sp.sprite = Sprite.Create(chunkTexture,
                new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));

            return sp.bounds.size.x;
        }

        private void OnValidate()
        {
            if ((transform.localScale - new Vector3(1, 1, 1)).magnitude < 0.1f)
                transform.localScale = new Vector3(50, 50, 1);
        }

        public override Color[,] GenerateColors()
        {
            Color[,] colors = new Color[textureWidth, textureHeight];
            for (int y = 0; y < textureHeight; y++)
            {
                for (int x = 0; x < textureWidth; x++)
                {
                    colors[x, y] = Color.black;
                }
            }

            return colors;
        }
    }
}