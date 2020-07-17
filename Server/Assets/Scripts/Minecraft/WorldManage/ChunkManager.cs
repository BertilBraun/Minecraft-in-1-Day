using Assets.Scripts.Minecraft.Player;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class ChunkManager : MonoBehaviour
    {
        public static ChunkManager Get;

        Dictionary<Vector2Int, Chunk> loadedChunks;
        Dictionary<Guid, List<Vector2Int>> loadedChunksPerUser;

        void Start()
        {
            Get = this;
            loadedChunks = new Dictionary<Vector2Int, Chunk>();
            loadedChunksPerUser = new Dictionary<Guid, List<Vector2Int>>();
        }

        public void Update()
        {
            foreach (PlayerHandler player in GameManager.Get.Players.Values)
                AddChunks(player);

            RemoveChunks();
        }

        private void FixedUpdate()
        {
            foreach (var c in loadedChunks.Values)
                c.OnTick(Time.fixedDeltaTime);
        }

        private void OnApplicationQuit()
        {
            foreach (Chunk chunk in loadedChunks.Values)
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
                    {
                        Vector2Int pos = new Vector2Int(x, z);
                        if (!loadedChunksPerUser[player.id].Contains(pos))
                        {
                            Chunk c = GetChunk(x, z);
                            if (!c.Generated)
                            {
                                c = Chunk.Load(new Vector2Int(x, z));
                                if (c == null)
                                    c = World.Get.GenerateChunk(x, z);
                                else
                                    AddChunk(c);
                            }
                            PacketSender.ChunkSend(player.id, c);
                            loadedChunksPerUser[player.id].Add(pos);
                            return;
                        }
                    }

            player.loadDistance++;
            if (player.loadDistance >= Settings.RenderDistance)
                player.loadDistance = 2;
        }

        void RemoveChunks()
        {
            List<Vector2Int> toDestroy = new List<Vector2Int>();
            foreach (Vector2Int chunkPos in loadedChunks.Keys)
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
                foreach (var list in loadedChunksPerUser.Values)
                    list.Remove(key);

                loadedChunks[key].Save();
                loadedChunks.Remove(key);
            }
        }

        public void AddPlayer(PlayerHandler player)
        {
            loadedChunksPerUser[player.id] = new List<Vector2Int>();
        }
        public void RemovePlayer(PlayerHandler player)
        {
            loadedChunksPerUser.Remove(player.id);
        }
        public Chunk AddChunk(int x, int z)
        {
            var key = new Vector2Int(x, z);

            if (ChunkExists(x, z))
                return loadedChunks[key];

            Chunk chunk = new Chunk(key);

            loadedChunks.Add(key, chunk);
            return chunk;
        }

        public void AddChunk(Chunk c)
        {
            loadedChunks[c.Pos] = c;
        }

        public Chunk GetChunk(int chunkx, int chunkz)
        {
            return AddChunk(chunkx, chunkz);
        }

        public bool ChunkExists(int cx, int cz)
        {
            return loadedChunks.ContainsKey(new Vector2Int(cx, cz));
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