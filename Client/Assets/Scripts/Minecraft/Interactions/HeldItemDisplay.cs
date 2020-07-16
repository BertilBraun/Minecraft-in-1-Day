using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class HeldItemDisplay : MonoBehaviour
    {
        public MeshFilter meshFilter = null;
        public BlockType HeldBlock;

        public void ChangeHeldBlock(BlockType type)
        {
            meshFilter.mesh = new CubeMeshBuilder().Build(type);
            HeldBlock = type;
        }
    }
}
