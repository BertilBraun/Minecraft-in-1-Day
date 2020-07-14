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

        Dictionary<Vector2Int, Chunk> dict = new Dictionary<Vector2Int, Chunk>();
        List<ChunkSection> chunksToUpdate = new List<ChunkSection>();
        
       // TODO rem Dictionary<Vector3Int, ChunkRenderer> chunksToUpdate = new Dictionary<Vector3Int, ChunkRenderer>();


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

            List<Vector2Int> toRemove = new List<Vector2Int>();
            foreach (var kv in dict)
                if (min.x > kv.Key.x || min.y > kv.Key.y || max.y < kv.Key.y || max.x < kv.Key.x)
                    toRemove.Add(kv.Key);

            foreach (var key in toRemove)
            {
                ChunkRenderManager.Get.RemoveChunk(key);
                dict.Remove(key);
            }
        }

        public void AddChunk(Chunk c)
        {
            dict[c.Pos] = c;

            TryGen(c.Pos - new Vector2Int(-1,  0));
            TryGen(c.Pos - new Vector2Int( 1,  0));
            TryGen(c.Pos - new Vector2Int( 0, -1));
            TryGen(c.Pos - new Vector2Int( 0,  1));

            TryGen(c.Pos);
        }

        void TryGen(Vector2Int pos)
        {
            if (dict.ContainsKey(pos - new Vector2Int(-1, 0)) &&
                dict.ContainsKey(pos - new Vector2Int(1, 0)) &&
                dict.ContainsKey(pos - new Vector2Int(0, -1)) &&
                dict.ContainsKey(pos - new Vector2Int(0, 1)))
                // TODO - only if all surrounding ones have also loaded?
                ChunkRenderManager.Get.AddChunk(dict[pos]);
        }

        public Chunk GetChunk(int chunkx, int chunkz)
        {
            Vector2Int key = new Vector2Int(chunkx, chunkz);
            if (!dict.ContainsKey(key))
                return null;

            return dict[key];
        }

        public bool ChunkExists(int cx, int cz)
        {
            return dict.ContainsKey(new Vector2Int(cx, cz));
        }

        public void UpdateChunk(int x, int y, int z)
        {
            Vector3Int c = Util.ToChunkCoords(x, y, z);

            if (x % Settings.ChunkSize.x == 0)
                _UpdateChunk(c.x - 1, c.y, c.z);
            else if (x % Settings.ChunkSize.x == Settings.ChunkSize.x - 1)
                _UpdateChunk(c.x + 1, c.y, c.z);

            if (y % Settings.ChunkSize.y == 0 && c.y != 0)
                _UpdateChunk(c.x, c.y - 1, c.z);
            else if (y % Settings.ChunkSize.y == Settings.ChunkSize.y - 1 && c.y != Settings.ChunkSectionsPerChunk - 1)
                _UpdateChunk(c.x, c.y + 1, c.z);

            if (z % Settings.ChunkSize.z == 0)
                _UpdateChunk(c.x, c.y, c.z - 1);
            else if (z % Settings.ChunkSize.z == Settings.ChunkSize.z - 1)
                _UpdateChunk(c.x, c.y, c.z + 1);

            _UpdateChunk(c.x, c.y, c.z);
        }

        void _UpdateChunk(int cx, int cy, int cz)
        {
            var key = new Vector2Int(cx, cz);
            if (!dict.ContainsKey(key))
                return;

            ChunkSection section = dict[key].sections[cy];
            if (!chunksToUpdate.Contains(section))
                chunksToUpdate.Add(section);
        }

    }
}
