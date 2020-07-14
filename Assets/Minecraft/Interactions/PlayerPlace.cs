using Assets.Minecraft.Items;
using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class PlayerPlace : MonoBehaviour
    {
        public BlockInteractor interactor;
        public World world;

        void OnEnable()
        {
            interactor.OnRayCast += OnRayCast;
        }
        void OnDisable()
        {
            interactor.OnRayCast -= OnRayCast;
        }

        void OnRayCast(Vector3Int _, Vector3Int point, BlockType block)
        {
            if (Input.GetMouseButtonDown(1))
                world.SetBlock(point.x, point.y, point.z, InventoryManager.HeldBlock);
        }
    }
}
