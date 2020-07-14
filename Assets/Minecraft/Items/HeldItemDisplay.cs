using UnityEngine;

namespace Assets.Minecraft.Items
{
    class HeldItemDisplay : MonoBehaviour
    {
        public MeshFilter meshFilter;

        void Update()
        {
            if (InventoryManager.HeldBlockChanged)
                meshFilter.mesh = new CubeMeshBuilder().Build(InventoryManager.HeldBlock);
        }
    }
}
