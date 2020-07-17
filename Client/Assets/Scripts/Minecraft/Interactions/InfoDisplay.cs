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
        public HeldItemDisplay heldItem = null;

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
                var localPlayer = GameManager.Get.localPlayer;
                var prediction = localPlayer.GetComponent<PlayerMovementPrediction>();

                infoText.text =
                    "FPS: " + (int)(1 / Time.deltaTime) * 2 + "\n" +
                    "Ping: " + GameManager.Get.ServerPing.TotalMilliseconds + "ms\n" +
                    "Player Position: " + localPlayer.transform.position.ToString() + "\n" +
                    "Player Rotation: " + localPlayer.transform.rotation.eulerAngles.ToString() + "\n" +
                    "Player is Flying: " + prediction.isFlying + "\n" +
                    "Player is Swimming: " + prediction.isSwimming + "\n" +
                    "Player is Grounded: " + prediction.isGrounded + "\n\n" +
                    "Held Block: " + heldItem.HeldBlock + "\n" +
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
