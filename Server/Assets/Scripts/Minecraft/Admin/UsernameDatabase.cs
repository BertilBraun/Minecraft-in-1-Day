using Assets.Scripts.Minecraft.Player;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Minecraft.Admin
{
    [Serializable]
    public class PlayerData
    {
        public Inventory Inventory = new Inventory(0);
        public Vector3 Pos = Vector3.zero;
        public Quaternion Rot = Quaternion.identity;

        public PlayerData()
        {
        }

        public PlayerData(Inventory inventory, Vector3 pos, Quaternion rot)
        {
            Inventory = inventory;
            Pos = pos;
            Rot = rot;
        }
    }

    public class UsernameDatabase
    {
        private static SerializableDictionary<string, PlayerData> database = new SerializableDictionary<string, PlayerData>();
        private static Dictionary<Guid, string> guidToName = new Dictionary<Guid, string>();

        public static void Load()
        {
            database = JsonUtility.FromJson<SerializableDictionary<string, PlayerData>>(File.ReadAllText(Settings.dataBaseSave));
        }
        public static void Save()
        {
            File.WriteAllText(Settings.dataBaseSave, JsonUtility.ToJson(database, Settings.beautifyOutput));
        }

        public static PlayerData Load(string name, Guid id)
        {
            guidToName[id] = name;
            if (database.ContainsKey(name))
                return database[name];

            database.Add(name, new PlayerData());
            return null;
        }
        public static void Save(string name, PlayerHandler player)
        {
            if (!database.ContainsKey(name))
                return;
            guidToName.Remove(player.id);
            database[name] = new PlayerData(player.inventory, player.transform.position, player.transform.rotation);
        }

        public static PlayerData Get(Guid id)
        {
            if (!guidToName.ContainsKey(id))
                return null;
            return database[guidToName[id]];
        }
    }
}
