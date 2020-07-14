using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft
{
    class Direction
    {
        public static Vector3Int[] Offset = new Vector3Int[]
        {
            new Vector3Int( 0,  0,  1),
            new Vector3Int(-1,  0,  0),
            new Vector3Int( 0, -1,  0),
            new Vector3Int( 0,  0, -1),
            new Vector3Int( 1,  0,  0),
            new Vector3Int( 0,  1,  0),
        };

        public static int North = 0;
        public static int East  = 1;
        public static int Up    = 2;
        public static int South = 3;
        public static int West  = 4;
        public static int Down  = 5;

        public static Vector3[][] Vertices = new Vector3[6][]
        {
            new Vector3[4]
            {
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0)
            },
            new Vector3[4]
            {
                new Vector3(1, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1)
            },
            new Vector3[4]
            {
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0)
            },
            new Vector3[4]
            {
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1)
            },
            new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0)
            },
            new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1)
            },
        };
    }
}
