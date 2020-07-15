using System;
using UnityEngine;

namespace Assets.Minecraft
{
    public class World : MonoBehaviour
    {
        public static World Get;

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

        public BlockType GetBlock(int x, int y, int z)
        {
            Vector2Int c = Util.ToChunkCoords(x, z);

            if (!ChunkManager.Get.ChunkExists(c.x, c.y))
                return BlockType.Air;

            return GetChunk(c.x, c.y).GetBlock(x % Settings.ChunkSize.x, y, z % Settings.ChunkSize.z);
        }
        public void SetBlock(int x, int y, int z, BlockType type)
        {
            Vector2Int c = Util.ToChunkCoords(x, z);

            if (!ChunkManager.Get.ChunkExists(c.x, c.y))
                return;

            GetChunk(c.x, c.y).SetBlock(x % Settings.ChunkSize.x, y, z % Settings.ChunkSize.z, type);
            ChunkManager.Get.UpdateChunk(x, y, z);
        }

        public Chunk GetChunk(int cx, int cz)
        {
            return ChunkManager.Get.GetChunk(cx, cz);
        }

        public bool IsLayerSolid(int cx, int y, int cz)
        {
            Chunk c = ChunkManager.Get.GetChunk(cx, cz);
            if (c == null)
                return false;
            return c.IsLayerSolid(y);
        }
    }
}
