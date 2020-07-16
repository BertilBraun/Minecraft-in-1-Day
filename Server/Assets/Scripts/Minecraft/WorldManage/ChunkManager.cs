using Assets.Scripts.Minecraft.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class ChunkManager : MonoBehaviour
    {
        Dictionary<Vector2Int, Chunk> dict;

        void Start()
        {
            dict = new Dictionary<Vector2Int, Chunk>();
        }

        public void Update()
        {
            foreach (PlayerHandler player in GameManager.Get.Players.Values)
                AddChunks(player);

            RemoveChunks();
        }

        private void OnApplicationQuit()
        {
            foreach (Chunk chunk in dict.Values)
                chunk.Save();
        }

        bool PlayerInRangeOfChunk(Vector3 playerPos, int cx, int cz)
        {
            Vector2Int renderDist = new Vector2Int((int)Settings.RenderDistance, (int)Settings.RenderDistance);

            Vector2Int min = playerPos.ToChunkCoords() - renderDist;
            Vector2Int max = playerPos.ToChunkCoords() + renderDist;

            return !(min.x > cx || min.y > cz || max.y < cz || max.x < cx);
        }

        void AddChunks(PlayerHandler player)
        {
            Vector2Int cam = player.transform.position.ToChunkCoords();

            for (int i = 0; i < player.loadDistance; i++)
                for (int x = cam.x - i; x <= cam.x + i; x++)
                    for (int z = cam.y - i; z <= cam.y + i; z++)
                        if (!GetChunk(x, z).Generated)
                        {
                            Chunk c = Chunk.Load(new Vector2Int(x, z));
                            if (c == null)
                                c = World.Get.GenerateChunk(x, z);
                            else
                                AddChunk(c);
                            PacketSender.ChunkSend(player.id, c);
                            return;
                        }

            player.loadDistance++;
            if (player.loadDistance >= Settings.RenderDistance)
                player.loadDistance = 2;
        }

        void RemoveChunks()
        {
            List<Vector2Int> toDestroy = new List<Vector2Int>();
            foreach (Vector2Int chunkPos in dict.Keys)
            {
                bool delete = true;
                foreach (PlayerHandler player in GameManager.Get.Players.Values)
                    if (PlayerInRangeOfChunk(player.transform.position, chunkPos.x, chunkPos.y))
                    {
                        delete = false;
                        break;
                    }
                if (delete)
                    toDestroy.Add(chunkPos);
            }

            foreach (Vector2Int key in toDestroy)
            {
                dict[key].Save();
                dict.Remove(key);
            }
        }

        public Chunk AddChunk(int x, int z)
        {
            var key = new Vector2Int(x, z);

            if (ChunkExists(x, z))
                return dict[key];

            Chunk chunk = new Chunk(key);

            dict.Add(key, chunk);
            return chunk;
        }

        public void AddChunk(Chunk c)
        {
            dict[c.Pos] = c;
        }

        public Chunk GetChunk(int chunkx, int chunkz)
        {
            return AddChunk(chunkx, chunkz);
        }

        public bool ChunkExists(int cx, int cz)
        {
            return dict.ContainsKey(new Vector2Int(cx, cz));
        }

        public void UpdateChunk(int x, int y, int z, BlockType type)
        {
            Vector3 pos = new Vector3(x, y, z);
            Vector2Int c = Util.ToChunkCoords(x, z);

            foreach (PlayerHandler player in GameManager.Get.Players.Values)
                if (PlayerInRangeOfChunk(player.transform.position, c.x, c.y))
                    PacketSender.ChunkUpdate(player.id, pos, type);
        }
    }
}

// OLD


//Dictionary<PlayerHandler, List<Chunk>> sendingChunks;
//Dictionary<PlayerHandler, List<int>> sendingIndex;

//// START
//sendingChunks = new Dictionary<PlayerHandler, List<Chunk>>();
//sendingIndex = new Dictionary<PlayerHandler, List<int>>();


//// UPDATE
//foreach (PlayerHandler player in GameManager.Get.Players.Values)
//    if (!sendingChunks.ContainsKey(player))
//    {
//        sendingChunks.Add(player, new List<Chunk>());
//        sendingIndex.Add(player, new List<int>());
//    }

//foreach (PlayerHandler player in GameManager.Get.Players.Values)
//    SendChunks(player);



//void SendChunks(PlayerHandler player)
//{
//    var chunks = sendingChunks[player];
//    var indices = sendingIndex[player];

//    if (chunks.Count == 0 || indices.Count == 0)
//        return;

//    Chunk c = chunks[0];
//    if (PlayerInRangeOfChunk(player.transform.position, c.Pos.x, c.Pos.y))
//    {
//        PacketSender.ChunkSend(player.id, c.sections[indices[0]++]);

//        if (indices[0] >= 16)
//        {
//            chunks.RemoveAt(0);
//            indices.RemoveAt(0);
//        }
//    }
//    else
//    {
//        chunks.RemoveAt(0);
//        indices.RemoveAt(0);
//    }
//}

// ADD CHUNKs

//sendingChunks[player].Add(World.Get.GenerateChunk(x, z));
//sendingIndex[player].Add(0);