using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class Chunk
    {
        public Vector2Int Pos { get; private set; }
        public bool Generated { get; set; }
        public HeightMap HeightMap { get; private set; }
        public readonly ChunkSection[] sections;

        public Chunk(Vector2Int pos)
        {
            Pos = pos;
            HeightMap = new HeightMap(pos);

            sections = new ChunkSection[Settings.ChunkSectionsPerChunk];
            for (int i = 0; i < sections.Length; i++)
                sections[i] = new ChunkSection(new Vector3Int(Pos.x, i, Pos.y), this);
        }

        public BlockType GetBlock(int relx, int rely, int relz)
        {
            if (OutsideBounds(relx, relz))
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + new Vector3Int(Settings.ChunkSize.x * Pos.x, Settings.ChunkSize.y, Settings.ChunkSize.z * Pos.y);
                return World.Get.GetBlock(absPos.x, absPos.y, absPos.z);
            }
            if (rely < 0 || rely >= Settings.ChunkSize.y)
                return BlockType.Air;

            return GetSection(rely).GetBlock(relx, rely % Settings.ChunkSectionSize.y, relz);
        }
        public void SetBlock(int relx, int rely, int relz, BlockType type)
        {
            if (OutsideBounds(relx, relz))
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + new Vector3Int(Settings.ChunkSize.x * Pos.x, Settings.ChunkSize.y, Settings.ChunkSize.z * Pos.y);
                World.Get.SetBlock(absPos.x, absPos.y, absPos.z, type);
                return;
            }
            if (rely < 0 || rely >= Settings.ChunkSize.y)
                return;

            GetSection(rely).SetBlock(relx, rely % Settings.ChunkSectionSize.y, relz, type);
        }

        ChunkSection GetSection(int rely)
        {
            return sections[rely / Settings.ChunkSectionSize.y];
        }

        bool OutsideBounds(int x, int z)
        {
            return x < 0 || x >= Settings.ChunkSize.x || z < 0 || z >= Settings.ChunkSize.z;
        }

        // TODO
        public void GenForNow()
        {
            for (int z = 0; z < Settings.ChunkSize.z; z++)
                for (int x = 0; x < Settings.ChunkSize.x; x++)
                    for (int y = 0; y < Settings.ChunkSize.y; y++)
                        if (Util.ToLin(x, y, z) % 2 == 0)
                            SetBlock(x, y, z, BlockType.Dirt);
                        else
                            SetBlock(x, y, z, BlockType.Air);

        }

        public void ToFile(string path)
        {
            File.WriteAllText(path, "");
            foreach (var section in sections)
                section.ToFile(path);
        }

    }
}
