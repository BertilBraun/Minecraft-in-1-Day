using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class PlayerInteract : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                if (BlockInteractor.Get.HasUpdated)
                    PacketSender.PlayerInteract(BlockInteractor.Get.hitPointPlus, true);

            if (Input.GetMouseButtonDown(1))
                if (BlockInteractor.Get.HasUpdated)
                    PacketSender.PlayerInteract(BlockInteractor.Get.hitPointMinus, false);
        }
    }
}
