using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class ChunkSection
    {
        public bool HasChanged { get; private set; }

        Vector3Int pos;
        BlockType[] blocks;
        Chunk parent;

        public ChunkSection(Vector3Int _pos, Chunk _parent)
        {
            pos = _pos;
            parent = _parent;
            blocks = new BlockType[Settings.ChunkSectionVolume];
        }

        public BlockType GetBlock(int relx, int rely, int relz)
        {
            if (OutsideBounds(relx, relz))
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + Settings.ChunkSectionSize * pos;
                return World.Get.GetBlock(absPos.x, absPos.y, absPos.z);
            }
            if (rely < 0 || rely >= Settings.ChunkSectionSize.y)
                return parent.GetBlock(relx, rely * Settings.ChunkSectionSize.y * pos.y, relz);

            return blocks[Util.ToLin(relx, rely, relz)];
        }
        public void SetBlock(int relx, int rely, int relz, BlockType type, bool changeHasChanged = false)
        {
            if (OutsideBounds(relx, relz))
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + Settings.ChunkSectionSize * pos;
                World.Get.SetBlock(absPos.x, absPos.y, absPos.z, type);
                return;
            }
            if (rely < 0 || rely >= Settings.ChunkSectionSize.y)
            {
                parent.SetBlock(relx, rely * Settings.ChunkSectionSize.y * pos.y, relz, type);
                return;
            }

            blocks[Util.ToLin(relx, rely, relz)] = type;
            if (changeHasChanged)
                HasChanged = true;
        }

        bool OutsideBounds(int x, int z)
        {
            return x < 0 || x >= Settings.ChunkSectionSize.x || z < 0 || z >= Settings.ChunkSectionSize.z;
        }

        public void ToFile(string path)
        {
            string output = "";
            foreach (BlockType block in blocks)
                output += (char)block;
            File.AppendAllText(path, output);
        }

        public byte[] Encode()
        {
            List<byte> blockData = new List<byte>();

            BlockType current = GetBlock(0, 0, 0);
            byte count = 0;

            for (int i = 0; i < Settings.ChunkSectionVolume; i++)
            {
                if (blockData.Count >= Settings.ChunkSectionVolume)
                    break;

                BlockType blockAt = blocks[i];
                if (current != blockAt || count == byte.MaxValue)
                {
                    blockData.Add((byte)current);
                    blockData.Add(count);
                    count = 0;
                    current = blockAt;
                }
                count++;
            }

            blockData.Add((byte)current);
            blockData.Add(count);

            List<byte> data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(pos.x));
            data.AddRange(BitConverter.GetBytes(pos.y));
            data.AddRange(BitConverter.GetBytes(pos.z));

            if (blockData.Count >= Settings.ChunkSectionVolume)
            {
                data.Add(0);
                data.AddRange(BitConverter.GetBytes(blocks.Length));
                foreach (var block in blocks)
                    data.Add((byte)block);
            }
            else
            {
                data.Add(1);
                data.AddRange(BitConverter.GetBytes(blockData.Count));
                data.AddRange(blockData);
            }

            return data.ToArray();
        }
    }
}
