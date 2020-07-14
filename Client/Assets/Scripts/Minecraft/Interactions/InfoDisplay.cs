using Assets.Minecraft;
using Assets.Minecraft.Interactions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class InfoDisplay : MonoBehaviour
    {
        public Text infoText = null;

        BlockType interactedBlock;
        Vector3Int rayCastHit;

        bool show = false;

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.I))
                show = !show;

            if (show)
                infoText.text =
                    "FPS: " + (int)(1 / Time.deltaTime) + "\n" +
                    "Player Position: " + GameManager.Get.localPlayerTransform.position.ToString() + "\n" +
                    "Player is Flying: " + GameManager.Get.localPlayer.GetComponent<PlayerInput>().isFlying + "\n\n" +
                    "Block Looked at: " + interactedBlock + "\n" +
                    "Position of Block Looked at: " + rayCastHit.ToString();
            else
                infoText.text = "";

            if (BlockInteractor.Get.HasUpdated)
            {
                rayCastHit = BlockInteractor.Get.HitPointPlus;
                interactedBlock = BlockInteractor.Get.BlockType;
            }
        }
    }
}
