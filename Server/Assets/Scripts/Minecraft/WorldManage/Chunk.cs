using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    [Serializable]
    public class Chunk
    {
        [SerializeField]
        public bool HasChanged;
        [SerializeField]
        public Vector2Int Pos;
        [SerializeField]
        public bool Generated;
        [NonSerialized]
        public HeightMap HeightMap;
        [SerializeField]
        public ChunkSection[] sections;

        public Chunk(Vector2Int pos)
        {
            Pos = pos;
            HeightMap = new HeightMap(pos);

            sections = new ChunkSection[Settings.ChunkSectionsPerChunk];
            for (int i = 0; i < sections.Length; i++)
                sections[i] = new ChunkSection(new Vector3Int(Pos.x, i, Pos.y), this);
        }

        public void OnTick(float dt)
        {
            foreach (var section in sections)
                section.OnTick(dt);
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
        public void SetBlock(int relx, int rely, int relz, BlockType type, bool changeHasChanged = false)
        {
            if (OutsideBounds(relx, relz))
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + new Vector3Int(Settings.ChunkSize.x * Pos.x, Settings.ChunkSize.y, Settings.ChunkSize.z * Pos.y);
                World.Get.SetBlock(absPos.x, absPos.y, absPos.z, type, changeHasChanged);
                return;
            }
            if (rely < 0 || rely >= Settings.ChunkSize.y)
                return;

            GetSection(rely).SetBlock(relx, rely % Settings.ChunkSectionSize.y, relz, type, changeHasChanged);
        }

        ChunkSection GetSection(int rely)
        {
            return sections[rely / Settings.ChunkSectionSize.y];
        }

        bool OutsideBounds(int x, int z)
        {
            return x < 0 || x >= Settings.ChunkSize.x || z < 0 || z >= Settings.ChunkSize.z;
        }

        public static Chunk Load(Vector2Int pos)
        {
            string path = Settings.chunkSaveFolder + pos.ToString() + ".chunk";
            if (!File.Exists(path))
                return null;

            Chunk c = JsonUtility.FromJson<Chunk>(File.ReadAllText(path));

            c.Generated = true;
            foreach (var section in c.sections)
                section.parent = c;

            c.HeightMap = new HeightMap(c.Pos);
            c.HeightMap.SetHeights(c);
            return c;
        }
        public void Save()
        {
            if (HasChanged)
                File.WriteAllText(Settings.chunkSaveFolder + Pos.ToString() + ".chunk", JsonUtility.ToJson(this, Settings.beautifyOutput));
        }

    }
}
