using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft
{
    enum BlockType : int
    {
        Air,
        Dirt,
        Grass,
        Stone,
        Wood,
        Leave
    }

    enum MeshType
    {
        Block,
        X,
    }

    enum MeshOrder
    {
        World,
        Fluid,
        Foliage
    }

    class Block
    {
        public BlockType Type { get; private set; }
        public MeshType Mesh { get; private set; }
        public MeshOrder Order { get; private set; }
        public Vector2[][] UVs { get; private set; }

        public bool Solid { get; private set; }
        public bool Opaque { get; private set; }


        public Block(BlockType _type, MeshType _meshType, MeshOrder _meshOrder, int AtlasIndex, bool _solid, bool _opaque) : 
            this(_type, _meshType, _meshOrder, AtlasIndex, AtlasIndex, AtlasIndex, _solid, _opaque)
        { }

        public Block(BlockType _type, MeshType _meshType, MeshOrder _meshOrder, int AtlasIndexUp, int AtlasIndexDown, int AtlasIndexSides, bool _solid, bool _opaque)
        {
            Type = _type;
            Mesh = _meshType;
            Order = _meshOrder;
            Solid = _solid;
            Opaque = _opaque;
            UVs = TextureAtlas.GenerateUVs(AtlasIndexUp, AtlasIndexDown, AtlasIndexSides);
        }
    }

    class BlockDictionary
    {
        static Dictionary<BlockType, Block> dict = null;

        static void Init()
        {
            dict = new Dictionary<BlockType, Block> {
                { BlockType.Air,    new Block(BlockType.Air, MeshType.Block, MeshOrder.World, 0, false, false) },
                { BlockType.Dirt,   new Block(BlockType.Dirt, MeshType.Block, MeshOrder.World, 2, true, true) },
                { BlockType.Grass,  new Block(BlockType.Grass, MeshType.Block, MeshOrder.World, 0, 2, 3, true, true) },
                { BlockType.Stone,  new Block(BlockType.Stone, MeshType.Block, MeshOrder.World, 1, true, true) },
                { BlockType.Wood,   new Block(BlockType.Wood, MeshType.Block, MeshOrder.World, 21, 21, 20, true, true) },
                { BlockType.Leave,  new Block(BlockType.Leave, MeshType.Block, MeshOrder.World, 53, true, true) }
            };
        }

        static public Block Get(BlockType type)
        {
            if (dict == null)
                Init();
            return dict[type];
        }
    }
}
