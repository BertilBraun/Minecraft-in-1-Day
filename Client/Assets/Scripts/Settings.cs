﻿using UnityEngine;

namespace Assets
{
    class Settings
    {
        public static float TexturesPerRow = 16;
        public static float BlockTextureSize = 16;
        public static float TextureSize = BlockTextureSize * TexturesPerRow;

        // Of a Square surrounding the player with the player in the middle, the RenderDistance is the length of that square
        public static float RenderDistance = 9;

        public static float DigDistance = 10;

        public static Vector3Int ChunkSize = new Vector3Int(16, 256, 16);
        public static Vector3Int ChunkSectionSize = new Vector3Int(16, 16, 16);
        public static int ChunkVolume = ChunkSize.x * ChunkSize.y * ChunkSize.z;
        public static int ChunkSectionVolume = ChunkSectionSize.x * ChunkSectionSize.y * ChunkSectionSize.z;

        public static int ChunkSectionsPerChunk = 16;
    }
}
