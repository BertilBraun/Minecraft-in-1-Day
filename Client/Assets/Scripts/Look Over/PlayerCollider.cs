//using Assets.Minecraft;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets.Scripts
//{
//    class PlayerCollider
//    {
//        static World world = null;
//        static Transform transform = null;
//        public static void Init(World _world, Transform _transform)
//        {
//            world = _world;
//            transform = _transform;
//        }

//        public static bool Collision(out bool isGrounded)
//        {
//            isGrounded = false;

//            Vector3 dim = new Vector3(0.8f, 1.2f, 0.8f);
//            Vector3 min = transform.position - dim / 2;
//            Vector3 max = transform.position + dim / 2;

//            for (int x = (int)min.x; x <= (int)max.x; x++)
//                for (int y = (int)(min.y - 0.3f); y <= (int)max.y; y++)
//                    for (int z = (int)min.z; z <= (int)max.z; z++)
//                        if (BlockDictionary.Get(world.GetBlock(x, y, z)).Solid)
//                        {
//                            if (y == (int)(min.y - 0.3f))
//                                isGrounded = true;
//                            return true;
//                        }
//            return false;
//        }
//    }
//}
