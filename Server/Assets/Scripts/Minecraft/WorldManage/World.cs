using System;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class World : MonoBehaviour
    {
        public static World Get;

        public ChunkManager manager;
        TerrainGenerator terrainGenerator;

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

        private void Start()
        {
            terrainGenerator = new TerrainGenerator();
        }

        public BlockType GetBlock(int x, int y, int z)
        {
            Vector2Int c = Util.ToChunkCoords(x, z);

            if (!manager.ChunkExists(c.x, c.y))
                return BlockType.Air;

            return GetChunk(c.x, c.y).GetBlock(x % Settings.ChunkSize.x, y, z % Settings.ChunkSize.z);
        }
        public void SetBlock(int x, int y, int z, BlockType type, bool changeHasChanged = false)
        {
            Vector2Int c = Util.ToChunkCoords(x, z);

            GetChunk(c.x, c.y).SetBlock(x % Settings.ChunkSize.x, y, z % Settings.ChunkSize.z, type, changeHasChanged);
        }
        public void Interact(int x, int y, int z, BlockType type)
        {
            SetBlock(x, y, z, type, true);
            manager.UpdateChunk(x, y, z, type);
        }

        public Vector3 GenerateSpawnPoint(Guid id, int cx = 100, int cz = 100)
        {
            PacketSender.ChunkSend(id, GenerateChunk(cx, cz));
            return new Vector3(7.5f + cx * Settings.ChunkSize.x, GetChunk(cx, cz).HeightMap[7, 7] + 3, 7.5f + cz * Settings.ChunkSize.z);
        }

        // TODO Optimize heavily

        public Chunk GenerateChunk(int x, int z)
        {
            Chunk c = GetChunk(x, z);

            if (c.Generated)
                return c;

            for (int ox = -1; ox <= 1; ox++)
                for (int oz = -1; oz <= 1; oz++)
                    terrainGenerator.GenerateTerrain(GetChunk(x + ox, z + oz));

            terrainGenerator.GenerateTerrain(c);
            c.Generated = true;
            return c;
        }

        public Chunk GetChunk(int x, int z)
        {
            return manager.GetChunk(x, z);
        }
    }
}
