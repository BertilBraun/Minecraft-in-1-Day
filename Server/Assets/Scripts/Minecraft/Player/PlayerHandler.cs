using Assets.Scripts.Minecraft.WorldManage;
using System;
using UnityEngine;

namespace Assets.Scripts.Minecraft.Player
{
    public class PlayerHandler : MonoBehaviour
    {
        public Guid id;
        public string username;

        public Inventory inventory;
        public PlayerMovement movement;

        public int loadDistance = 1;
        public int mWheelScroll;

        public void Initialize(Guid _id, string _username)
        {
            id = _id;
            username = _username;
            inventory = InventoryManager.Get.LoadInventory(this);
            movement.Init(id);
        }

        /// <summary>Updates the player input with newly received input.</summary>
        /// <param name="_inputs">The new key inputs.</param>
        /// <param name="_rotation">The new rotation.</param>
        public void SetInput(bool[] _inputs, Quaternion _rotation, int _mWheelScroll)
        {
            movement.SetInput(_inputs);
            transform.rotation = _rotation;
            mWheelScroll = _mWheelScroll;
        }

        // TODO Future, Do raycast on server instead of client side
        public void Interact(Vector3Int point, bool leftClick)
        {
            if ((transform.position - point).sqrMagnitude > Settings.DigDistance * Settings.DigDistance)
            {
                Debug.Log("To far away to Interact");
                return;
            }

            if (leftClick)
            {
                Vector3 pos = point + new Vector3(0.5f + UnityEngine.Random.Range(0, 0.1f), 0.15f, 0.5f + UnityEngine.Random.Range(0, 0.1f));
                DroppedBlock.Instantiate(World.Get.GetBlock(point.x, point.y, point.z), pos);
                World.Get.Interact(point.x, point.y, point.z, BlockType.Air);
            }
            else
            {
                World.Get.SetBlock(point.x, point.y, point.z, inventory.HeldBlock);
                if (PlayerCollider.AnyCollision())
                    World.Get.SetBlock(point.x, point.y, point.z, BlockType.Air);
                else
                    World.Get.Interact(point.x, point.y, point.z, inventory.HeldBlock);
            }
        }

    }
}