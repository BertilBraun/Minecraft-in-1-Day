using Assets.Minecraft;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCollider
    {
        static Vector3 dim = new Vector3(0.8f, 1.2f, 0.8f);

        public static bool Collision(Transform transform, out bool isGrounded)
        {
            isGrounded = false;

            Vector3 min = transform.position - dim / 2;
            Vector3 max = transform.position + dim / 2;

            for (int x = (int)min.x; x <= (int)max.x; x++)
                for (int y = (int)(min.y - 0.3f); y <= (int)max.y; y++)
                    for (int z = (int)min.z; z <= (int)max.z; z++)
                        if (BlockDictionary.Get(World.Get.GetBlock(x, y, z)).Solid)
                        {
                            if (y == (int)(min.y - 0.3f))
                                isGrounded = true;
                            return true;
                        }
            return false;
        }
    }
}
