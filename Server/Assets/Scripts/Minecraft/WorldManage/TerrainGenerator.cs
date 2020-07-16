namespace Assets.Scripts.Minecraft.WorldManage
{
    class TerrainSetting
    {
        public static int waterLvl = 60;
    }

    public class TerrainGenerator
    {
        System.Random rand = new System.Random();

        Chunk c;

        public void GenerateTerrain(Chunk _c)
        {
            c = _c;
            if (c.Generated)
                return;

            GenerateHeightTerrain();
            GenerateFoliage();

            c.HeightMap.SetHeights(c);
        }

        void GenerateHeightTerrain()
        {
            c.HeightMap.Generate();

            for (int z = 0; z < Settings.ChunkSize.z; z++)
                for (int x = 0; x < Settings.ChunkSize.x; x++)
                {
                    int h = c.HeightMap[x, z];
                    for (int y = 0; y <= h; y++)
                        if (y == h && y <= TerrainSetting.waterLvl + 2)
                            c.SetBlock(x, y, z, BlockType.Sand);
                        else if (y == h)
                            c.SetBlock(x, y, z, BlockType.Grass);
                        else if (y > h - 2)
                            c.SetBlock(x, y, z, BlockType.Dirt);
                        else
                            c.SetBlock(x, y, z, BlockType.Stone);

                    for (int y = h + 1; y <= TerrainSetting.waterLvl; y++)
                        c.SetBlock(x, y, z, BlockType.Water);
                }
        }

        void GenerateFoliage()
        {
            for (int z = 0; z < Settings.ChunkSize.z; z++)
                for (int x = 0; x < Settings.ChunkSize.x; x++)
                {
                    if (rand.Next(0, 300) == 0 && c.HeightMap[x, z] > TerrainSetting.waterLvl + 3)
                        Tree(x, z);
                }
        }

        void Tree(int x, int z)
        {
            int y = c.HeightMap[x, z] + 1;
            int h = rand.Next(4, 6);

            for (int o = 0; o < h; o++)
                SetBlock(x, y + o, z, BlockType.Wood);

            Area(x - 2, z - 2, x + 2, z + 2, y + h - 2, BlockType.Leave);
            Area(x - 2, z - 2, x + 2, z + 2, y + h - 1, BlockType.Leave);
            Area(x - 1, z - 1, x + 1, z + 1, y + h, BlockType.Leave);

            SetBlock(x + 1, y + h + 1, z + 0, BlockType.Leave);
            SetBlock(x + 0, y + h + 1, z + 1, BlockType.Leave);
            SetBlock(x + 0, y + h + 1, z + 0, BlockType.Leave);
            SetBlock(x - 1, y + h + 1, z + 0, BlockType.Leave);
            SetBlock(x + 0, y + h + 1, z - 1, BlockType.Leave);
        }

        void Area(int sx, int sz, int ex, int ez, int y, BlockType type)
        {
            for (int x = sx; x <= ex; x++)
                for (int z = sz; z <= ez; z++)
                    SetBlock(x, y, z, type);
        }
        void SetBlock(int x, int y, int z, BlockType type)
        {
            if (c.GetBlock(x, y, z) == BlockType.Air)
                c.SetBlock(x, y, z, type);
        }
    }
}
