using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class HeldItemDisplay : MonoBehaviour
    {
        public MeshFilter meshFilter = null;

        public void ChangeHeldBlock(BlockType type)
        {
            meshFilter.mesh = new CubeMeshBuilder().Build(type);
        }
    }
}
