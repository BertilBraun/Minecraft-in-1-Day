using UnityEngine;

namespace Assets.Minecraft
{
    class TextureAtlas
    {
        public static Vector2[][] GenerateUVs(int idx)
        {
            return GenerateUVs(idx, idx, idx);
        }
        public static Vector2[][] GenerateUVs(int idxUp, int idxDown, int idxSides)
        {
            Vector2[][] uvs = new Vector2[6][];

            for (int i = 0; i < 6; i++)
            {
                if (i == (int)Direction.Up)
                    uvs[i] = GetUVs(idxUp);
                else if (i == (int)Direction.Down)
                    uvs[i] = GetUVs(idxDown);
                else
                    uvs[i] = GetUVs(idxSides);
            }

            return uvs;
        }
        
        static float indivTextureW = 1 / Mathf.Floor(Settings.TextureSize / Settings.BlockTextureSize);
        static Vector2[] GetUVs(int idx)
        {
            float   x0 = (idx % Settings.TexturesPerRow) * indivTextureW,
                    y0 = Mathf.FloorToInt(idx / Settings.TexturesPerRow) * indivTextureW,
                    x1 = x0 + indivTextureW,
                    y1 = y0 + indivTextureW;
            return new Vector2[] {
                new Vector2(x1, 1f - y1),
                new Vector2(x0, 1f - y1),
                new Vector2(x0, 1f - y0),
                new Vector2(x1, 1f - y0),
            };
        }
    }
}
