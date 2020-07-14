using Assets.Scripts.Minecraft.WorldManage;
using System;
using UnityEngine;

namespace Assets.Scripts.Minecraft.Player
{
    public class InventoryManager
    {
        static public void PickUp(GameObject player, BlockType type)
        {
            Debug.Log("Picked Up: " + type);
        }

        public static Inventory LoadInventory(string username)
        {
            return new Inventory();
        }
        public static void Input(Guid id, Inventory inv, byte mWheelInput)
        {
            if (mWheelInput != 0)
            {
                bool HeldBlockChanged = true;

                if (mWheelInput == 1 && inv.HeldBlock != BlockType.Count - 1)
                    inv.HeldBlock++;
                else if (mWheelInput == 2 && inv.HeldBlock != BlockType.Air + 1)
                    inv.HeldBlock--;
                else
                    HeldBlockChanged = false;

                if (HeldBlockChanged)
                    PacketSender.HeldItemChanged(id, inv.HeldBlock);
            }
        }
    }
}
