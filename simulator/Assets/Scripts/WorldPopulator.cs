using System.Linq;
using UnityEngine;

/*
    Populate the tile with trees or other objects
 */

public class WorldPopulator : MonoBehaviour
{
    [Header("Tree generation parameters")]
    public float noiseScale; // sample perlin noise from 0 to noiseScale for each tile
    public int treeDensity; // max trees for each tile
    [Range(0, 1)] public float treeTreshold, maxRandomOffset;

    [Space(10)]

    [Header("Reference objects and weights")]
    public GameObject[] referenceTrees;
    public float[] weights;

    private WorldGenerator worldGen;
    private int perlinOffsetX, perlinOffsetY;

    private float randomOffset; // maximum postion offset from the grid

    void Start() {
        worldGen = FindObjectOfType<WorldGenerator>();

        System.Random prng = new System.Random(worldGen.seed);
        //generates a big offset to avoid symetry
        this.perlinOffsetX = prng.Next(-100000, +100000);
        this.perlinOffsetY = prng.Next(-100000, +100000);
    }

    void OnValidate()
    {
        if(worldGen != null) {
            System.Random prng = new System.Random(worldGen.seed);
            //generates a big offset to avoid symetry
            this.perlinOffsetX = prng.Next(-100000, +100000);
            this.perlinOffsetY = prng.Next(-100000, +100000);

            worldGen.reloadTerrain();
        }
    }

    public GameObject createTrees(Vector2Int tilePos, TerrainTile tile) {
        int tileSize = worldGen.tileSize;
        GameObject parent = new GameObject("trees");

        float tilex = tilePos.x * tileSize - tileSize/2;
        float tilez = tilePos.y * tileSize - tileSize/2;

        float step = tileSize / Mathf.Sqrt(treeDensity);
        this.randomOffset = step * maxRandomOffset;

        for (float x = tilex; x < tilex + tileSize; x += step)
        {
            for (float z = tilez; z < tilez + tileSize; z += step)
            {
                float perlinX = (noiseScale * x / tileSize) + perlinOffsetX;
                float perlinY = (noiseScale * z / tileSize) + perlinOffsetY;

                float sample = Mathf.PerlinNoise(perlinX, perlinY);
                if (sample > treeTreshold)
                {
                    // spawn tree with some random rotation and offset
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    float offsetx = Random.Range(0f, randomOffset);
                    float offsetz = Random.Range(0f, randomOffset);

                    // actually spawns the tree
                    GameObject reference = drawFromList(referenceTrees, weights);

                    float xTree = x + offsetx;
                    float zTree = z + offsetz;
                    float yTree = getYFromXZ(new Vector2(xTree, zTree), tile);

                    if (yTree > worldGen.waterLevel) {
                        Vector3 treeOrigin = new Vector3(xTree, yTree, zTree);
                        GameObject tree = GameObject.Instantiate(reference, treeOrigin, rotation);

                        // parents to the empty
                        tree.transform.parent = parent.transform;
                    }
                }
            }
        }

        return parent;
    }
    private float getYFromXZ(Vector2 xz, TerrainTile tile) {
        RaycastHit hit;
        Physics.Raycast(new Vector3(xz.x, 1000, xz.y), Vector3.down, out hit, 1000);
        return hit.point.y;
    }

    /* randomly draws an item from the list according to the weghts array */
    private GameObject drawFromList(GameObject[] objects, float[] weights) {
        if (objects.Length != weights.Length)
            return null;

        float totalWeight = weights.Sum();
        float r = Random.Range(0f, totalWeight);
        float sum = 0;
        for(int i = 0; i < objects.Length; i++) {
            sum += weights[i];
            if(r < sum) {
                return objects[i];
            }
        }

        return objects[objects.Length - 1];
    }
}
