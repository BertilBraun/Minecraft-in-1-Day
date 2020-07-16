using Assets.Scripts.Minecraft.WorldManage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Minecraft
{
    class ChunkMeshBuilder
    {
        ChunkSection c = null;
        MeshBuilder WorldMeshBuilder, FluidMeshBuilder, FoliageMeshBuilder;
        List<Vector3> lightPos;

        public ChunkMeshBuilder(ChunkSection _c)
        {
            c = _c;

            lightPos = new List<Vector3>();
            WorldMeshBuilder = new MeshBuilder();
            FluidMeshBuilder = new MeshBuilder();
            FoliageMeshBuilder = new MeshBuilder();
        }

        void CheckBlock(int x, int y, int z)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            BlockType block = c.GetBlock(x, y, z);

            if (block == BlockType.Air)
                return;

            Block data = BlockDictionary.Get(block);
            data.Generate(GetActiveMesh(data), pos, c);
        }

        MeshBuilder GetActiveMesh(Block block)
        {
            switch (block.Order)
            {
                case MeshOrder.World:
                    return WorldMeshBuilder;
                case MeshOrder.Fluid:
                    return FluidMeshBuilder;
                case MeshOrder.Foliage:
                    return FoliageMeshBuilder;
            }
            Debug.Assert(false);
            return null;
        }

        bool ShouldCheckLayer(int y)
        {
            return !(//c.IsLayerEmpty(y) ||
                    (c.IsLayerSolid(y) &&
                    c.IsLayerSolid(y - 1) &&
                    c.IsLayerSolid(y + 1) &&
                    World.Get.IsLayerSolid(c.Pos.x - 1, y, c.Pos.y) &&
                    World.Get.IsLayerSolid(c.Pos.x + 1, y, c.Pos.y) &&
                    World.Get.IsLayerSolid(c.Pos.x, y, c.Pos.y - 1) &&
                    World.Get.IsLayerSolid(c.Pos.x, y, c.Pos.y + 1)));
        }

        public LoadedData BuildChunk()
        {
            for (int y = 0; y < Settings.ChunkSectionSize.y; y++)
            {
                if (!ShouldCheckLayer(y))
                    continue;

                for (int z = 0; z < Settings.ChunkSectionSize.z; z++)
                    for (int x = 0; x < Settings.ChunkSectionSize.x; x++)
                        CheckBlock(x, y, z);
            }

            return new LoadedData(c.Pos, WorldMeshBuilder, FluidMeshBuilder, FoliageMeshBuilder, lightPos);
        }
    }

    class CubeMeshBuilder
    {
        MeshBuilder builder = new MeshBuilder();

        public Mesh Build(BlockType type)
        {
            Block block = BlockDictionary.Get(type);

            block.Generate(builder, Vector3Int.zero, null);
            return builder.ToMesh();
        }
    }

    [Serializable]
    public class MeshBuilder
    {
        [SerializeField]
        List<Vector3> vertices = new List<Vector3>();
        [SerializeField]
        List<Vector3> normals = new List<Vector3>();
        [SerializeField]
        List<Vector2> uv = new List<Vector2>();
        [SerializeField]
        List<int> triangles = new List<int>();
        [NonSerialized]
        int idx = 0;

        Vector3[] GetVertices(Vector3[][] vertices, Vector3 pos, int dir)
        {
            var verts = (Vector3[])vertices[dir].Clone();
            for (int i = 0; i < verts.Length; i++)
                verts[i] += pos;
            return verts;
        }

        public void AddQuad(Vector3[][] vertices, Vector3 pos, int dir, Vector2[][] uvs, float w = 1, float h = 1)
        {
            Vector3[] normals = new Vector3[4] {
                -BlockMesh.Offset[dir], -BlockMesh.Offset[dir], -BlockMesh.Offset[dir], -BlockMesh.Offset[dir]
            };

            AddData(GetVertices(vertices, pos, dir), normals, uvs[dir], new int[] { 0, 1, 2, 2, 3, 0 }, 4);
        }

        public void AddData(Vector3[] _vertices, Vector3[] _normals, Vector2[] _uv, int[] indecies, int idxcount)
        {
            vertices.AddRange(_vertices);
            normals.AddRange(_normals);
            uv.AddRange(_uv);

            foreach (int i in indecies)
                triangles.Add(i + idx);
            idx += idxcount;
        }

        public Mesh ToMesh()
        {
            Mesh mesh = new Mesh();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uv.ToArray();

            return mesh;
        }
    }
}
