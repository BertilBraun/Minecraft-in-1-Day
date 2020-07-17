using System;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class HeightMap
    {
        int[] Map;
        Vector2Int pos;

        public HeightMap(Vector2Int _pos)
        {
            pos = _pos;
            Map = new int[Settings.ChunkSize.x * Settings.ChunkSize.z];
        }

        public HeightMap(HeightMap other)
        {
            pos = new Vector2Int(other.pos.x, other.pos.y);
            Map = (int[])other.Map.Clone();
        }

        public void Generate()
        {
            for (int z = 0; z < Settings.ChunkSize.z; z++)
                for (int x = 0; x < Settings.ChunkSize.x; x++)
                {
                    float val = Sample(100f, x, z) * 3f + Sample(30f, x, z) * 3f + Sample(15f, x, z) + Sample(1f, x, z);

                    val *= 10f;
                    val += 40f;

                    Map[x + (Settings.ChunkSize.x * z)] = (int)val;
                }
        }

        public void SetHeights(Chunk c)
        {
            Map = new int[Settings.ChunkSize.x * Settings.ChunkSize.z];

            for (int z = 0; z < Settings.ChunkSize.z; z++)
                for (int x = 0; x < Settings.ChunkSize.x; x++)
                    for (int y = Settings.ChunkSize.y - 1; y >= 0; y--)
                        if (c.GetBlock(x, y, z) != BlockType.Air)
                        {
                            Map[x + (Settings.ChunkSize.x * z)] = y;
                            break;
                        }
        }

        float Sample(float div, int x, int z)
        {
            return Mathf.PerlinNoise(
                        (pos.x * Settings.ChunkSize.x + x) / div,
                        (pos.y * Settings.ChunkSize.z + z) / div);
        }

        public int this[int x, int z]
        {
            get
            {
                if (x < 0)
                    return World.Get.GetChunk(pos.x - 1, pos.y).HeightMap[x + Settings.ChunkSize.x, z];
                if (x >= Settings.ChunkSize.x)
                    return World.Get.GetChunk(pos.x + 1, pos.y).HeightMap[x - Settings.ChunkSize.x, z];
                if (z < 0)
                    return World.Get.GetChunk(pos.x, pos.y - 1).HeightMap[x, z + Settings.ChunkSize.z];
                if (z >= Settings.ChunkSize.z)
                    return World.Get.GetChunk(pos.x, pos.y + 1).HeightMap[x, z - Settings.ChunkSize.z];
                return Map[x + (Settings.ChunkSize.x * z)];
            }
            set
            {
                if (x < 0)
                    World.Get.GetChunk(pos.x - 1, pos.y).HeightMap[x + Settings.ChunkSize.x, z] = value;
                else if (x >= Settings.ChunkSize.x)
                    World.Get.GetChunk(pos.x + 1, pos.y).HeightMap[x - Settings.ChunkSize.x, z] = value;
                else if (z < 0)
                    World.Get.GetChunk(pos.x, pos.y - 1).HeightMap[x, z + Settings.ChunkSize.z] = value;
                else if (z >= Settings.ChunkSize.z)
                    World.Get.GetChunk(pos.x, pos.y + 1).HeightMap[x, z - Settings.ChunkSize.z] = value;
                else
                    Map[x + (Settings.ChunkSize.x * z)] = value;
            }
        }
    }
}
