//using System.Collections.Generic;
//using System.Configuration;
//using UnityEngine;

//namespace Assets.Minecraft
//{
//    class World : MonoBehaviour
//    {
//        public GameObject chunkPrefab = null;
//        public Transform WorldParent = null;
//        public Transform Player = null;

//        TerrainGenerator terrainGenerator;
//        Dictionary<Vector2Int, (Chunk, ChunkRenderer)> dict;

//        void Start()
//        {
//            dict = new Dictionary<Vector2Int, (Chunk, ChunkRenderer)>();
//            terrainGenerator = new TerrainGenerator();

//            GenerateChunk(0, 0);
//            SpawnPlayer();
//        }

//        void Update()
//        {
//            LoadChunks();
//            RemoveChunks();
//        }

//        int loadDistance = 1;
//        int loadingIdx = 0;
//        void LoadChunks()
//        {
//            loadingIdx--;
//            if (loadingIdx > 0)
//                return;
//            loadingIdx = 5;

//            bool isMeshMade = false;
//            Vector2Int cam = Player.position.ToChunkCoords();

//            for (int i = 0; i < loadDistance; i++)
//                for (int x = cam.x - i; x <= cam.x + i; x++)
//                    for (int z = cam.y - i; z <= cam.y + i; z++)
//                        if (!GetChunk(x, z).Loaded)
//                        {
//                            GenerateChunk(x, z);
//                            return;
//                        }

//            if (!isMeshMade)
//                loadDistance++;

//            if (loadDistance >= Settings.RenderDistance)
//                loadDistance = 2;
//        }

//        void RemoveChunks()
//        {
//            var renderDist = new Vector2Int((int)Settings.RenderDistance, (int)Settings.RenderDistance);
//            var min = Player.position.ToChunkCoords() - renderDist;
//            var max = Player.position.ToChunkCoords() + renderDist;

//            List<Vector2Int> toDestroy = new List<Vector2Int>();
//            foreach (var kv in dict)
//                if (min.x > kv.Key.x || min.y > kv.Key.y || max.y < kv.Key.y || max.x < kv.Key.x)
//                    toDestroy.Add(kv.Key);

//            foreach (var key in toDestroy)
//            {
//                Destroy(dict[key].Item2.gameObject);
//                dict.Remove(key);
//            }
//        }

//        public void SpawnPlayer()
//        {
//            Player.position = new Vector3(7.5f, GetChunk(0, 0).HeightMap[7, 7] + 3, 7.5f);
//        }

//        public void UpdateChunk(int x, int z)
//        {
//            int cx = x / Settings.ChunkSize.x;
//            int cz = z / Settings.ChunkSize.z;

//            if (x % Settings.ChunkSize.x == 0)
//                _UpdateChunk(cx - 1, cz);
//            else if (x % Settings.ChunkSize.x == Settings.ChunkSize.x - 1)
//                _UpdateChunk(cx + 1, cz);
//            if (z % Settings.ChunkSize.z == 0)
//                _UpdateChunk(cx, cz - 1);
//            else if (z % Settings.ChunkSize.z == Settings.ChunkSize.z - 1)
//                _UpdateChunk(cx, cz + 1);

//            _UpdateChunk(cx, cz);
//        }

//        void _UpdateChunk(int cx, int cz)
//        {
//            dict[new Vector2Int(cx, cz)].Item2.Regenerate();
//        }

//        public BlockType GetBlock(int x, int y, int z)
//        {
//            int cx = x / Settings.ChunkSize.x;
//            int cz = z / Settings.ChunkSize.z;

//            if (!dict.ContainsKey(new Vector2Int(cx, cz)))
//                return BlockType.Air;

//            return GetChunk(cx, cz).GetBlock(x % Settings.ChunkSize.x, y, z % Settings.ChunkSize.z);
//        }
//        public void SetBlock(int x, int y, int z, BlockType type)
//        {
//            int cx = x / Settings.ChunkSize.x;
//            int cz = z / Settings.ChunkSize.z;

//            GetChunk(cx, cz).SetBlock(x % Settings.ChunkSize.x, y, z % Settings.ChunkSize.z, type);
//            UpdateChunk(x, z);
//        }

//        public (Chunk, ChunkRenderer) GenerateChunk(int x, int z)
//        {
//            Vector2Int key = new Vector2Int(x, z);
//            (Chunk, ChunkRenderer) ccr;

//            if (dict.ContainsKey(key))
//                ccr = dict[key];
//            else
//                ccr = AddChunk(x, z);

//            if (ccr.Item1.Loaded)
//                return ccr;

//            for (int ox = -1; ox <= 1; ox++)
//                for (int oz = -1; oz <= 1; oz++)
//                    terrainGenerator.GenerateTerrain(GetChunk(x + ox, z + oz));

//            ccr.Item2.Regenerate();

//            return ccr;
//        }

//        public (Chunk, ChunkRenderer) AddChunk(int x, int z)
//        {
//            Vector2Int key = new Vector2Int(x, z);
//            Chunk chunk = new Chunk(key, this);
//            ChunkRenderer renderer = Instantiate(chunkPrefab, WorldParent).GetComponent<ChunkRenderer>();

//            renderer.chunk = chunk;

//            dict.Add(key, (chunk, renderer));
//            return (chunk, renderer);
//        }

//        public Chunk GetChunk(int chunkx, int chunkz)
//        {
//            var idx = new Vector2Int(chunkx, chunkz);
//            if (!dict.ContainsKey(idx))
//                AddChunk(chunkx, chunkz);
//            return dict[idx].Item1;
//        }
//    }
//}

