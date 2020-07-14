using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class PlayerDig : MonoBehaviour
    {
        public GameObject droppedBlock;
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

        void OnRayCast(Vector3Int point, Vector3Int _, BlockType block)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = point + new Vector3(0.5f + Random.Range(0, 0.1f), 0.15f, 0.5f + Random.Range(0, 0.1f));
                DroppedBlock dropped = Instantiate(droppedBlock, pos, Quaternion.identity).GetComponent<DroppedBlock>();
                dropped.Init(block);

                world.SetBlock(point.x, point.y, point.z, BlockType.Air);
            }
        }
    }
}
