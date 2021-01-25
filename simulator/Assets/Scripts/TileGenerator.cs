using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creates a single terrain tile using perlin noise
 */

public class TileGenerator
{
    private WorldGenerator worldGen;

    private Vector2Int perlinOffset;

    public TileGenerator(WorldGenerator worldGen) {
        this.worldGen = worldGen;
        setPerlinOffset();
    }

    public void setPerlinOffset() {
        /* changes the seed a tiny bit to get a different output */
        System.Random prng = new System.Random(worldGen.seed + 1);
        this.perlinOffset.x = prng.Next(-100000, +100000);
        this.perlinOffset.y = prng.Next(-100000, +100000);
    }

    public TerrainTile createNewTile(Vector2Int tilePos, int resolution)
    {
        float groundLevel = worldGen.groundLevel;
        int tileSize = worldGen.tileSize;
        float posX = tilePos.x * tileSize - tileSize/2;
        float posZ = tilePos.y * tileSize - tileSize/2;
        double step = tileSize * 1.0f / resolution;
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;


        int meshSize = resolution + 2;
        int borderedSize = meshSize + 2;

        MeshData meshData = new MeshData(meshSize);

        // generates the height map
        HeightMap hm = HeightMap.generateHeightMap(tilePos, tileSize, perlinOffset, worldGen.terrainNoiseScale, borderedSize);

        // vertex indices map, negative values are edges that will not be rendered
        int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;
        for (int y = 0; y < borderedSize; y++)
        {
            for (int x = 0; x < borderedSize; x++)
            {
                bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

                if(isBorderVertex) {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                } else {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        // for every point 
        for (int y = 0; y < borderedSize; y++)
        {
            for (int x = 0; x < borderedSize; x++)
            {
                int vertexIndex = vertexIndicesMap[x, y];
                
                // get the height of the current vertex
                float sample = hm.heights[x, y];
                sample = sample > groundLevel ? sample : groundLevel;
                sample *= worldGen.heightMultiplier;

                // finds the min and max height
                if (sample > maxHeight)
                    maxHeight = sample;
                if (sample < minHeight)
                    minHeight = sample;

                // adds vertex the the mesh data
                Vector3 vertexPosition = new Vector3((float) (posX + (x * step)),  sample, (float) (posZ + (y * step)));
                meshData.addVertex(vertexPosition, vertexIndex);   

                // creates the two triangles to form a square
                if (x < meshSize - 1 && y < meshSize - 1)
                {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x+1, y];
                    int c = vertexIndicesMap[x, y+1];
                    int d = vertexIndicesMap[x+1,y+1];
                    meshData.addTriangle(c, d, a);
                    meshData.addTriangle(b, a, d);
                }
            }
        }

        // creates mesh from meshData
        Mesh mesh = meshData.createMesh();

        // creates the mesh object, applies the material and give an explicit title
        GameObject meshObject = new GameObject();
        meshObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        meshObject.AddComponent<MeshRenderer>().sharedMaterial = worldGen.groundMaterial;
        meshObject.AddComponent<MeshCollider>();
        meshObject.name = "T(" + tilePos.x + "," + tilePos.y + ")";

        // parents to the woldGen gameObject to keep the hierarchy clean
        meshObject.transform.parent = worldGen.transform;

        return new TerrainTile(meshObject, hm, tilePos, minHeight, maxHeight);
    }

    // reverse normals on all triangles
    // obsololete since the normal orientation is defined by the vertex add order
    private void reverseNormals(GameObject meshObject) {
        Vector3[] normals = meshObject.GetComponent<MeshFilter>().sharedMesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        meshObject.GetComponent<MeshFilter>().sharedMesh.normals = normals;

        int[] triangles = meshObject.GetComponent<MeshFilter>().sharedMesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = t;
        }

        meshObject.GetComponent<MeshFilter>().sharedMesh.triangles = triangles;
    }

    // sub class used to store temporarily the mesh data before creating it
    public class MeshData
    {
        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;

        Vector3[] borderVertices;
        int[] borderTriangles;

        int triangleIndex;
        int borderTriangleIndex;

        public MeshData(int size)
        {
            vertices = new Vector3[size * size];
            uvs = new Vector2[size * size];
            triangles = new int[(size - 1) * (size - 1) * 6];
            borderVertices = new Vector3[size * 4 + 4]; // borders + corners
            borderTriangles = new int[24 * size]; // 6 * 4 * size
        }

        public void addVertex(Vector3 vertexPosition, int vertexIndex) {
            if(vertexIndex < 0) {
                borderVertices[-vertexIndex - 1] = vertexPosition;
            } else {
                vertices[vertexIndex] = vertexPosition;
                uvs[vertexIndex] = new Vector2(0, vertexPosition.y); ;
            }
        }

        public void addTriangle(int a, int b, int c)
        {
            if(a < 0 || b < 0 ||c < 0) {
                borderTriangles[borderTriangleIndex] = a;
                borderTriangles[borderTriangleIndex + 1] = b;
                borderTriangles[borderTriangleIndex + 2] = c;

                borderTriangleIndex += 3;
            } else {
                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;

                triangleIndex += 3;
            }

        }

        Vector3[] calculateNormals() {
            Vector3[] vertexNormals = new Vector3[vertices.Length];
            int triangleCount = triangles.Length / 3;
            for (int i = 0; i < triangleCount; i++) {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = triangles[normalTriangleIndex];
                int vertexIndexB = triangles[normalTriangleIndex + 1];
                int vertexIndexC = triangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = surfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }

            int borderTriangleCount = borderTriangles.Length / 3;
            for (int i = 0; i < borderTriangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = borderTriangles[normalTriangleIndex];
                int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
                int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = surfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                if(vertexIndexA >= 0) {
                    vertexNormals[vertexIndexA] += triangleNormal;
                }

                if (vertexIndexB >= 0) {
                    vertexNormals[vertexIndexB] += triangleNormal;
                }

                if (vertexIndexC >= 0) {
                    vertexNormals[vertexIndexC] += triangleNormal;
                }   
            }

            for (int i=0; i < vertexNormals.Length; i++) {
                vertexNormals[i].Normalize();
            }

            return vertexNormals;
        }

        Vector3 surfaceNormalFromIndices(int indexA, int indexB, int indexC) {
            Vector3 pointA = (indexA < 0) ? borderVertices[-indexA-1] : vertices[indexA];
            Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
            Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;

            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        // returns a mesh from all the data stored in the class 
        public Mesh createMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.normals = calculateNormals();
            return mesh;
        }
    }
}
