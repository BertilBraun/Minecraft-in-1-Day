using Assets.Minecraft;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Minecraft.WorldManage
{
    public class BlockMeshingManager : MonoBehaviour
    {
        public static BlockMeshingManager Get;

        [Serializable]
        public struct NamedMesh
        {
            public BlockType type;
            public Mesh mesh;
        }
        public NamedMesh[] typeToMesh;

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

    }

    public class CodeMeshedBlock : Block
    {
        public Vector2[][] UVs { get; private set; }

        public CodeMeshedBlock(BlockType _type, MeshOrder _meshOrder, int AtlasIndex, bool _solid, bool _opaque) :
            this(_type, _meshOrder, AtlasIndex, AtlasIndex, AtlasIndex, _solid, _opaque)
        { }

        public CodeMeshedBlock(BlockType _type, MeshOrder _meshOrder, int AtlasIndexUp, int AtlasIndexDown, int AtlasIndexSides, bool _solid, bool _opaque)
            : base(_type, _meshOrder, _solid, _opaque)
        {
            UVs = TextureAtlas.GenerateUVs(AtlasIndexUp, AtlasIndexDown, AtlasIndexSides);
        }

        public override void Generate(MeshBuilder activeBuilder, Vector3Int pos, ChunkSection c)
        {
            TryAddFace(activeBuilder, pos, BlockMesh.Down, c);
            TryAddFace(activeBuilder, pos, BlockMesh.Up, c);

            TryAddFace(activeBuilder, pos, BlockMesh.East, c);
            TryAddFace(activeBuilder, pos, BlockMesh.West, c);

            TryAddFace(activeBuilder, pos, BlockMesh.South, c);
            TryAddFace(activeBuilder, pos, BlockMesh.North, c);
        }

        void TryAddFace(MeshBuilder activeBuilder, Vector3Int pos, int dir, ChunkSection c)
        {
            if (ShouldMakeFace(pos, dir, c))
                activeBuilder.AddQuad(BlockMesh.Vertices, pos, dir, UVs);
        }

        bool ShouldMakeFace(Vector3Int pos, int dir, ChunkSection c)
        {
            Vector3Int adj = pos - BlockMesh.Offset[dir];
            BlockType adjBlock = BlockType.Air;
            if (c != null)
                adjBlock = c.GetBlock(adj.x, adj.y, adj.z);

            if (Order == MeshOrder.Fluid && Type == adjBlock)
                return false;

            if (!BlockDictionary.Get(adjBlock).Opaque)
                return true;

            return false;
        }

    }

    public class CodeFluidMeshedBlock : Block
    {
        public Vector2[][] UVs { get; private set; }

        public CodeFluidMeshedBlock(BlockType _type, int AtlasIndex, bool _solid, bool _opaque)
            : base(_type, MeshOrder.Fluid, _solid, _opaque)
        {
            UVs = TextureAtlas.GenerateUVs(AtlasIndex, AtlasIndex, AtlasIndex);
        }

        public override void Generate(MeshBuilder activeBuilder, Vector3Int pos, ChunkSection c)
        {
            Vector3Int top = pos - BlockMesh.Offset[BlockMesh.Up];
            BlockType topBlock = (c != null) ? c.GetBlock(top.x, top.y, top.z) : BlockType.Air;
            Vector3[][] vertices;

            if (topBlock != Type)
                vertices = FluidMesh.Vertices;
            else
                vertices = BlockMesh.Vertices;

            TryAddFace(activeBuilder, pos, BlockMesh.East, c, vertices);
            TryAddFace(activeBuilder, pos, BlockMesh.West, c, vertices);

            TryAddFace(activeBuilder, pos, BlockMesh.South, c, vertices);
            TryAddFace(activeBuilder, pos, BlockMesh.North, c, vertices);

            TryAddFace(activeBuilder, pos, BlockMesh.Down, c, vertices);
            TryAddFace(activeBuilder, pos, BlockMesh.Up, c, vertices);
        }

        void TryAddFace(MeshBuilder activeBuilder, Vector3 pos, int dir, ChunkSection c, Vector3[][] vertices)
        {
            if (c == null)
            {
                activeBuilder.AddQuad(vertices, pos, dir, UVs);
                return;
            }

            Vector3Int adj = (pos - BlockMesh.Offset[dir]).ToIntVec();
            BlockType adjBlock = c.GetBlock(adj.x, adj.y, adj.z);

            if (Type != adjBlock && !BlockDictionary.Get(adjBlock).Opaque)
                activeBuilder.AddQuad(vertices, pos, dir, UVs);
        }

    }

    public class LoadedMeshBlock : Block
    {
        public Mesh mesh;

        public LoadedMeshBlock(BlockType _type, MeshOrder _meshOrder, bool _solid, bool _opaque)
            : base(_type, _meshOrder, _solid, _opaque)
        {
            mesh = BlockMeshingManager.Get.typeToMesh.FirstOrDefault(nM => nM.type == _type).mesh;
            Debug.Assert(mesh != null, "Mesh type not found in dictionary, add it to BlockMeshingManager");
        }

        public override void Generate(MeshBuilder activeBuilder, Vector3Int pos, ChunkSection c)
        {
            var verts = (Vector3[])mesh.vertices.Clone();
            for (int i = 0; i < verts.Length; i++)
                verts[i] += pos;
            activeBuilder.AddData(verts, mesh.normals, mesh.uv, mesh.triangles, mesh.triangles.Max() + 1);
        }
    }

}
