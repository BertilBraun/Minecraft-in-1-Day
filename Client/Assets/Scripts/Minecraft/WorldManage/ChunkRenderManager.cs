using Assets.Minecraft;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class ChunkRenderManager : MonoBehaviour
    {
        public static ChunkRenderManager Get;
        public Transform worldParent;
        public GameObject chunkPrefab;

        WorldLoader loader;
        Dictionary<Vector3Int, ChunkRenderer> chunkRenderer = new Dictionary<Vector3Int, ChunkRenderer>();

        private void Awake()
        {
            if (Get == null)
                Get = this;
            else if (Get != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }
        
        public void Start()
        {
            loader = new WorldLoader();
        }

        private void Update()
        {
            MeshChunks();
        }

        public void OnDestroy()
        {
            loader.SetRunning(false);
        }

        public void AddChunk(Chunk c)
        {
            for (int y = 0; y < Settings.ChunkSectionsPerChunk; y++)
            {
                Vector3Int key = new Vector3Int(c.Pos.x, y, c.Pos.y);
                ChunkRenderer renderer = Instantiate(chunkPrefab, worldParent).GetComponent<ChunkRenderer>();
                renderer.chunk = c;

                chunkRenderer.Add(key, renderer);
            }

            foreach (ChunkSection section in c.sections)
                loader.Enqueue(section);
        }

        public void RemoveChunk(Vector2Int pos)
        {
            List<Vector3Int> toRemove = new List<Vector3Int>();

            foreach (var key in chunkRenderer.Keys)
                if (key.x == pos.x && key.z == pos.y)
                    toRemove.Add(key);

            foreach (var key in toRemove)
            {
                Destroy(chunkRenderer[key].gameObject);
                chunkRenderer.Remove(key);
            }
        }

        public void UpdateSection(ChunkSection section)
        {
            if (chunkRenderer.ContainsKey(section.Pos))
                if (section.Meshed)
                {
                    LoadedData data = new ChunkMeshBuilder(section).BuildChunk();
                    chunkRenderer[section.Pos].CommitMesh(data);
                }
        }

        void MeshChunks()
        {
            while (loader.Dequeue(out LoadedData data))
                if (chunkRenderer.ContainsKey(data.Pos))
                    chunkRenderer[data.Pos].CommitMesh(data);
        }

    }
}
