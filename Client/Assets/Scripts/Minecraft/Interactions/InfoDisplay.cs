using Assets.Minecraft;
using Assets.Minecraft.Interactions;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class InfoDisplay : MonoBehaviour
    {
        public Text infoText = null;
        public HeldItemDisplay heldItem;

        BlockType interactedBlock;
        Vector3Int rayCastHit;

        bool show = true;

        private void Start()
        {
            infoText.color = Color.white;
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.I))
                show = !show;

            if (show)
            {
                var prediction = GameManager.Get.localPlayer.GetComponent<PlayerMovementPrediction>();

                infoText.text =
                    "FPS: " + (int)(1 / Time.deltaTime) * 2 + "\n" +
                    "Player Position: " + GameManager.Get.localPlayerTransform.position.ToString() + "\n" +
                    "Player is Flying: " + prediction.isFlying + "\n" +
                    "Player is Grounded: " + prediction.isGrounded + "\n" +
                    "Held Block: " + heldItem.HeldBlock + "\n\n" +
                    "Block Looked at: " + interactedBlock + "\n" +
                    "Position of Block Looked at: " + rayCastHit.ToString();
            }
            else
                infoText.text = "";

            rayCastHit = BlockInteractor.Get.hitPointPlus;
            interactedBlock = BlockInteractor.Get.blockType;
        }
    }
}
