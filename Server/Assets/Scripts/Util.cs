﻿using UnityEngine;

namespace Assets.Scripts
{
    public static class Util
    {
        public static int ToLin(int x, int y, int z)
        {
            return x + (Settings.ChunkSize.x * z) + (Settings.ChunkSize.x * Settings.ChunkSize.z * y);
        }
        public static Vector2Int ToChunkCoords(int x, int z)
        {
            return new Vector2Int(Mathf.FloorToInt(x / Settings.ChunkSize.x), Mathf.FloorToInt(z / Settings.ChunkSize.z));
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
