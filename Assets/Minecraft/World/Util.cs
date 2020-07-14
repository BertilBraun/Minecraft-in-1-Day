using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft
{
    static class Util
    {
        public static void MeasureTime(Action function)
        {
            var s = Time.realtimeSinceStartup;
            function();
            Debug.Log("Time for function: " + (Time.realtimeSinceStartup - s).ToString("f6"));

        }

        public static Vector2Int ToChunkCoords(this Vector3 pos)
        {
            return new Vector2Int(Mathf.FloorToInt(pos.x / Settings.ChunkSize.x), Mathf.FloorToInt(pos.z / Settings.ChunkSize.z));
        }
        public static Vector3Int ToIntVec(this Vector3 pos)
        {
            return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
        }
        public static Vector2Int ToIntVec(this Vector2 pos)
        {
            return new Vector2Int((int)pos.x, (int)pos.y);
        }
    }
}
