using System.Collections.Generic;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public enum BlockType : byte
    {
        Air,
        Dirt,
        Grass,
        Stone,
        Wood,
        Leave,
        Plank,
        Glass,
        Torch,
        Water,

        Count
    }

    public class BlockDictionary
    {
        static Dictionary<BlockType, Block> dict = null;

        static void Init()
        {
            dict = new Dictionary<BlockType, Block> {
                { BlockType.Air,    new Block(BlockType.Air, false) },
                { BlockType.Dirt,   new Block(BlockType.Dirt, true) },
                { BlockType.Grass,  new Block(BlockType.Grass, true) },
                { BlockType.Stone,  new Block(BlockType.Stone, true) },
                { BlockType.Wood,   new Block(BlockType.Wood, true) },
                { BlockType.Leave,  new Block(BlockType.Leave, true) },
                { BlockType.Plank,  new Block(BlockType.Plank, true) },
                { BlockType.Glass,  new Block(BlockType.Glass, true) },
                { BlockType.Torch,  new Block(BlockType.Torch, false) },
                { BlockType.Water,  new Block(BlockType.Water, false) },
            };
        }

        static public Block Get(BlockType type)
        {
            if (dict == null)
                Init();
            return dict[type];
        }
    }

    public class Block
    {
        public BlockType Type { get; private set; }
        public bool Solid { get; private set; }


        public Block(BlockType _type, bool _solid)
        {
            Type = _type;
            Solid = _solid;
        }
    }
}