using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using UnityEngine;

namespace Assets.Minecraft
{
    class World : MonoBehaviour
    {
        public GameObject chunkPrefab = null;
        public Transform WorldParent = null;
        public Transform Player = null;

        WorldLoader loader;
        TerrainGenerator terrainGenerator;
        Dictionary<Vector2Int, (Chunk, ChunkRenderer)> dict;

        void Start()
        {
            loader = new WorldLoader();
            dict = new Dictionary<Vector2Int, (Chunk, ChunkRenderer)>();
            terrainGenerator = new TerrainGenerator();

            GenerateChunk(0, 0);
            SpawnPlayer();
        }

        void Update()
        {
            AddChunks();
            LoadChunks();
            RemoveChunks();
        }

        void OnDestroy()
        {
            loader.SetRunning(false);
        }

        int loadDistance = 1;
        void AddChunks()
        {
            Vector2Int cam = Player.position.ToChunkCoords();

            for (int i = 0; i < loadDistance; i++)
                for (int x = cam.x - i; x <= cam.x + i; x++)
                    for (int z = cam.y - i; z <= cam.y + i; z++)
                    {
                        Chunk c = GetChunk(x, z);
                        if (!c.Generated || !c.Meshed)
                        {
                            GenerateChunk(x, z);
                            return;
                        }
                    }
                        
            
            loadDistance++;
            if (loadDistance >= Settings.RenderDistance)
                loadDistance = 2;
        }

        void LoadChunks()
        {
            while (loader.Dequeue(out LoadedData data))
                if (dict.ContainsKey(data.Pos))
                    dict[data.Pos].Item2.CommitMesh(data);
                else
                    Debug.Log("Allready Deleted");
        }

        void RemoveChunks()
        {
            var renderDist = new Vector2Int((int)Settings.RenderDistance, (int)Settings.RenderDistance);
            var min = Player.position.ToChunkCoords() - renderDist;
            var max = Player.position.ToChunkCoords() + renderDist;

            List<Vector2Int> toDestroy = new List<Vector2Int>();
            foreach (var kv in dict)
                if (min.x > kv.Key.x || min.y > kv.Key.y || max.y < kv.Key.y || max.x < kv.Key.x)
                    toDestroy.Add(kv.Key);

            foreach (var key in toDestroy)
            {
                Destroy(dict[key].Item2.gameObject);
                dict.Remove(key);
            }
        }

        public void SpawnPlayer()
        {
            Player.position = new Vector3(7.5f, GetChunk(0, 0).HeightMap[7, 7] + 3, 7.5f);
        }

        public void UpdateChunk(int x, int z)
        {
            int cx = x / Settings.ChunkSize.x;
            int cz = z / Settings.ChunkSize.z;

            if (x % Settings.ChunkSize.x == 0)
                _UpdateChunk(cx - 1, cz);
            else if (x % Settings.ChunkSize.x == Settings.ChunkSize.x - 1)
                _UpdateChunk(cx + 1, cz);
            if (z % Settings.ChunkSize.z == 0)
                _UpdateChunk(cx, cz - 1);
            else if (z % Settings.ChunkSize.z == Settings.ChunkSize.z - 1)
                _UpdateChunk(cx, cz + 1);

            _UpdateChunk(cx, cz);
        }

        void _UpdateChunk(int cx, int cz)
        {
            dict[new Vector2Int(cx, cz)].Item2.Regenerate();
        }

        public BlockType GetBlock(int x, int y, int z)
        {
            int cx = x / Settings.ChunkSize.x;
            int cz = z / Settings.ChunkSize.z;

            if (!dict.ContainsKey(new Vector2Int(cx, cz)))
                return BlockType.Air;

            return GetChunk(cx, cz).GetBlock(x % Settings.ChunkSize.x, y, z % Settings.ChunkSize.z);
        }
        public void SetBlock(int x, int y, int z, BlockType type)
        {
            int cx = x / Settings.ChunkSize.x;
            int cz = z / Settings.ChunkSize.z;

            GetChunk(cx, cz).SetBlock(x % Settings.ChunkSize.x, y, z % Settings.ChunkSize.z, type);
            UpdateChunk(x, z);
        }

        public (Chunk, ChunkRenderer) GenerateChunk(int x, int z)
        {
            Vector2Int key = new Vector2Int(x, z);
            (Chunk, ChunkRenderer) ccr;

            if (dict.ContainsKey(key))
                ccr = dict[key];
            else
                ccr = AddChunk(x, z);

            Debug.Log("Meshed");
            if (ccr.Item1.Meshed)
                return ccr;

            for (int ox = -1; ox <= 1; ox++)
                for (int oz = -1; oz <= 1; oz++)
                    terrainGenerator.GenerateTerrain(GetChunk(x + ox, z + oz));

            loader.Enqueue(ccr.Item1);

            return ccr;
        }

        public (Chunk, ChunkRenderer) AddChunk(int x, int z)
        {
            Vector2Int key = new Vector2Int(x, z);
            Chunk chunk = new Chunk(key, this);
            ChunkRenderer renderer = Instantiate(chunkPrefab, WorldParent).GetComponent<ChunkRenderer>();

            renderer.chunk = chunk;

            dict.Add(key, (chunk, renderer));
            return (chunk, renderer);
        }

        public Chunk GetChunk(int chunkx, int chunkz)
        {
            var idx = new Vector2Int(chunkx, chunkz);
            if (!dict.ContainsKey(idx))
                AddChunk(chunkx, chunkz);
            return dict[idx].Item1;
        }
    }
}
