using Assets.Scripts.Minecraft.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class BlockEntityDictionary
    {
        static Dictionary<BlockType, Type> dict = null;

        static void Init()
        {
            dict = new Dictionary<BlockType, Type> {
                { BlockType.Furnace, typeof(Furnace) },
            };
        }

        static public Type Get(BlockType type)
        {
            if (dict == null)
                Init();
            if (!dict.ContainsKey(type))
                return null;
            return dict[type];
        }
    }

    [Serializable]
    public abstract class BlockEntity
    {
        public Vector3Int pos;

        public abstract void OnTick(float dt);
    }

    [Serializable]
    public class Furnace : BlockEntity
    {
        public Slot burnSlot;
        public Slot cookSlot;
        public Slot cookedSlot;

        public float timeLeftToCook = 2f;
        public float burntimeLeft = 3f;

        public int lastTickTime = 0;

        public Furnace()
        {
            lastTickTime = GameManager.Get.ServerTick;
        }

        public override void OnTick(float dt)
        {
            while (lastTickTime <= GameManager.Get.ServerTick)
            {
                Debug.Log("Cooking");
                timeLeftToCook -= dt;
                burntimeLeft -= dt;

                if (burntimeLeft <= 0)
                {
                    if (burnSlot.count > 0)
                        burnSlot.count--;
                    else
                        timeLeftToCook = 2f;
                }

                if (timeLeftToCook <= 0)
                {
                    cookSlot.count--;
                    cookedSlot.count++;
                    timeLeftToCook = 2f;
                    Debug.Log("Cooked");
                }
                lastTickTime++;
            }
            lastTickTime = GameManager.Get.ServerTick;
        }
    }
}
