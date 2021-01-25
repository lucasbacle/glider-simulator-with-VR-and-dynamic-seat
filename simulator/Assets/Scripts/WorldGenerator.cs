using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Main class to generate the world procedurally, it allows fine control over terrain, water and clouds */

public class WorldGenerator : MonoBehaviour
{
    [Header("Global parameters")]
    public int seed; 
    public int tileSize; // taille de la tuile
    public int terrainResolution;
    [Header("The viewer generates terrain around it")]
    public GameObject viewer;

    [Space(10)]

    [Header("View distance")]
    [Range(0, 50)] public int viewDistance;
    [Range(0, 50)] public int objectViewDistance;

    [Space(10)]

    [Header("Terrain height")]
    public float terrainNoiseScale;
    public float heightMultiplier = 200;
    public static float defaultGliderHeight;

    [Range(0, 1)] public float groundLevel;
    public Material groundMaterial;

    [Header("Clouds")]
    public GameObject cloud;
    public float distanceFromGround;
    public Vector3 cloudForce;
    public int maxParticles;
    public float cloudHeight;

    private TileGenerator tileGenerator;
    private WorldPopulator worldPopulator;
    private Dictionary<Vector2Int, TerrainTile> tiles;
    private List<TerrainTile> visibleTiles;

    [Header("Water")]
    public GameObject water;
    public float waterLevel;

    // Start is called before the first frame update
    void Start()
    {
        if(MultiSceneValues.worldHeight != 0) {
            heightMultiplier = MultiSceneValues.worldHeight;
            WorldGenerator.defaultGliderHeight = heightMultiplier + 50;
        }

        tileGenerator = new TileGenerator(this);
        tiles = new Dictionary<Vector2Int, TerrainTile>();
        visibleTiles = new List<TerrainTile>();
        worldPopulator = FindObjectOfType<WorldPopulator>();

        initClouds();
        initWater();

        // set seed for random
        Random.InitState(seed);
    }

    // Update is called once per frame
    void Update()
    {
        // the object view distance should be lower or equal to the view distance
        objectViewDistance = Mathf.Clamp(objectViewDistance, 0, viewDistance);

        Vector3 position = viewer.transform.position;
        float poxXCorrected = position.x > 0 ? position.x + tileSize/2 : position.x - tileSize/2;
        float poxZCorrected = position.z > 0 ? position.z + tileSize / 2 : position.z - tileSize / 2;

        Vector2Int worldPos = new Vector2Int((int) (poxXCorrected / tileSize), (int) (poxZCorrected / tileSize));

        /* desactivation des tiles */
        loadGroundTiles(worldPos);
        loadTileObjects(worldPos);
    }

    // appellé lorsqu'on change un parametre dans l'inspector
    void OnValidate() {
        reloadTerrain();
    }

    void initWater() {
        Vector3 baseScale = water.transform.localScale;
        Vector3 newScale = new Vector3(this.tileSize * 50, this.tileSize * 50, baseScale.y);
        water.transform.localScale = newScale;
    }

    void initClouds() {
        ParticleSystem particles = cloud.GetComponent<ParticleSystem>();
        
        var sh = particles.shape;
        sh.enabled = true;
        sh.scale = new Vector3(tileSize, cloudHeight, tileSize);

        AnimationCurve curvex = new AnimationCurve();
        curvex.AddKey(0f, cloudForce.x);
        curvex.AddKey(1f, cloudForce.x);

        AnimationCurve curvey = new AnimationCurve();
        curvex.AddKey(0f, cloudForce.y);
        curvex.AddKey(1f, cloudForce.y);

        AnimationCurve curvez = new AnimationCurve();
        curvex.AddKey(0f, cloudForce.z);
        curvex.AddKey(1f, cloudForce.z);

        var fo = particles.forceOverLifetime;
        fo.enabled = true;
        fo.x = new ParticleSystem.MinMaxCurve(1f, curvex);
        fo.y = new ParticleSystem.MinMaxCurve(1f, curvey);
        fo.z = new ParticleSystem.MinMaxCurve(1f, curvez);

        particles.maxParticles = this.maxParticles;

        cloud.transform.position = new Vector3(0, distanceFromGround, 0);
    }

