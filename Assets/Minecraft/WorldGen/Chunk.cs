using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Assets.Minecraft
{
    class Chunk
    {
        struct Layer
        {
            public int count;

            public bool IsSolid()
            {
                return count == Settings.ChunkSize.x * Settings.ChunkSize.z;
            }
            public bool IsEmpty()
            {
                return count == 0;
            }
        }

        public Vector2Int Pos { get; private set; }
        public bool Meshed { get; set; }
        public bool Generated { get; set; }
        public HeightMap HeightMap { get; private set; }

        public World world;
        
        Layer[] layers;
        BlockType[] blocks;

        public Chunk(Chunk other)
        {
            Pos = new Vector2Int(other.Pos.x, other.Pos.y);
            Meshed = other.Meshed;

            world = other.world;
            blocks = (BlockType[])other.blocks.Clone();
            layers = (Layer[])other.layers.Clone();
            HeightMap = new HeightMap(other.HeightMap);
        }
        public Chunk(Vector2Int pos, World _world)
        {
            Pos = pos;
            Meshed = false;

            world = _world;
            blocks = new BlockType[Settings.ChunkSize.x * Settings.ChunkSize.y * Settings.ChunkSize.z];
            layers = new Layer[Settings.ChunkSize.y];
            HeightMap = new HeightMap(world, pos);
        }

        public bool IsLayerSolid(int y)
        {
            if (y < 0 || y >= Settings.ChunkSize.y)
                return false;
            return layers[y].IsSolid();
        }
        public bool IsLayerEmpty(int y)
        {
            if (y < 0 || y >= Settings.ChunkSize.y)
                return false;
            return layers[y].IsEmpty();
        }

        public BlockType GetBlock(int relx, int rely, int relz)
        {
            if (relx < 0 || relx >= Settings.ChunkSize.x || relz < 0 || relz >= Settings.ChunkSize.z)
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + new Vector3Int(Settings.ChunkSize.x * Pos.x, Settings.ChunkSize.y, Settings.ChunkSize.z * Pos.y);
                return world.GetBlock(absPos.x, absPos.y, absPos.z);
            }
            if (rely < 0 || rely >= Settings.ChunkSize.y)
                return BlockType.Air;

            return blocks[Settings.toLin(relx, rely, relz)];
        }
        public void SetBlock(int relx, int rely, int relz, BlockType type)
        {
            if (relx < 0 || relx >= Settings.ChunkSize.x || relz < 0 || relz >= Settings.ChunkSize.z)
            {
                Vector3Int absPos = new Vector3Int(relx, rely, relz) + new Vector3Int(Settings.ChunkSize.x * Pos.x, Settings.ChunkSize.y, Settings.ChunkSize.z * Pos.y);
                world.SetBlock(absPos.x, absPos.y, absPos.z, type);
                return;
            }
            if (rely < 0 || rely >= Settings.ChunkSize.y)
                return;

            blocks[Settings.toLin(relx, rely, relz)] = type;
            OnSetBlock(relx, rely, relz, type);
        }

        void OnSetBlock(int relx, int rely, int relz, BlockType type)
        {
            layers[rely].count += BlockDictionary.Get(type).Opaque ? 1 : -1;

            if (type != BlockType.Air)
                HeightMap[relx, relz] = Math.Max(HeightMap[relx, relz], rely);
            else
                for (int y = rely; y >= 0; y--)
                    if (GetBlock(relx, y, relz) != BlockType.Air)
                    {
                        HeightMap[relx, relz] = y;
                        break;
                    }
        }
    }
}
