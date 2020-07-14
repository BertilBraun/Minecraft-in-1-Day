using System;
using UnityEngine;

namespace Assets.Minecraft
{
    static class Util
    {
        public static void MeasureTime(Action function, string funcName = "")
        {
            var s = Time.realtimeSinceStartup;
            function();
            Debug.Log("Time for function " + funcName + ": " + (Time.realtimeSinceStartup - s).ToString("f6"));

        }

        public static int ToLin(int x, int y, int z)
        {
            return x + (Settings.ChunkSectionSize.x * z) + (Settings.ChunkSectionSize.x * Settings.ChunkSectionSize.z * y);
        }
        public static Vector2Int ToChunkCoords(int x, int z)
        {
            return new Vector2Int(Mathf.FloorToInt(x / Settings.ChunkSize.x), Mathf.FloorToInt(z / Settings.ChunkSize.z));
        }
        public static Vector3Int ToChunkCoords(int x, int y, int z)
        {
            return new Vector3Int(Mathf.FloorToInt(x / Settings.ChunkSize.x), Mathf.FloorToInt(y / Settings.ChunkSize.y), Mathf.FloorToInt(z / Settings.ChunkSize.z));
        }
        public static Vector2Int ToChunkCoords(this Vector3 pos)
        {
            return new Vector2Int(Mathf.FloorToInt(pos.x / Settings.ChunkSize.x), Mathf.FloorToInt(pos.z / Settings.ChunkSize.z));
        }
        public static Vector3Int ToIntVec(this Vector3 pos)
        {
            return new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        }
        public static Vector2Int ToIntVec(this Vector2 pos)
        {
            return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        }
    }
}
