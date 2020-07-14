using Assets.Scripts.Minecraft.WorldManage;
using System.IO;
using UnityEngine;

namespace Assets.Minecraft
{
    public class Chunk
    {
        public bool Meshed = false; // TODO Remove
        public Vector2Int Pos { get; private set; }
        public bool Loaded { get; set; }
        public readonly ChunkSection[] sections;
        
        public Chunk(Vector2Int pos)
        {
            Pos = pos;
            Loaded = false;

            sections = new ChunkSection[Settings.ChunkSectionsPerChunk];
            //for (int i = 0; i < sections.Length; i++)
            //    sections[i] = new ChunkSection(new Vector3Int(Pos.x, i, Pos.y), this);
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
                World.Get.SetBlock(absPos.x, absPos.y, absPos.z, type); // TODO Should this call here?
                return;
            }
            if (rely < 0 || rely >= Settings.ChunkSize.y)
                return;

            GetSection(rely).SetBlock(relx, rely % Settings.ChunkSectionSize.y, relz, type);
        }

        public bool IsLayerSolid(int y)
        {
            if (y < 0 || y >= Settings.ChunkSize.y)
                return false;
            return GetSection(y).IsLayerSolid(y % Settings.ChunkSectionSize.y);
        }
        public bool IsLayerEmpty(int y)
        {
            if (y < 0 || y >= Settings.ChunkSize.y)
                return false;
            return GetSection(y).IsLayerEmpty(y % Settings.ChunkSectionSize.y);
        }

        bool OutsideBounds(int x, int z)
        {
            return x < 0 || x >= Settings.ChunkSize.x || z < 0 || z >= Settings.ChunkSize.z;
        }

        ChunkSection GetSection(int y)
        {
            if (y < 0 || y >= Settings.ChunkSize.y)
                return null;
            return sections[y / Settings.ChunkSectionSize.y];
        }

        public void ToFile(string path)
        {
            File.WriteAllText(path, "");
            foreach (var section in sections)
                section.ToFile(path);
        }

    }
}
