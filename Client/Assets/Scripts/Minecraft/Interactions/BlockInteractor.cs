using System;
using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    public class BlockInteractor : MonoBehaviour
    {
        public static BlockInteractor Get;

        public bool HasUpdated { get; private set; }

        public BlockType blockType;
        public Vector3Int hitPointPlus;
        public Vector3Int hitPointMinus;

        private void Awake()
        {
            if (Get == null)
                Get = this;
            else if (Get != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        void LateUpdate()
        {
            HasUpdated = false;

            Camera camera = Camera.main;
            if (!camera)
                return;

            Vector3 origin = camera.transform.position;
            Vector3 forward = camera.transform.forward;

            Vector2Int c = origin.ToChunkCoords();

            hitPointMinus = origin.ToIntVec();
            for (float dist = 0; dist < Settings.DigDistance; dist += 0.05f)
            {
                hitPointPlus = (origin + forward * dist).ToIntVec();
                blockType = World.Get.GetBlock(hitPointPlus.x, hitPointPlus.y, hitPointPlus.z);
                
                if (blockType != BlockType.Air)
                {
                    HasUpdated = true;
                    return;
                }

                hitPointMinus = hitPointPlus;
            }

            hitPointMinus = hitPointPlus = origin.ToIntVec();
        }
    }
}
