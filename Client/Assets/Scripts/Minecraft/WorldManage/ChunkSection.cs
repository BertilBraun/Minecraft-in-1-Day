using Assets.Minecraft;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class ChunkSection
    {
        public struct Layer
        {
            public int count;

            public void Update(BlockType type)
            {
                count += BlockDictionary.Get(type).Opaque ? 1 : -1;
            }
            public bool IsSolid()
            {
                return count == Settings.ChunkSize.x * Settings.ChunkSize.z;
            }
            public bool IsEmpty()
            {
                return count == 0;
            }
        }

        public bool Meshed { get; set; }

        public Vector3Int Pos;
        BlockType[] blocks;
        Layer[] layers;
        Chunk parent;

        public ChunkSection(Vector3Int _pos, Chunk _parent)
        {
            Pos = _pos;
            parent = _parent;
            blocks = new BlockType[Settings.ChunkSectionVolume];
            layers = new Layer[Settings.ChunkSectionSize.y];
        }

        public BlockType GetBlock(int relx, int rely, int relz)
        {
            if (OutsideBounds(relx, relz))
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + Settings.ChunkSectionSize * Pos;
                return World.Get.GetBlock(absPos.x, absPos.y, absPos.z);
            }
            if (rely < 0 || rely >= Settings.ChunkSectionSize.y)
                return parent.GetBlock(relx, rely * Settings.ChunkSectionSize.y * Pos.y, relz);

            return blocks[Util.ToLin(relx, rely, relz)];
        }
        public void SetBlock(int relx, int rely, int relz, BlockType type)
        {
            if (OutsideBounds(relx, relz))
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + Settings.ChunkSectionSize * Pos;
                World.Get.SetBlock(absPos.x, absPos.y, absPos.z, type);
                return;
            }
            if (rely < 0 || rely >= Settings.ChunkSectionSize.y)
            {
                parent.SetBlock(relx, rely * Settings.ChunkSectionSize.y * Pos.y, relz, type);
                return;
            }

            blocks[Util.ToLin(relx, rely, relz)] = type;
            layers[rely].Update(type);
        }

        public bool IsLayerSolid(int y)
        {
            if (y < 0 || y >= Settings.ChunkSectionSize.y)
                return parent.IsLayerSolid(y * Pos.y * Settings.ChunkSectionSize.y);
            return layers[y].IsSolid();
        }
        public bool IsLayerEmpty(int y)
        {
            if (y < 0 || y >= Settings.ChunkSectionSize.y)
                return parent.IsLayerEmpty(y * Pos.y * Settings.ChunkSectionSize.y);
            return layers[y].IsEmpty();
        }

        public void ToFile(string path)
        {
            string output = "";
            foreach (BlockType block in blocks)
                output += (char)block;
            File.AppendAllText(path, output);
        }

        public ChunkSection Copy()
        {
            ChunkSection other = (ChunkSection)MemberwiseClone();

            other.Meshed = Meshed;
            other.Pos = new Vector3Int(Pos.x, Pos.y, Pos.z);
            other.blocks = (BlockType[])blocks.Clone();
            other.layers = (Layer[])layers.Clone();
            other.parent = parent;
            return other;
        }

        bool OutsideBounds(int x, int z)
        {
            return x < 0 || x >= Settings.ChunkSectionSize.x || z < 0 || z >= Settings.ChunkSectionSize.z;
        }

        public static ChunkSection Decode(Chunk parent, Vector3Int pos, byte[] blockData, bool encoded)
        {
            ChunkSection c = new ChunkSection(pos, parent);

            try
            {
                if (!encoded)
                {
                    for (int i = 0; i < Settings.ChunkSectionVolume; i++)
                        c.blocks[i] = (BlockType)blockData[i];
                }
                else
                {
                    int index = 0;
                    int blockIndex = 0;

                    while (blockIndex < Settings.ChunkSectionVolume)
                    {
                        BlockType type = (BlockType)blockData[index++];
                        byte count = blockData[index++];

                        for (int i = 0; i < count; i++)
                            c.blocks[blockIndex++] = type;
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log("Error on Chunk Parse, Did not recieve all Data, Exception: " + e.Message);
                // TODO if ! assert, discard, request it again
                return null;
            }

            for (int y = 0; y < Settings.ChunkSectionSize.y; y++)
                for (int z = 0; z < Settings.ChunkSectionSize.z; z++)
                    for (int x = 0; x < Settings.ChunkSectionSize.x; x++)
                        c.layers[y].Update(c.blocks[Util.ToLin(x, y, z)]);

            return c;
        }

    }
}