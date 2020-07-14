using Assets.Minecraft.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    class DroppedBlock : MonoBehaviour
    {
        public MeshFilter meshFilter;
        BlockType type;

        float originalY;

        void Update()
        {
            transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
            transform.position = new Vector3(transform.position.x, originalY + Mathf.Sin(Time.realtimeSinceStartup * 4) * 0.1f, transform.position.z); ;
        }
        public void Init(BlockType _type)
        {
            type = _type;
            meshFilter.mesh = new CubeMeshBuilder().Build(type);
            originalY = transform.position.y;
        }

        public void PickUp()
        {
            InventoryManager.PickUp(type);
            Destroy(gameObject);
        }
    }
}
