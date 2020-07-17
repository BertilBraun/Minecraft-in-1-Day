using Assets.Scripts;
using Assets.Scripts.Minecraft.WorldManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft
{
    public enum BlockType : byte
    {
        Air,
        Dirt,
        Grass,
        Sand,
        Stone,
        Wood,
        Leave,
        Plank,
        Glass,
        Torch,
        Water,
        Furnace,
        //Stair,

        Count
    }

    public class BlockDictionary
    {
        static Dictionary<BlockType, Block> dict = null;

        static void Init()
        {
            dict = new Dictionary<BlockType, Block> {
                { BlockType.Air,    new CodeMeshedBlock(BlockType.Air, MeshOrder.World, 0, false, false) },
                { BlockType.Dirt,   new CodeMeshedBlock(BlockType.Dirt, MeshOrder.World, 2, true, true) },
                { BlockType.Grass,  new CodeMeshedBlock(BlockType.Grass, MeshOrder.World, 0, 2, 3, true, true) },
                { BlockType.Sand,   new CodeMeshedBlock(BlockType.Sand, MeshOrder.World, 48, true, true) },
                { BlockType.Stone,  new CodeMeshedBlock(BlockType.Stone, MeshOrder.World, 1, true, true) },
                { BlockType.Wood,   new CodeMeshedBlock(BlockType.Wood, MeshOrder.World, 21, 21, 20, true, true) },
                { BlockType.Leave,  new CodeMeshedBlock(BlockType.Leave, MeshOrder.World, 53, true, true) },
                { BlockType.Plank,  new CodeMeshedBlock(BlockType.Plank, MeshOrder.World, 4, true, true) },
                { BlockType.Glass,  new CodeMeshedBlock(BlockType.Glass, MeshOrder.World, 49, true, false) },
                { BlockType.Torch,  new LoadedMeshBlock(BlockType.Torch, MeshOrder.World, false, false) },
                { BlockType.Water,  new CodeFluidMeshedBlock(BlockType.Water, 206, false, false) },
                { BlockType.Furnace,new CodeMeshedBlock(BlockType.Furnace, MeshOrder.World, 45, 45, 44, true, true) },
                //{ BlockType.Stair,  new LoadedMeshBlock(BlockType.Stair, MeshOrder.World, true, true) },
            };
        }

        static public Block Get(BlockType type)
        {
            if (dict == null)
                Init();
            return dict[type];
        }
    }

    public enum MeshOrder
    {
        World,
        Fluid,
        Foliage
    }

    public abstract class Block
    {
        public BlockType Type { get; private set; }
        public MeshOrder Order { get; private set; }

        public bool Solid { get; private set; }
        public bool Opaque { get; private set; }


        public Block(BlockType _type, MeshOrder _meshOrder, bool _solid, bool _opaque)
        {
            Type = _type;
            Order = _meshOrder;
            Solid = _solid;
            Opaque = _opaque;
        }

        public abstract void Generate(MeshBuilder activeBuilder, Vector3Int pos, ChunkSection c);
    }

}
