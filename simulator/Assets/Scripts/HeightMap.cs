using UnityEngine;

/* Generate height map from the perlin noise */
public class HeightMap
{
    public int size;
    public float[,] heights;

    public HeightMap(int size)
    {
        this.size = size;
        this.heights = new float[size, size];
    }

    public static HeightMap generateHeightMap(Vector2Int tilePos, float tileSize, Vector2Int perlinOffset, float terrainNoiseScale, int borderedResolution)
    {        
        HeightMap hm = new HeightMap(borderedResolution);
        
        double step = tileSize * 1.0 / (borderedResolution - 3);

        for (int x = 0; x < borderedResolution; x++)
        {
            for (int y = 0; y < borderedResolution; y++)
            {
                float worldX = (float) ((x - tilePos.x) * step + tilePos.x * tileSize);
                float worldY = (float) ((y - tilePos.y) * step + tilePos.y * tileSize);

                float perlinX = (terrainNoiseScale * worldX / tileSize) + perlinOffset.x;
                float perlinY = (terrainNoiseScale * worldY / tileSize) + perlinOffset.y;

                float sampleHeight = Mathf.PerlinNoise(perlinX, perlinY);
                hm.heights[x, y] = sampleHeight;
            }
        }

        return hm;
    }
}