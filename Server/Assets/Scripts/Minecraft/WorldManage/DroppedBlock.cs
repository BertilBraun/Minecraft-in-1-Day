using Assets.Scripts.Minecraft.Player;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class DroppedBlock : MonoBehaviour
    {
        static int nextID = 1;

        public int ID = 0;
        public BlockType type;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                InventoryManager.PickUp(other.gameObject, type);
                Destroy(ID);
            }
        }

        DroppedBlock Init(BlockType _type)
        {
            ID = nextID++;
            type = _type;
            return this;
        }

        public static int Instantiate(BlockType type, Vector3 pos)
        {
            DroppedBlock block = Instantiate(GameManager.Get.droppedBlockPrefab, pos, Quaternion.identity).GetComponent<DroppedBlock>().Init(type);
            PacketSender.BlockDropped(block.ID, type, pos);
            GameManager.Get.DroppedBlocks.Add(block.ID, block);
            return block.ID;
        }

        public static bool Destroy(int id)
        {
            if (!GameManager.Get.DroppedBlocks.ContainsKey(id))
                return false;

            PacketSender.PlayerPickup(id);
            Destroy(GameManager.Get.DroppedBlocks[id].gameObject);
            GameManager.Get.DroppedBlocks.Remove(id);
            return true;
        }
    }
}
