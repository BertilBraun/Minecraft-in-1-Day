using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Minecraft
{
    class ChunkMeshBuilder
    {
        Chunk c = null;
        MeshBuilder activeBuilder = null;
        MeshBuilder WorldMeshBuilder, FluidMeshBuilder, FoliageMeshBuilder;

        public ChunkMeshBuilder(Chunk _c)
        {
            c = _c;

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
            activeBuilder = GetActiveMesh(data);

            switch (data.Mesh)
            {
                case MeshType.Block:

                    TryAddFace(pos, Direction.Down, data);
                    TryAddFace(pos, Direction.Up, data);

                    TryAddFace(pos, Direction.East, data);
                    TryAddFace(pos, Direction.West, data);

                    TryAddFace(pos, Direction.South, data);
                    TryAddFace(pos, Direction.North, data);
                    break;
                case MeshType.X:
                    Debug.Log("Not yet implemented"); // TODO
                    break;
                default:
                    break;
            }
        }

        void TryAddFace(Vector3Int pos, int dir, Block block)
        {
            if (ShouldMakeFace(block, pos, dir))
            {
                Vector3Int posInWorldSpace = new Vector3Int(
                        pos.x + c.Pos.x * Settings.ChunkSize.x,
                        pos.y,
                        pos.z + c.Pos.y * Settings.ChunkSize.z );

                activeBuilder.AddQuad(posInWorldSpace, dir, block);
            }
        }

        bool ShouldMakeFace(Block block, Vector3Int pos, int dir)
        {
            Vector3Int adj = pos - Direction.Offset[dir];
            BlockType adjBlock = c.GetBlock(adj.x, adj.y, adj.z);

            if (block.Order == MeshOrder.Fluid && block.Type == adjBlock)
                return false;
            
            if (!BlockDictionary.Get(adjBlock).Opaque)
                return true;

            return false;
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
            return !(c.IsLayerEmpty(y) ||
                    (c.IsLayerSolid(y) &&
                    c.IsLayerSolid(y - 1) &&
                    c.IsLayerSolid(y + 1) &&
                    c.world.GetChunk(c.Pos.x - 1, c.Pos.y).IsLayerSolid(y) &&
                    c.world.GetChunk(c.Pos.x + 1, c.Pos.y).IsLayerSolid(y) &&
                    c.world.GetChunk(c.Pos.x, c.Pos.y - 1).IsLayerSolid(y) &&
                    c.world.GetChunk(c.Pos.x, c.Pos.y + 1).IsLayerSolid(y)));
        }

        public LoadedData BuildChunk()
        {
            for (int y = 0; y < Settings.ChunkSize.y; y++)
            {
                if (!ShouldCheckLayer(y))
                    continue;

                for (int z = 0; z < Settings.ChunkSize.z; z++)
                    for (int x = 0; x < Settings.ChunkSize.x; x++)
                        CheckBlock(x, y, z);
            }

            return new LoadedData(c.Pos, WorldMeshBuilder, FluidMeshBuilder, FoliageMeshBuilder);
        }
    }

    class CubeMeshBuilder
    {
        MeshBuilder builder = new MeshBuilder();

        public Mesh Build(BlockType type)
        {
            Block block = BlockDictionary.Get(type);

            for (int i = 0; i < 6; i++)
                builder.AddQuad(Vector3Int.zero, i, block);

            return builder.ToMesh();
        }
    }
    class MeshBuilder
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();
        int idx = 0;

        Vector3[] GetVertices(Vector3Int pos, int dir, int w, int h)
        {
            var verts = (Vector3[])Direction.Vertices[dir].Clone();

            for (int i = 0; i < verts.Length; i++)
                verts[i] += pos;
            return verts;
        }

        public void AddQuad(Vector3Int pos, int dir, Block block, int w = 1, int h = 1)
        {
            vertices.AddRange(GetVertices(pos, dir, w, h));

            for (int i = 0; i < 4; i++)
                normals.Add(-Direction.Offset[dir]);
            for (int i = 0; i < 4; i++)
                uv.Add(block.UVs[dir][i]);

            triangles.AddRange(new List<int> { 0 + idx, 1 + idx, 2 + idx, 2 + idx, 3 + idx, 0 + idx });
            idx += 4;
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
