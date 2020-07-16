using Assets.Minecraft;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Player
{
    public class WaterPostProcessing : MonoBehaviour
    {

        public Volume waterProfile;
        public Volume normalProfile;

        private void Update()
        {
            Camera camera = Camera.main;
            if (!camera)
                return;

            Vector3Int pos = camera.transform.position.ToIntVec();
            if (BlockDictionary.Get(World.Get.GetBlock(pos.x, pos.y, pos.z)).Order == MeshOrder.Fluid)
            {
                waterProfile.enabled = true;
                normalProfile.enabled = false;
            }
            else
            {
                waterProfile.enabled = false;
                normalProfile.enabled = true;
            }
        }
    }
}
