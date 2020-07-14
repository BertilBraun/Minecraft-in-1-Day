using Assets.Minecraft;
using Assets.Minecraft.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class InfoDisplay : MonoBehaviour
    {
        public Text infoText;
        public Player_Movement player;

        public BlockInteractor interactor;

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
                    "Player Position: " + player.transform.position.ToString() + "\n" +
                    "Player is Grounded: " + player.isGrounded + "\n" +
                    "Player is Flying: " + player.isFlying + "\n\n" +
                    "Block Looked at: " + interactedBlock + "\n" +
                    "Position of Block Looked at: " + rayCastHit.ToString();
            else
                infoText.text = "";

            rayCastHit = Vector3Int.zero;
            interactedBlock = BlockType.Air;
        }


        void OnEnable() => interactor.OnRayCast += OnRayCast;
        void OnDisable() => interactor.OnRayCast -= OnRayCast;
        void OnRayCast(Vector3Int _, Vector3Int point, BlockType block)
        {
            rayCastHit = _;
            interactedBlock = block;
        }
    }
}
