using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class PickUpCollider : MonoBehaviour
    {
        public DroppedBlock droppedBlock;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                droppedBlock.PickUp();
        }

    }
}
