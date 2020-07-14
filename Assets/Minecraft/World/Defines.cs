using UnityEngine;

namespace Assets.Minecraft
{
    class Settings
    {
        public static int toLin(int x, int y, int z)
        {
            return x + (ChunkSize.x * z) + (ChunkSize.x * ChunkSize.z * y);
        }

        public static float TexturesPerRow = 16;
        public static float BlockTextureSize = 16;
        public static float TextureSize = BlockTextureSize * TexturesPerRow;

        public static float RenderDistance = 8;

        public static float DigDistance = 5;

        public static Vector3Int ChunkSize = new Vector3Int(16, 256, 16);
    }
}
