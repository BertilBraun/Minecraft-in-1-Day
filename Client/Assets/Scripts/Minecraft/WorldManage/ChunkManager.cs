using Assets.Scripts.Minecraft.WorldManage;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Minecraft
{
    public class ChunkManager : MonoBehaviour
    {
        public static ChunkManager Get;

        Dictionary<(int,int), Chunk> dict = new Dictionary<(int, int), Chunk>();
        List<ChunkSection> chunksToUpdate = new List<ChunkSection>();
        
        private void Awake()
        {
            if (Get == null)
                Get = this;
            else if (Get != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        public void Update()
        {
            UpdateChunks();
            RemoveChunks();
        }

        void UpdateChunks()
        {
            foreach (var section in chunksToUpdate)
            {
                ChunkRenderManager.Get.UpdateSection(section);
                chunksToUpdate.Remove(section);
                break;
            }
        }
        void RemoveChunks()
        {
            var renderDist = new Vector2Int((int)Settings.RenderDistance, (int)Settings.RenderDistance);
            var min = GameManager.Get.localPlayerTransform.position.ToChunkCoords() - renderDist;
            var max = GameManager.Get.localPlayerTransform.position.ToChunkCoords() + renderDist;

            List<(int,int)> toRemove = new List<(int, int)>();
            foreach (var kv in dict)
                if (min.x > kv.Key.Item1 || min.y > kv.Key.Item2 || max.y < kv.Key.Item2 || max.x < kv.Key.Item1)
                    toRemove.Add(kv.Key);

            foreach (var key in toRemove)
            {
                ChunkRenderManager.Get.RemoveChunk(key.Item1, key.Item2);
                dict.Remove(key);
            }
        }

        public void AddChunk(Chunk c)
        {
            dict.Add((c.Pos.x, c.Pos.y), c);
            ChunkRenderManager.Get.AddChunk(c);
        }

        public Chunk GetChunk(int chunkx, int chunkz)
        {
            var key = (chunkx, chunkz);
            if (!dict.ContainsKey(key))
                return null;

            return dict[key];
        }

        public ChunkSection GetChunkSection(int x, int y, int z)
        {
            var chunkPos = new Vector3(x, y, z).ToChunkCoords();
            var key = (chunkPos.x, chunkPos.y);
            if (!dict.ContainsKey(key))
                return null;

            return dict[key].sections[y / Settings.ChunkSectionSize.y];
        }

        public bool ChunkExists(int cx, int cz)
        {
            return dict.ContainsKey((cx, cz));
        }

        public void UpdateChunk(int x, int y, int z)
        {
            Vector3Int c = Util.ToChunkCoords(x, y, z);

            if (x % Settings.ChunkSectionSize.x == 0)
                _UpdateChunk(c.x - 1, c.y, c.z);
            else if (x % Settings.ChunkSectionSize.x == Settings.ChunkSectionSize.x - 1)
                _UpdateChunk(c.x + 1, c.y, c.z);

            if (y % Settings.ChunkSectionSize.y == 0 && c.y != 0)
                _UpdateChunk(c.x, c.y - 1, c.z);
            else if (y % Settings.ChunkSectionSize.y == Settings.ChunkSectionSize.y - 1 && c.y != Settings.ChunkSectionsPerChunk - 1)
                _UpdateChunk(c.x, c.y + 1, c.z);

            if (z % Settings.ChunkSectionSize.z == 0)
                _UpdateChunk(c.x, c.y, c.z - 1);
            else if (z % Settings.ChunkSectionSize.z == Settings.ChunkSectionSize.z - 1)
                _UpdateChunk(c.x, c.y, c.z + 1);

            _UpdateChunk(c.x, c.y, c.z);
        }
        void _UpdateChunk(int cx, int cy, int cz)
        {
            var key = (cx, cz);
            if (!dict.ContainsKey(key))
                return;

            ChunkSection section = dict[key].sections[cy];
            if (!chunksToUpdate.Contains(section))
                chunksToUpdate.Add(section);
        }

    }
}
