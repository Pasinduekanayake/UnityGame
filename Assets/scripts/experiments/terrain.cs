using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrain : MonoBehaviour
{
    public GameObject access;
    private kmean clustering_script;

    public GameObject accessTile;
    private tileGrid tileGrid_script;

    public GameObject accessPos;
    private positionDetails pos_script;

    public GameObject accessPath;
    private pathFinding path_script;

    public GameObject accessObjects;
    private placeObjects objects_script;

    public int width = 50;
    public int depth = 50;
    public GameObject pointsPrefab;
    public List<GameObject> verticesList;

    public Vector3[] vertices;
    int[] triangles;

    Vector2[] uvs;

    MeshFilter meshFilter;
    Mesh mesh;

    Dictionary<GameObject, List<GameObject>> clusters;

    // Start is called before the first frame update
    void Start()
    {
        clustering_script = access.GetComponent<kmean>();
        tileGrid_script = accessTile.GetComponent<tileGrid>();
        pos_script = accessPos.GetComponent<positionDetails>();
        path_script = accessPath.GetComponent<pathFinding>();
        objects_script = accessObjects.GetComponent<placeObjects>();

        initializeMesh();
        createMesh();
        updateMesh();

        DrawVertice();

        clustering_script.start();
        clusters = clustering_script.clusters;
        terrainUpdateUsingClusterPoints();
        updateMesh();

        tileGrid_script.start();

        Vector3 StartNode = pos_script.getStartNode();
        //Debug.Log(StartNode + ": start node");
        //path_script.Search(StartNode);
        //path_script.Search2(StartNode);

        objects_script.placeObjectsMethod(StartNode);
        objects_script.createPickableObjects();

        path_script.printValue();
        //Debug.Log("Path script executed");
    }

    private void initializeMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "Procedural Mesh";
        //update the MeshFilter with empty mesh
        meshFilter.mesh = mesh;
    }

    private void createMesh()
    {
        createVertices();
        createTriangles();
        // createColors();
        // createUVs();
    }

    private void updateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.colors = colors;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private void DrawVertice()
    {
        for (int k = 0; k < vertices.Length; k++)
        {
            var verticesNew = Instantiate(pointsPrefab, vertices[k], Quaternion.identity, transform);

            verticesList.Add(verticesNew);
        }
    }

    private void createVertices()
    {
        vertices = new Vector3[(width + 1) * (depth + 1)];
        int vertexIndex = 0; //keep track of current index
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                vertices[vertexIndex] = new Vector3(x, 0, z); // y value will be updated with perlin noise

                vertexIndex++;
            }
        }
    }

    private void terrainUpdateUsingClusterPoints()
    {
        //Debug.Log("Entered successfully");
        var clusterCounter = 0;
        foreach (var cluster in clusters)
        {
            foreach (var point in cluster.Value)
            {
                float x = point.transform.position.x;
                float z = point.transform.position.z;
                float y;
                if (clusterCounter == 0)
                {
                    y = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 2f;
                    if (y < 2f)
                    {
                        y = 2f;
                    }
                    point.transform.position = new Vector3(x, y, z);
                }
                else if(clusterCounter == 1)
                {
                    y = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 4f;
                    if (y < 2f)
                    {
                        y = 2f;
                    }
                    point.transform.position = new Vector3(x, y, z);
                }
                else if(clusterCounter == 2)
                {
                    y = Mathf.PerlinNoise(x * 0.2f, z * 0.2f) * 4f;
                    if (y < 2f)
                    {
                        y = 2f;
                    }
                    point.transform.position = new Vector3(x, y, z);
                }
                else
                {
                    y = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 5f;
                    if (y < 2f)
                    {
                        y = 2f;
                    }
                    point.transform.position = new Vector3(x, y, z);
                }


                for(int i=0; i< vertices.Length; i++)
                {
                    if(vertices[i].x == x & vertices[i].z == z)
                    {
                        vertices[i].y = y;
                    }
                }
            }
            clusterCounter++;
        }
    }

    private void createTriangles()
    {
        triangles = new int[width * depth * 6];
        int currentVertexPoint = 0;
        int currentTrianglePoint = 0;
        //clock-wise because we do not want to render the bottom part of the vertices
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[currentTrianglePoint + 0] = currentVertexPoint + 0;
                triangles[currentTrianglePoint + 1] = currentVertexPoint + width + 1;
                triangles[currentTrianglePoint + 2] = currentVertexPoint + 1;

                triangles[currentTrianglePoint + 3] = currentVertexPoint + 1;
                triangles[currentTrianglePoint + 4] = currentVertexPoint + width + 1;
                triangles[currentTrianglePoint + 5] = currentVertexPoint + width + 2;

                currentVertexPoint++;
                currentTrianglePoint += 6;
            }
            currentVertexPoint++;
        }

    }


}
