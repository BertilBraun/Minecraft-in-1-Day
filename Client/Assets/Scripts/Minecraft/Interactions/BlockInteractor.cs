using System;
using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    public class BlockInteractor : MonoBehaviour
    {
        public static BlockInteractor Get;

        public bool HasUpdated { get; private set; }
        public BlockType BlockType
        {
            get
            {
                if (!HasUpdated)
                    throw new Exception("BlockInteractor has not updated Yet! Check for BlockInteractor.HasUpdated!");
                return blockType;
            }
        }
        public Vector3Int HitPointPlus
        {
            get
            {
                if (!HasUpdated)
                    throw new Exception("BlockInteractor has not updated Yet! Check for BlockInteractor.HasUpdated!");
                return hitPointPlus;
            }
        }
        public Vector3Int HitPointMinus
        {
            get
            {
                if (!HasUpdated)
                    throw new Exception("BlockInteractor has not updated Yet! Check for BlockInteractor.HasUpdated!");
                return hitPointMinus;
            }
        }

        BlockType blockType;
        Vector3Int hitPointPlus;
        Vector3Int hitPointMinus;

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
                Vector3Int point = hitPointPlus - new Vector3Int(c.x * Settings.ChunkSize.x, 0, c.y * Settings.ChunkSize.z);
                blockType = World.Get.GetBlock(point.x, point.y, point.z);

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
