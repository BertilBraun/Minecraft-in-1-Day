using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class PlayerInteract : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                if (BlockInteractor.Get.HasUpdated)
                    PacketSender.PlayerInteract(BlockInteractor.Get.HitPointPlus, true);

            if (Input.GetMouseButtonDown(1))
                if (BlockInteractor.Get.HasUpdated)
                    PacketSender.PlayerInteract(BlockInteractor.Get.HitPointMinus, false);

            if (Input.GetMouseButtonDown(0))
                PacketSender.PlayerInteract(transform.position + Vector3.one * 2, true);
            if (Input.GetMouseButtonDown(1))
                PacketSender.PlayerInteract(transform.position + Vector3.one * 2, false);
        }
    }
}
