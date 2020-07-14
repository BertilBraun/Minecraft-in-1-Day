using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft
{
    class LoadedData
    {
        public readonly Vector2Int Pos;
        public readonly MeshBuilder WorldMesh;
        public readonly MeshBuilder FluidMesh;
        public readonly MeshBuilder FoliageMesh;

        public LoadedData(Vector2Int pos, MeshBuilder worldMesh, MeshBuilder fluidMesh, MeshBuilder foliageMesh)
        {
            Pos = pos;
            WorldMesh = worldMesh;
            FluidMesh = fluidMesh;
            FoliageMesh = foliageMesh;
        }
    }
    class WorldLoader
    {
        Thread thread;
        Queue<Chunk> toLoad;
        Queue<LoadedData> loaded;
        bool isRunning = true;

        public WorldLoader()
        {
            toLoad = new Queue<Chunk>();
            loaded = new Queue<LoadedData>();

            thread = new Thread(Loop);
            thread.Start();
        }

        public void SetRunning(bool r)
        {
            isRunning = r;
        }

        public void Enqueue(Chunk c)
        {
            lock (toLoad)
            {
                Debug.Log("Here");
                c.Meshed = true; // TODO Leave here?
                toLoad.Enqueue(new Chunk(c));
            }
        }

        public bool Dequeue(out LoadedData data)
        {
            data = null;
            lock (loaded)
            {
                if (loaded.Count == 0)
                    return false;

                data = loaded.Dequeue();
            }
            return true;
        }

        void Loop()
        {
            Thread.Sleep(200);

            while (isRunning)
            {
                Thread.Sleep(5);
                Load();
            }
        }
        void Load()
        {
            Chunk c;
            lock (toLoad)
            {
                if (toLoad.Count == 0)
                    return;

                c = toLoad.Dequeue();
            }

            LoadedData data = new ChunkMeshBuilder(c).BuildChunk();

            lock (loaded)
            {
                loaded.Enqueue(data);
            }
        }
    }
}
