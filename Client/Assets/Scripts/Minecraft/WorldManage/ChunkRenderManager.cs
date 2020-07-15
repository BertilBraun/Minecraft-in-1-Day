using Assets.Minecraft;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class ChunkRenderManager : MonoBehaviour
    {
        public static ChunkRenderManager Get;
        public Transform worldParent;
        public GameObject chunkPrefab;
        public GameObject lightPrefab;

        WorldLoader loader;
        Dictionary<Vector3Int, ChunkRenderer> chunkRenderer = new Dictionary<Vector3Int, ChunkRenderer>();
        HashSet<ChunkSection> sectionsToMesh = new HashSet<ChunkSection>();

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
                renderer.transform.position = key * Settings.ChunkSectionSize;
                renderer.chunk = c;
                renderer.section = c.sections[y];

                chunkRenderer.Add(key, renderer);
            }

            foreach (ChunkSection section in c.sections)
                Process(section);
        }

        ChunkSection GetChunkSection(Vector3Int pos)
        {
            return chunkRenderer[pos].section;
        }

        void Process(ChunkSection section)
        {
            Vector3Int pos = section.Pos;
            sectionsToMesh.Add(section);

            TryToLoad(new Vector3Int(pos.x - 1, pos.y, pos.z));
            TryToLoad(new Vector3Int(pos.x + 1, pos.y, pos.z));
            TryToLoad(new Vector3Int(pos.x, pos.y - 1, pos.z));
            TryToLoad(new Vector3Int(pos.x, pos.y + 1, pos.z));
            TryToLoad(new Vector3Int(pos.x, pos.y, pos.z - 1));
            TryToLoad(new Vector3Int(pos.x, pos.y, pos.z + 1));

            TryToLoad(pos);
        }
        void TryToLoad(Vector3Int pos)
        {
            if (AllNeighborsLoaded(pos))
            {
                loader.Enqueue(GetChunkSection(pos));
                sectionsToMesh.Remove(GetChunkSection(pos));
            }
        }
        bool AllNeighborsLoaded(Vector3Int pos)
        {
            bool isOnY;
            if (pos.y == 0)
                isOnY = Loaded(pos.x, pos.y + 1, pos.z);
            else if (pos.y == Settings.ChunkSectionsPerChunk - 1)
                isOnY = Loaded(pos.x, pos.y - 1, pos.z);
            else
                isOnY = Loaded(pos.x, pos.y - 1, pos.z) && Loaded(pos.x, pos.y + 1, pos.z);

            return isOnY &&
                Loaded(pos.x - 1, pos.y, pos.z) &&
                Loaded(pos.x + 1, pos.y, pos.z) &&
                Loaded(pos.x, pos.y, pos.z - 1) &&
                Loaded(pos.x, pos.y, pos.z + 1);
        }
        bool Loaded(int x, int y, int z)
        {
            return chunkRenderer.ContainsKey(new Vector3Int(x, y, z));
        }

        public void RemoveChunk(int x, int z)
        {
            List<Vector3Int> toRemove = new List<Vector3Int>();

            foreach (var key in chunkRenderer.Keys)
                if (key.x == x && key.z == z)
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
                {
                    ChunkRenderer renderer = chunkRenderer[data.Pos];
                    renderer.CommitMesh(data);
                 
                    foreach (var pos in data.LightPos)
                    {
                        Debug.Log("Added light");
                        Instantiate(lightPrefab, renderer.transform).transform.position = pos;
                    }
                }
        }

    }
}