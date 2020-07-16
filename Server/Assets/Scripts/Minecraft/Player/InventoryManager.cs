using Assets.Scripts.Minecraft.WorldManage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Minecraft.Player
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Get;

        Dictionary<PlayerHandler, Inventory> inventories = new Dictionary<PlayerHandler, Inventory>();

        private void Awake()
        {
            if (Get == null)
                Get = this;
            else if (Get != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        private void FixedUpdate()
        {
            foreach (var kv in inventories)
                Input(kv.Key, kv.Value);
        }

        public void PickUp(PlayerHandler player, BlockType type)
        {
            Debug.Log("Picked Up: " + type);
        }
        public void Input(PlayerHandler player, Inventory inv)
        {
            if (player.mWheelScroll != 0)
            {
                bool HeldBlockChanged = true;

                if (player.mWheelScroll > 0 && inv.HeldBlock != BlockType.Count - 1)
                    inv.HeldBlock++;
                else if (player.mWheelScroll < 0 && inv.HeldBlock != BlockType.Air + 1)
                    inv.HeldBlock--;
                else
                    HeldBlockChanged = false;

                if (HeldBlockChanged)
                    PacketSender.HeldItemChanged(player.id, inv.HeldBlock);
                player.mWheelScroll = 0;
            }
        }
        public Inventory LoadInventory(PlayerHandler player)
        {
            inventories.Add(player, new Inventory());
            return inventories[player];
        }
    }
}
