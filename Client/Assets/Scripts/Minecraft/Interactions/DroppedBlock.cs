using UnityEngine;

namespace Assets.Minecraft.Interactions
{
    public class DroppedBlock : MonoBehaviour
    {
        public MeshFilter meshFilter;
        BlockType type;

        float originalY;

        void Update()
        {
            transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
            transform.position = new Vector3(transform.position.x, originalY + Mathf.Sin(Time.realtimeSinceStartup * 4) * 0.1f, transform.position.z); ;
        }

        public void Init(BlockType _type, Vector3 pos)
        {
            type = _type;
            meshFilter.mesh = new CubeMeshBuilder().Build(type);
            transform.position = pos;
            originalY = transform.position.y;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
