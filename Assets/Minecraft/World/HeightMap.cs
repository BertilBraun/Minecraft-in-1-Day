using System;
using UnityEngine;

namespace Assets.Minecraft
{
    class HeightMap
    {
        int[] Map;
        World world;
        Vector2Int pos;

        public HeightMap(World _world, Vector2Int _pos)
        {
            world = _world;
            pos = _pos;
            Map = new int[Settings.ChunkSize.x * Settings.ChunkSize.z];
        }

        public void Generate()
        {
            for (int z = 0; z < Settings.ChunkSize.z; z++)
                for (int x = 0; x < Settings.ChunkSize.x; x++)
                {
                    int idx = x + (Settings.ChunkSize.x * z);

                    float val = Mathf.PerlinNoise(
                        (pos.x * Settings.ChunkSize.x + x) / 100f, 
                        (pos.y * Settings.ChunkSize.z + z) / 100f) 
                        + Mathf.PerlinNoise(
                        (pos.x * Settings.ChunkSize.x + x) / 20f,
                        (pos.y * Settings.ChunkSize.z + z) / 20f) * 0.5f;

                    Map[idx] = Math.Max((int)(val * 100), Map[idx]);
                }
        }

        public int this[int x, int z]
        {
            get
            {
                if (x < 0)
                    return world.GetChunk(pos.x - 1, pos.y).HeightMap[x + Settings.ChunkSize.x, z];
                if (x >= Settings.ChunkSize.x)
                    return world.GetChunk(pos.x + 1, pos.y).HeightMap[x - Settings.ChunkSize.x, z];
                if (z < 0)
                    return world.GetChunk(pos.x, pos.y - 1).HeightMap[x, z + Settings.ChunkSize.z];
                if (z >= Settings.ChunkSize.z)
                    return world.GetChunk(pos.x, pos.y + 1).HeightMap[x, z - Settings.ChunkSize.z];
                return Map[x + (Settings.ChunkSize.x * z)];
            }
            set
            {
                if (x < 0)
                    world.GetChunk(pos.x - 1, pos.y).HeightMap[x + Settings.ChunkSize.x, z] = value;
                else if (x >= Settings.ChunkSize.x)
                    world.GetChunk(pos.x + 1, pos.y).HeightMap[x - Settings.ChunkSize.x, z] = value;
                else if (z < 0)
                    world.GetChunk(pos.x, pos.y - 1).HeightMap[x, z + Settings.ChunkSize.z] = value;
                else if (z >= Settings.ChunkSize.z)
                    world.GetChunk(pos.x, pos.y + 1).HeightMap[x, z - Settings.ChunkSize.z] = value;
                else 
                    Map[x + (Settings.ChunkSize.x * z)] = value;
            }
        }
    }
}
