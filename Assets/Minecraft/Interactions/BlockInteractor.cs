using System;
using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class BlockInteractor : MonoBehaviour
    {
        public World world;
        public Transform cameraTransform;
        public LayerMask mask;

        public event Action<Vector3Int, Vector3Int, BlockType> OnRayCast;

        void Update()
        {
            if (OnRayCast == null)
                return;

            BlockPickerOutputData output = BlockPicker.GetBlockLookedAt(cameraTransform.position, cameraTransform.forward, world, Settings.DigDistance, mask);

            if (output != null)
                OnRayCast?.Invoke(output.hitPoint, output.pointBeforeHit, output.blockType);
        }
    }
}
