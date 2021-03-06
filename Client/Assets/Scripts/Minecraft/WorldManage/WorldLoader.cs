﻿using Assets.Scripts.Minecraft.WorldManage;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Minecraft
{
    [Serializable]
    public class LoadedData
    {
        [SerializeField]
        public Vector3Int Pos;
        [SerializeField]
        public MeshBuilder WorldMesh;
        [SerializeField]
        public MeshBuilder FluidMesh;
        [SerializeField]
        public MeshBuilder FoliageMesh;

        public List<Vector3> LightPos;

        public LoadedData(Vector3Int pos, MeshBuilder worldMesh, MeshBuilder fluidMesh, MeshBuilder foliageMesh, List<Vector3> lightPos)
        {
            Pos = pos;
            WorldMesh = worldMesh;
            FluidMesh = fluidMesh;
            FoliageMesh = foliageMesh;

            LightPos = lightPos;
        }
    }
    class WorldLoader
    {
        Thread thread;
        Queue<ChunkSection> toLoad;
        Queue<LoadedData> loaded;
        bool isRunning = true;

        public WorldLoader()
        {
            toLoad = new Queue<ChunkSection>();
            loaded = new Queue<LoadedData>();

            thread = new Thread(Loop);
            thread.Start();
        }

        public void SetRunning(bool r)
        {
            isRunning = r;
        }

        public void Enqueue(ChunkSection c)
        {
            lock (toLoad)
            {
                c.Meshed = true; // TODO Leave here?
                toLoad.Enqueue(c.Copy()); // TODO Copy maybe? to not have to worry about threading access
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
                try
                {
                    Load();
                }
                catch (Exception)
                {
                    Debug.Log("Meshing Exception, Retrying...");
                }
            }
        }
        void Load()
        {
            ChunkSection c;
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
