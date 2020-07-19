using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators.BaseGenerators
{
    public interface IChunkableGenerator
    {
        object GenerateChunkData();
        void ApplyChunkData(ChunkGenerator.MapChunk mapChunk);
        float PreGenerateChunk(GameObject g);
    }

    public class ChunkGenerator : MonoBehaviour
    {
        public GameObject iChunkedGeneratorGameObject;
        public IChunkableGenerator iChunkableGenerator;

        public Transform chunkLocatorTransform;

        [Range(0, 10)] public int renderDistance = 1;

        private Vector3 _lastChunkPos;
        private float _lastChunkWidth;
        private List<MapChunk> _chunks;
        private float _chunkWorldWidth = -1;
        private Queue<MapChunk> _mapRequestQueue;

        private void Start()
        {
            iChunkableGenerator = iChunkedGeneratorGameObject.GetComponent<Generator>() as IChunkableGenerator;

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

            if (_chunks.Count == 0 || (_chunks[0].DataReceived &&
                                       _lastChunkPos.x - chunkLocatorTransform.position.x <
                                       _chunkWorldWidth * renderDistance))
            {
                _chunks.Add(GenerateChunk());
            }

            for (int i = 0; i < _chunks.Count; i++)
            {
                if (!_chunks[i].DataUpdated && _chunks[i].DataReceived)
                {
                    iChunkableGenerator.ApplyChunkData(_chunks[i]);

                    _chunks[i].DataUpdated = true;

                    // TODO: maybe fix this? this seems kinda bad
                    _chunkWorldWidth = _chunks[i].GameObject.GetComponent<SpriteRenderer>().bounds.size.x;
                }
            }
        }

        private MapChunk GenerateChunk()
        {
            GameObject g = new GameObject(iChunkableGenerator.GetType().Name + "  Map Chunk");
            g.transform.parent = transform;
            // Same scale as parent since LOCAL-scale
            g.transform.localScale = new Vector3(1, 1, 1);
            
            float chunkWorldWidth = iChunkableGenerator.PreGenerateChunk(g);
            g.transform.position = _lastChunkPos + new Vector3(_lastChunkWidth / 2f + chunkWorldWidth / 2f, 0, 0);

            _lastChunkPos = g.transform.position;
            _lastChunkWidth = chunkWorldWidth;

            return new MapChunk(g, this);
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
                    mapChunk.OnDataReceived(iChunkableGenerator.GenerateChunkData());
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