using System.Runtime.Serialization.Configuration;
using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class BlockPickerOutputData
    {
        public BlockType blockType;
        public Vector3Int hitPoint;
        public Vector3Int pointBeforeHit;

        public BlockPickerOutputData() : this(BlockType.Air, Vector3Int.zero, Vector3Int.zero)
        { }

        public BlockPickerOutputData(BlockType _blockType, Vector3Int _hitPoint, Vector3Int _pointBeforeHit)
        {
            blockType = _blockType;
            hitPoint = _hitPoint;
            pointBeforeHit = _pointBeforeHit;
        }
    }

    class BlockPicker
    {
        public static BlockPickerOutputData GetBlockLookedAt(Vector3 origin, Vector3 forward, World world, float maxDistance = 5, int LayerMask = 0)
        {
            Vector3Int pointBefore = origin.ToIntVec();
            for (float dist = 0; dist < maxDistance; dist += 0.01f)
            {
                Vector3Int point = (origin + forward * dist).ToIntVec();
                BlockType type = world.GetBlock(point.x, point.y, point.z);

                if (type != BlockType.Air)
                    return new BlockPickerOutputData(type, point, pointBefore);

                pointBefore = point;
            }
            return null;
            //if (Physics.Raycast(origin, forward, out RaycastHit hit, maxDistance, LayerMask))
            //{
            //    Debug.DrawRay(origin, forward * hit.distance, Color.red, 0.5f);
            //
            //    Vector3 point = origin + forward * (hit.distance + 0.01f);
            //    Vector3 pointBefore = origin + forward * (hit.distance - 0.01f);
            //    Vector3Int pointIn = new Vector3Int((int)point.x, (int)point.y, (int)point.z);
            //
            //    return new BlockPickerOutputData (
            //        world.GetBlock(pointIn.x, pointIn.y, pointIn.z),
            //        pointIn,
            //        new Vector3Int((int)pointBefore.x, (int)pointBefore.y, (int)pointBefore.z)
            //    );
            //}

            return null;
        }
    }
}
