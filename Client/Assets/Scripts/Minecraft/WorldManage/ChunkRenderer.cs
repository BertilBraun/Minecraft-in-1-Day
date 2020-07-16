using Assets.Scripts.Minecraft.WorldManage;
using System;
using System.IO;
using UnityEngine;

namespace Assets.Minecraft
{
    public class ChunkRenderer : MonoBehaviour
    {
        public Chunk chunk = null;
        public ChunkSection section = null;

        public MeshFilter WorldMeshFilter = null;
        public MeshFilter FluidMeshFilter = null;
        public MeshFilter FoliageMeshFilter = null;

        public void CommitMesh(LoadedData data)
        {
            WorldMeshFilter.mesh = data.WorldMesh.ToMesh();
            FluidMeshFilter.mesh = data.FluidMesh.ToMesh();
            FoliageMeshFilter.mesh = data.FoliageMesh.ToMesh();

            chunk.Meshed = true;

            var pos = section.Pos;
            if (pos.x > 97 && pos.x < 102 && pos.z > 98 && pos.z < 104 && WorldMeshFilter.mesh.vertices.Length > 0 && pos.y > 0)
            {
                string path = "Assets/Chunks/" + pos.ToString() + ".dat";
                //File.WriteAllText(path, JsonUtility.ToJson(data, true));
            }

        }
    }
}