    public void reloadTerrain() {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }

        if (tiles != null)
            tiles.Clear();

        if (visibleTiles != null)
            visibleTiles.Clear();

        if (tileGenerator != null)
            tileGenerator.setPerlinOffset();

        initClouds();

        Random.InitState(seed);
    }

    private void unloadTerrain() {
        foreach (TerrainTile tile in visibleTiles)
        {
            Transform tileObjectsTransform = tile.tileObject.transform.Find("tileObjects");
            if(tileObjectsTransform != null) {
                tileObjectsTransform.gameObject.SetActive(false);
            }
            tile.tileObject.SetActive(false);
        }
        visibleTiles.Clear();
    }

    private void loadGroundTiles(Vector2Int worldPos) {
        List<TerrainTile> nextVisibleTiles = new List<TerrainTile>();

        /* affichage des tiles */
        for (int i = -viewDistance; i <= viewDistance; i++)
        {
            for (int j = -viewDistance; j <= viewDistance; j++)
            {
                Vector2Int tilePos = new Vector2Int(worldPos.x + i, worldPos.y + j);

                TerrainTile tile;
                if (!tiles.ContainsKey(tilePos))
                {
                    // crée une nouvelle tuile si elle n'existe pas
                    tile = tileGenerator.createNewTile(tilePos, terrainResolution);
                    
                    // activate and deactivate to allow ray tracing
                    tile.tileObject.SetActive(false);
                    tile.tileObject.SetActive(true);

                    // adds clouds to the tile
                    GameObject cloudClone = Instantiate(cloud, new Vector3(tilePos.x * tileSize, cloud.transform.position.y, tilePos.y * tileSize), Quaternion.identity);
                    cloudClone.transform.parent = tile.tileObject.transform;

                    if(tile.minHeight < this.waterLevel) {
                        // adds water to the tile
                        GameObject waterClone = Instantiate(water, new Vector3(tilePos.x * tileSize, waterLevel, tilePos.y * tileSize), Quaternion.Euler(new Vector3(-90, 0, 0)));
                        waterClone.transform.parent = tile.tileObject.transform;
                    }                    

                    tiles.Add(tilePos, tile); // ajout au dictionnaire 
                } else {
                    // la charge si elle existe deja
                    tile = tiles[tilePos];
                    tile.tileObject.SetActive(true);
                }


                nextVisibleTiles.Add(tile);
            }
        }

        //disable tiles that will not be visible next update
        foreach (TerrainTile tile in visibleTiles)
        {
            if(!nextVisibleTiles.Contains(tile)) {
                Transform tileObjectsTransform = tile.tileObject.transform.Find("tileObjects");
                if (tileObjectsTransform != null)
                {
                    tileObjectsTransform.gameObject.SetActive(false);
                }
                tile.tileObject.SetActive(false);
            }
        }

        visibleTiles = nextVisibleTiles;
    }

    private void loadTileObjects(Vector2Int worldPos) {
        /* affichage des elements de décor */
        for (int i = -objectViewDistance; i <= objectViewDistance; i++)
        {
            for (int j = -objectViewDistance; j <= objectViewDistance; j++)
            {
                Vector2Int tilePos = new Vector2Int(worldPos.x + i, worldPos.y + j);

                if (tiles.ContainsKey(tilePos))
                {
                    TerrainTile tile = tiles[tilePos];
                    Transform tileObjectsTransform = tile.tileObject.transform.Find("tileObjects");
                    if (tileObjectsTransform == null)
                    {
                        //creates tile objects
                        GameObject tileObjects = new GameObject("tileObjects");
                        GameObject trees = worldPopulator.createTrees(tilePos, tile);

                        //parents to the tile
                        trees.transform.parent = tileObjects.transform;
                        tileObjects.transform.parent = tile.tileObject.transform;
                    }
                    else
                    {
                        //activates tile objects
                        tileObjectsTransform.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Debug.LogError("Tried to populate a non existing tile");
                }
            }
        }
    }
}
