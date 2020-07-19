using GameAssets.WorldGen.Scripts.Generators.BaseGenerators;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public class PlatformGenerator : SpriteGenerator, IChunkedGenerator
    {
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

        public int GetChunkWidth()
        {
            return width;
        }

        public int GetChunkHeight()
        {
            return height;
        }

        private void OnValidate()
        {
            if ((transform.localScale - new Vector3(1, 1, 1)).magnitude < 0.1f)
                transform.localScale = new Vector3(50, 50, 1);
        }

        public override Color[,] GenerateColors()
        {
            Color[,] colors = new Color[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[x, y] = Color.black;
                }
            }

            return colors;
        }
    }
}