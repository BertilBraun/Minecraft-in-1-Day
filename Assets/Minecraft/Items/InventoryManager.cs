using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft.Items
{
    class InventoryManager : MonoBehaviour
    {
        public static bool HeldBlockChanged { get; private set; }
        public static BlockType HeldBlock { get; private set; }

        void Start()
        {
        }

        void Update()
        {
            if (HeldBlock == BlockType.Air)
            {
                HeldBlockChanged = true;
                HeldBlock = BlockType.Grass;
            }
            else
                HeldBlockChanged = false;
        }

        static public void PickUp(BlockType type)
        {
            Debug.Log("Picked Up: " + type);
        }
    }
}
