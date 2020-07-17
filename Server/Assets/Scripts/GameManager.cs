using Assets.Scripts.Minecraft.Admin;
using Assets.Scripts.Minecraft.Player;
using Assets.Scripts.Minecraft.WorldManage;
using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class GameData
    {
        public int ServerTick = 0;

        public static GameData Load()
        {
            return JsonUtility.FromJson<GameData>(File.ReadAllText(Settings.gameSave));
        }
        public static void Save(GameData data)
        {
            File.WriteAllText(Settings.gameSave, JsonUtility.ToJson(data, Settings.beautifyOutput));
        }
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Get;

        public int ServerTick = 0;

        public GameObject droppedBlockPrefab;
        public GameObject playerPrefab;

        public SerializableDictionary<int, DroppedBlock> DroppedBlocks = new SerializableDictionary<int, DroppedBlock>();
        public SerializableDictionary<Guid, PlayerHandler> Players = new SerializableDictionary<Guid, PlayerHandler>();

        public GameManager()
        {
            Get = this;
        }

        private void Awake()
        {
            if (Get != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
                return;
            }
            UsernameDatabase.Load();

            GameData data = GameData.Load();
            ServerTick = data.ServerTick;
        }
        private void FixedUpdate()
        {
            PacketSender.SendServerTick(ServerTick);
            ServerTick++;
        }

        private void OnApplicationQuit()
        {
            foreach (var player in Players.Values)
                UsernameDatabase.Save(player.username, player);
            UsernameDatabase.Save();

            GameData data = new GameData();
            data.ServerTick = ServerTick;
            GameData.Save(data);
        }

        public void AddPlayer(Guid id, PlayerHandler player)
        {
            Players.Add(id, player);
            ChunkManager.Get.AddPlayer(player);
            var data = UsernameDatabase.Load(player.username, id);
            if (data != null)
            {
                player.inventory = data.Inventory;
                player.transform.position = data.Pos + new Vector3(0, 0.5f, 0);
                player.transform.rotation = data.Rot;
            }
            else
                player.transform.position = World.Get.GenerateSpawnPoint(id);

            InventoryManager.Get.AddInventory(player);
            Debug.Log("Added Player at Position: " + player.transform.position);
        }
        public void RemovePlayer(Guid id)
        {
            PlayerHandler player = Players[id];
            UsernameDatabase.Save(player.username, player);
            ChunkManager.Get.RemovePlayer(player);

            Destroy(player.gameObject);
            Players.Remove(id);
        }

        public PlayerHandler InstantiatePlayer()
        {
            return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<PlayerHandler>();
        }
    }
}
