using Assets.Minecraft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class MenuChunkRenderer : MonoBehaviour
    {
        public MeshFilter WorldMeshFilter = null;
        public MeshFilter FluidMeshFilter = null;
        public MeshFilter FoliageMeshFilter = null;

        public void Init(string path)
        {
            LoadedData data = JsonUtility.FromJson<LoadedData>(File.ReadAllText(path));

            transform.localPosition = data.Pos * 16;
            WorldMeshFilter.mesh = data.WorldMesh.ToMesh();
            FluidMeshFilter.mesh = data.FluidMesh.ToMesh();
            FoliageMeshFilter.mesh = data.FoliageMesh.ToMesh();

        }
    }
}
