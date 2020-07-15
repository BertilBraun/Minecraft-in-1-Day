using Assets.Scripts.Minecraft.Player;
using Assets.Scripts.Minecraft.WorldManage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Get;

        public GameObject droppedBlockPrefab;
        public GameObject playerPrefab;

        public Dictionary<int, DroppedBlock> DroppedBlocks = new Dictionary<int, DroppedBlock>();
        public Dictionary<Guid, PlayerHandler> Players = new Dictionary<Guid, PlayerHandler>();

        public GameManager()
        {
            Get = this;
        }

        public void AddPlayer(Guid id, PlayerHandler player)
        {
            // TODO Remove
            //Chunk c = new Chunk(Vector2Int.one);
            //c.GenForNow();
            //c.ToFile("../tempChunkoutputServer.dat");
            //Debug.Log("Send Chunk -> check file if output is correct!");
            //foreach (var section in c.sections)
            //    PacketSender.ChunkSend(id, section);

            // TODO only for testing purpose PacketSender.TestPacket(id);

            Players.Add(id, player);
            player.transform.position = World.Get.GenerateSpawnPoint(id);
            Debug.Log("Added Player at Position: " + player.transform.position);
        }

        public PlayerHandler InstantiatePlayer()
        {
            return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<PlayerHandler>();
        }
    }
}
