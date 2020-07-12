using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators.BaseGenerators
{
    public interface IChunkedGenerator
    {
        object GenerateChunkData();
        void ApplyChunkData(ChunkGenerator.MapChunk mapChunk);
        int GetChunkWidth();
        int GetChunkHeight();
    }

    public class ChunkGenerator : MonoBehaviour
    {
        public GameObject iChunkedGeneratorGameObject;
        public IChunkedGenerator iChunkedGenerator;
        
        public Transform chunkLocatorTransform;

        [Range(0, 10)] public int renderDistance = 1;

        private Vector3 _lastChunkPos;
        private List<MapChunk> _chunks;
        private float _chunkWorldWidth = -1;
        private Queue<MapChunk> _mapRequestQueue;

        private void Start()
        {
            iChunkedGenerator = iChunkedGeneratorGameObject.GetComponent<Generator>() as IChunkedGenerator;

            _chunks = new List<MapChunk>();
            _mapRequestQueue = new Queue<MapChunk>();
            _lastChunkPos = transform.position;

            // Can only have a single map generator thread since generators are instanced. Maybe use unity scriptable objects to store generator data instead?
            new Thread(new ThreadStart(delegate { MapGeneratorThread(); })).Start();
            
            _chunks.Add(GenerateChunk());
        }
        
        private void Update()
        {
            if (_chunks.Count > 0 &&
                chunkLocatorTransform.position.x - _chunks[0].GameObject.transform.position.x > _chunkWorldWidth)
            {
                Destroy(_chunks[0].GameObject);
                _chunks.RemoveAt(0);
            }

            if (_chunks.Count == 0 || (_chunks[0].DataReceived && _lastChunkPos.x - chunkLocatorTransform.position.x < _chunkWorldWidth * renderDistance))
            {
                _chunks.Add(GenerateChunk());
            }

            for (int i = 0; i < _chunks.Count; i++)
            {
                if (!_chunks[i].DataUpdated && _chunks[i].DataReceived)
                {
                    iChunkedGenerator.ApplyChunkData(_chunks[i]);

                    _chunks[i].DataUpdated = true;
                    
                    // TODO: maybe fix this? this seems kinda bad
                    _chunkWorldWidth = _chunks[i].GameObject.GetComponent<SpriteRenderer>().bounds.size.x;
                }
            }
        }
        
        private MapChunk GenerateChunk()
        {
            int chunkWidth = iChunkedGenerator.GetChunkWidth();
            int chunkHeight = iChunkedGenerator.GetChunkHeight();
            
            Texture2D chunkTexture = new Texture2D(chunkWidth, chunkHeight);

            GameObject g = new GameObject("Map Chunk");
            
            SpriteRenderer sp = g.AddComponent<SpriteRenderer>();
            sp.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
            
            sp.sprite = Sprite.Create(chunkTexture,
                new Rect(0, 0, chunkWidth, chunkHeight), new Vector2(0.5f, 0.5f));
            g.transform.parent = transform;
            // Same scale as parent since LOCAL-scale
            g.transform.localScale = new Vector3(1, 1, 1);
            g.transform.position = _lastChunkPos + new Vector3(sp.bounds.size.x, 0, 0);

            _lastChunkPos = g.transform.position;

            return new MapChunk(g , this);
        }

        private void RequestData(MapChunk mapChunk)
        {
            _mapRequestQueue.Enqueue(mapChunk);   
        }

        private void MapGeneratorThread()
        {
            // TODO: fix while(true)
            while (true)
            {
                if (_mapRequestQueue.Count > 0)
                {
                    MapChunk mapChunk = _mapRequestQueue.Dequeue();
                    mapChunk.OnDataReceived(iChunkedGenerator.GenerateChunkData());
                }
            }
        }

        public class MapChunk
        {
            public GameObject GameObject;
            public object Data;
            public bool DataReceived = false;
            public bool DataUpdated = false;

            public MapChunk(GameObject gameObject, ChunkGenerator bg)
            {
                GameObject = gameObject;

                bg.RequestData(this);
            }

            public void OnDataReceived(object data)
            {
                Data = data;
                DataReceived = true;
            }
        }
    }
}