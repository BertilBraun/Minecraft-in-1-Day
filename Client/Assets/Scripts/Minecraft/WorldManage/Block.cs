using Assets.Scripts;
using Assets.Scripts.Minecraft.WorldManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Minecraft
{
    public enum BlockType : byte
    {
        Air,
        Dirt,
        Grass,
        Stone,
        Wood,
        Leave,
        Plank,
        Glass,
        Torch,
        Water,

        Count
    }

    public class BlockDictionary
    {
        static Dictionary<BlockType, Block> dict = null;

        static void Init()
        {
            dict = new Dictionary<BlockType, Block> {
                { BlockType.Air,    new CodeMeshedBlock(BlockType.Air, MeshOrder.World, 0, false, false) },
                { BlockType.Dirt,   new CodeMeshedBlock(BlockType.Dirt, MeshOrder.World, 2, true, true) },
                { BlockType.Grass,  new CodeMeshedBlock(BlockType.Grass, MeshOrder.World, 0, 2, 3, true, true) },
                { BlockType.Stone,  new CodeMeshedBlock(BlockType.Stone, MeshOrder.World, 1, true, true) },
                { BlockType.Wood,   new CodeMeshedBlock(BlockType.Wood, MeshOrder.World, 21, 21, 20, true, true) },
                { BlockType.Leave,  new CodeMeshedBlock(BlockType.Leave, MeshOrder.World, 53, true, true) },
                { BlockType.Plank,  new CodeMeshedBlock(BlockType.Plank, MeshOrder.World, 4, true, true) },
                { BlockType.Glass,  new CodeMeshedBlock(BlockType.Glass, MeshOrder.World, 49, true, false) },
                { BlockType.Torch,  new LoadedMeshBlock(BlockType.Torch, MeshOrder.World, "Assets/Assets/cube.obj", false, false) },
                { BlockType.Water,  new LoadedMeshBlock(BlockType.Water, MeshOrder.Fluid, "Assets/Assets/cube.obj", false, false) },
            };
        }

        static public Block Get(BlockType type)
        {
            if (dict == null)
                Init();
            return dict[type];
        }
    }

    public enum MeshOrder
    {
        World,
        Fluid,
        Foliage
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
                activeBuilder.AddQuad(pos, dir, UVs);
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

    public class LoadedMeshBlock : Block
    {
        public Mesh mesh;

        public LoadedMeshBlock(BlockType _type, MeshOrder _meshOrder, string _meshPath, bool _solid, bool _opaque)
            : base(_type, _meshOrder, _solid, _opaque)
        {
            mesh = new OBJImporter().ImportFile(_meshPath);
        }

        public override void Generate(MeshBuilder activeBuilder, Vector3Int pos, ChunkSection c)
        {
            activeBuilder.AddData(mesh.vertices, mesh.normals, mesh.uv, mesh.triangles, mesh.triangles.Max() + 1);
            Debug.Log("vertices " + mesh.vertices.Length);
            Debug.Log("normals " + mesh.normals.Length);
            Debug.Log("uv " + mesh.uv.Length);
            Debug.Log("triangles" + mesh.triangles.Length);
            Debug.Log("Max" + mesh.triangles.Max());
        }
    }

    public abstract class Block
    {
        public BlockType Type { get; private set; }
        public MeshOrder Order { get; private set; }

        public bool Solid { get; private set; }
        public bool Opaque { get; private set; }


        public Block(BlockType _type, MeshOrder _meshOrder, bool _solid, bool _opaque)
        {
            Type = _type;
            Order = _meshOrder;
            Solid = _solid;
            Opaque = _opaque;
        }

        public abstract void Generate(MeshBuilder activeBuilder, Vector3Int pos, ChunkSection c);
    }

}
