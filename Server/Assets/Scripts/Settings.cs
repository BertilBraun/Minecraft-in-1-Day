using UnityEngine;

namespace Assets.Scripts
{
    class Settings
    {
        public static bool beautifyOutput = true; // TODO Change to false to reduce Save sizes

        public static string chunkSaveFolder = "Saves/Chunks/";
        public static string dataBaseSave = "Saves/dataBase.dat";
        public static string gameSave = "Saves/game.dat";


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
