using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft
{
    class ChunkRenderer : MonoBehaviour
    {
        public Chunk chunk = null;

        public MeshFilter WorldMeshFilter = null;
        public MeshCollider WorldMeshCollider = null;

        public MeshFilter FluidMeshFilter = null;

        public MeshFilter FoliageMeshFilter = null;
        public MeshCollider FoliageMeshCollider = null;

        public void CommitMesh(LoadedData data)
        {
            WorldMeshFilter.mesh = data.WorldMesh.ToMesh();
            WorldMeshCollider.sharedMesh = data.WorldMesh.ToMesh();

            FluidMeshFilter.mesh = data.FluidMesh.ToMesh();

            FoliageMeshFilter.mesh = data.FoliageMesh.ToMesh();
            FoliageMeshCollider.sharedMesh = data.FoliageMesh.ToMesh();

            chunk.Meshed = true;
        }
        void Start()
        {
        }

        public void Regenerate()
        {
            //Util.MeasureTime(() => new ChunkMeshBuilder(chunk, this).BuildChunk());
            LoadedData data = new ChunkMeshBuilder(chunk).BuildChunk();
            CommitMesh(data);
        }
    }
}
