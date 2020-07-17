using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    [Serializable]
    public class BlockEntityManager : ISerializationCallbackReceiver
    {
        [Serializable]
        public class SerializedBlockEntity
        {
            [SerializeField]
            public string type;
            [SerializeField]
            public string data;

            public static SerializedBlockEntity Serialize(object obj)
            {
                return new SerializedBlockEntity()
                {
                    type = obj.GetType().ToString(),
                    data = JsonUtility.ToJson(obj)
                };
            }
            public static BlockEntity Deserialize(SerializedBlockEntity sd)
            {
                return (BlockEntity)JsonUtility.FromJson(sd.data, Type.GetType(sd.type));
            }
        }

        List<BlockEntity> blockEntities = new List<BlockEntity>();

        [SerializeField]
        public List<SerializedBlockEntity> SerializedBlockEntities;

        public void OnTick(float dt)
        {
            foreach (var entity in blockEntities)
                entity.OnTick(dt);
        }

        public void AddIfBlockEntity(int relx, int rely, int relz, BlockType type)
        {
            Type entityType = BlockEntityDictionary.Get(type);
            if (entityType == null)
                return;

            foreach (var e in blockEntities)
                if (e.pos.x == relx && e.pos.y == rely && e.pos.z == relz)
                    return;

            BlockEntity entity = (BlockEntity)Activator.CreateInstance(entityType);
            entity.pos = new Vector3Int(relx, rely, relz);
            blockEntities.Add(entity);
        }
        public void RemoveIfBlockEntity(int relx, int rely, int relz, BlockType blockAtPos)
        {
            if (BlockEntityDictionary.Get(blockAtPos) == null)
                return;

            foreach (var entity in blockEntities)
                if (entity.pos.x == relx && entity.pos.y == rely && entity.pos.z == relz)
                {
                    blockEntities.Remove(entity);
                    break;
                }
        }

        public void OnBeforeSerialize()
        {
            SerializedBlockEntities = new List<SerializedBlockEntity>();

            foreach (var block in blockEntities)
                SerializedBlockEntities.Add(SerializedBlockEntity.Serialize(block));
        }
        public void OnAfterDeserialize()
        {
            blockEntities = new List<BlockEntity>();
            foreach (var serialized in SerializedBlockEntities)
                blockEntities.Add(SerializedBlockEntity.Deserialize(serialized));
        }
    }
}
