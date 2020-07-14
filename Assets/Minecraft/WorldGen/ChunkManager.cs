using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Minecraft
{
    class ChunkManager : MonoBehaviour
    {
        public GameObject chunkPrefab = null;
        public Transform WorldParent = null;
        public Transform Player = null;
        public World world = null;
        
        WorldLoader loader;
        Dictionary<Vector2Int, (Chunk, ChunkRenderer)> dict;
        Dictionary<Vector2Int, ChunkRenderer> chunksToUpdate;

        void Start()
        {
            dict = new Dictionary<Vector2Int, (Chunk, ChunkRenderer)>();
            chunksToUpdate = new Dictionary<Vector2Int, ChunkRenderer>();

            loader = new WorldLoader();
        }

        public void OnDestroy()
        {
            loader.SetRunning(false);
        }

        public void Update()
        {
            AddChunks();
            MeshChunks();
            UpdateChunks();
            RemoveChunks();
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
                            world.GenerateChunk(x, z);
                            return;
                        }
                    }


            loadDistance++;
            if (loadDistance >= Settings.RenderDistance)
                loadDistance = 2;
        }

        void MeshChunks()
        {
            while (loader.Dequeue(out LoadedData data))
                if (dict.ContainsKey(data.Pos))
                    dict[data.Pos].Item2.CommitMesh(data);
                else
                    Debug.Log("Allready Deleted");
        }

        void UpdateChunks()
        {
            foreach (var kv in chunksToUpdate)
            {
                Debug.Log("Regen");
                kv.Value.Regenerate();
                chunksToUpdate.Remove(kv.Key);
                break;
            }
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

        public void Enqueue(Chunk c)
        {
            loader.Enqueue(c);
        }
        public Chunk AddChunk(int x, int z)
        {
            var key = new Vector2Int(x, z);
            Chunk chunk = new Chunk(key, world);
            ChunkRenderer renderer = Instantiate(chunkPrefab, WorldParent).GetComponent<ChunkRenderer>();

            renderer.chunk = chunk;

            dict.Add(key, (chunk, renderer));
            return chunk;
        }

        public Chunk GetChunk(int chunkx, int chunkz)
        {
            if (!ChunkExists(chunkx, chunkz))
                AddChunk(chunkx, chunkz);
            return dict[new Vector2Int(chunkx, chunkz)].Item1;
        }

        public bool ChunkExists(int cx, int cz)
        {
            return dict.ContainsKey(new Vector2Int(cx, cz));
        }

        public void UpdateChunk(int x, int z)
        {
            Vector2Int c = Util.ToChunkCoords(x, z);

            if (x % Settings.ChunkSize.x == 0)
                _UpdateChunk(c.x - 1, c.y);
            else if (x % Settings.ChunkSize.x == Settings.ChunkSize.x - 1)
                _UpdateChunk(c.x + 1, c.y);
            if (z % Settings.ChunkSize.z == 0)
                _UpdateChunk(c.x, c.y - 1);
            else if (z % Settings.ChunkSize.z == Settings.ChunkSize.z - 1)
                _UpdateChunk(c.x, c.y + 1);

            _UpdateChunk(c.x, c.y);
        }

        void _UpdateChunk(int cx, int cz)
        {
            var key = new Vector2Int(cx, cz);
            if (!chunksToUpdate.ContainsKey(key) && dict.ContainsKey(key))
            {
                var ccr = dict[key];
                if (ccr.Item1.Meshed)
                    chunksToUpdate.Add(key, ccr.Item2);
            }
        }

    }
}
