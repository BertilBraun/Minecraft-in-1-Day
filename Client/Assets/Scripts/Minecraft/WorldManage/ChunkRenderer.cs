using System;
using UnityEngine;

namespace Assets.Minecraft
{
    class ChunkRenderer : MonoBehaviour
    {
        public Chunk chunk = null;

        public MeshFilter WorldMeshFilter = null;
        public MeshFilter FluidMeshFilter = null;
        public MeshFilter FoliageMeshFilter = null;

        public void CommitMesh(LoadedData data)
        {
            WorldMeshFilter.mesh = data.WorldMesh.ToMesh();
            FluidMeshFilter.mesh = data.FluidMesh.ToMesh();
            FoliageMeshFilter.mesh = data.FoliageMesh.ToMesh();

            chunk.Meshed = true;
        }
    }
}
