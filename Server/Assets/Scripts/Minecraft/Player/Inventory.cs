using Assets.Scripts.Minecraft.WorldManage;
using System;
using UnityEngine;

namespace Assets.Scripts.Minecraft.Player
{
    [Serializable]
    public struct Slot
    {
        [SerializeField]
        public BlockType Item;
        [SerializeField]
        public byte count;
    }


    [Serializable]
    public class Inventory
    {
        public BlockType HeldBlock = BlockType.Dirt;
        Slot[] slots;

        public Inventory(int slots)
        {
            this.slots = new Slot[slots];
        }
    }
}
