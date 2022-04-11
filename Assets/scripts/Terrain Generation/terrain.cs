using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class terrain : MonoBehaviour
{
    [Header("Access Scripts")]
    public GameObject accessCluster;
    private clustering clustering_script;
    public GameObject accessTile;
    private tileGrid tileGrid_script;
    public GameObject accessPos;
    private positionDetails pos_script;
    public GameObject accessObjects;
    private placeObjects objects_script;
    public GameObject accessPathFinding;
    private pathFinding path_script;
    public GameObject accessVisualizeCluster;
    private visualizeClusters visualizeCluster_script;
    public GameObject accessTerrainColors;
    private terrainColors terrainColors_script;
    public GameObject accessEnemies;
    private placeEnemies enemies_script;

    [Header("Access Buttons")]
    private int uiStates = 0;
    public GameObject ButtonPlaceVegetation;
    Button accessPlaceVegetation;
    public GameObject ButtonPlaceTiles;
    Button accessPlaceTiles;
    public GameObject ButtonPlacePlayer;
    Button accessPlacePlayer;
    public GameObject ButtonPlaceObjects;
    Button accessPlaceObjects;
    public GameObject ButtonPlaceEnemies;
    Button accessPlaceEnemies;
    public GameObject ButtonPlacePath;
    Button accessPlacePath;
    public GameObject ButtonPlaceClusters;
    Button accessPlaceClusters;

    [Header("Terrain Generation")]
    public int width = 50;
    public int depth = 50;
    public GameObject playerPrefab;
    public Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    MeshFilter meshFilter;
    Mesh mesh;

    [Header("Terrain Noise")]
    public int octaves = 0;
    public float persistance = 0;
    public float lacunarity = 0;
    public float heightMultiplier = 0;
    public AnimationCurve heightCurveCluster1;
    public AnimationCurve heightCurveCluster2;
    public AnimationCurve heightCurveCluster3;
    public AnimationCurve heightCurveCluster4;
    public AnimationCurve heightCurveCluster5;

    [Header("Pathfinding and Clustering")]
    Vector3 StartNode;
    public int numberOfObjects = 8;
    public Dictionary<GameObject, List<Vector3>> clusters;

    [Header("Place Enemy")]
    private bool enemiesAdd = false;
    private bool shouldPlayerMove;
    private List<Vector3> playerPath = new List<Vector3>();
    private int pathIndex1;
    private int pathIndex2;
    public GameObject EnemyPrefab;
    GameObject enemyInstance;

    // Start is called before the first frame update
    void Start()
    {
        clustering_script = accessCluster.GetComponent<clustering>();
        tileGrid_script = accessTile.GetComponent<tileGrid>();
        pos_script = accessPos.GetComponent<positionDetails>();
        objects_script = accessObjects.GetComponent<placeObjects>();
        path_script = accessPathFinding.GetComponent<pathFinding>();
        visualizeCluster_script = accessVisualizeCluster.GetComponent<visualizeClusters>();
        terrainColors_script = accessTerrainColors.GetComponent<terrainColors>();
        enemies_script = accessEnemies.GetComponent<placeEnemies>();

        accessPlaceVegetation = ButtonPlaceVegetation.GetComponent<Button>();
        accessPlaceVegetation.interactable = true;
        accessPlaceTiles = ButtonPlaceTiles.GetComponent<Button>();
        accessPlaceTiles.interactable = false;
        accessPlacePlayer = ButtonPlacePlayer.GetComponent<Button>();
        accessPlacePlayer.interactable = false;
        accessPlaceObjects = ButtonPlaceObjects.GetComponent<Button>();
        accessPlaceObjects.interactable = false;
        accessPlaceEnemies = ButtonPlaceEnemies.GetComponent<Button>();
        accessPlaceEnemies.interactable = false;
        accessPlacePath = ButtonPlacePath.GetComponent<Button>();
        accessPlacePath.interactable = false;
        accessPlaceClusters = ButtonPlaceClusters.GetComponent<Button>();
        accessPlaceClusters.interactable = false;

        initializeMesh();
        createMesh();

        clustering_script.start();
        clusters = clustering_script.clusters;
        terrainUpdateUsingClusterPoints();
        createMeshColors();
        updateMesh();
    }

    public void placeVegetation()
    {
        if (uiStates == 0)
        {
            tileGrid_script.GenerateVegetation();
            accessPlaceVegetation.gameObject.SetActive(false);
            accessPlaceTiles.interactable = true;
            uiStates++;
        }
    }

    public void placeTiles()
    {
        if (uiStates == 1)
        {
            tileGrid_script.GenerateGrid();
            accessPlaceTiles.gameObject.SetActive(false);
            accessPlacePlayer.interactable = true;
            uiStates++;
        }
    }

    public void placePlayer()
    {
        //cant place a player without tiles
        if (uiStates == 2)
        {
            StartNode = pos_script.getStartNode();
            Instantiate(playerPrefab, new Vector3(StartNode.x, StartNode.y + 2.5f, StartNode.z), Quaternion.identity, transform);
            accessPlacePlayer.gameObject.SetActive(false);
            accessPlaceObjects.interactable = true;
            uiStates++;
        }
    }

    public void placeObjects()
    {
        //cant place objects without placing a player
        if (uiStates == 3)
        {
            objects_script.placeObjectsMethod(StartNode, numberOfObjects);
            objects_script.createPickableObjects(numberOfObjects);
            accessPlaceObjects.gameObject.SetActive(false);
            accessPlaceEnemies.interactable = true;
            uiStates++;
        }
    }

    public void addEnemies()
    {
        //cant add enemies without placing objects
        if (uiStates == 4)
        {
            enemiesAdd = !enemiesAdd;
            enemyInstance = enemies_script.PlaceEnemies(1, EnemyPrefab);
            var path1 = enemies_script.pathDetails(path_script.enemyParentsList[0], enemies_script.startNode[0], enemies_script.endNode[0]);
            MovePlayer(path1);
            accessPlaceEnemies.gameObject.SetActive(false);
            accessPlacePath.interactable = true;
            uiStates++;
        }
    }

    public void pathVisualize()
    {
        //cant visualize path without placing objects
        if (uiStates == 5)
        {
            path_script.pathVisualize(StartNode);
            accessPlacePath.gameObject.SetActive(false);
            accessPlaceClusters.interactable = true;
            uiStates++;
        }
    }

    public void clusterVisualize()
    {
        //cant visualize clusters without tiles
        if (uiStates == 6)
        {
            visualizeCluster_script.VisualizeClusters();
            accessPlaceClusters.gameObject.SetActive(false);
            uiStates++;
        }
    }

    private void MovePlayer(List<Vector3> path)
    {
        shouldPlayerMove = true;
        playerPath = path;
        pathIndex1 = playerPath.Count - 1;
        pathIndex2 = 0;
    }

    private void initializeMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "Procedural Mesh";
        meshFilter.mesh = mesh;
    }

    private void createMesh()
    {
        createVertices();
        createTriangles();
    }

    private void createMeshColors()
    {
        createColors();
        createUVs();
    }

    private void updateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private void createVertices()
    {
        vertices = new Vector3[(width + 1) * (depth + 1)];
        int vertexIndex = 0;
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                vertices[vertexIndex] = new Vector3(x, 0, z);
                vertexIndex++;
            }
        }
    }

    private Tuple<float, float> checkHeight(float maxHeight, float minHeight, float y)
    {
        if (y > maxHeight){
            maxHeight = y;
        }else if (y < minHeight){
            minHeight = y;
        }
        var tuple = new Tuple<float, float>(maxHeight, minHeight);
        return tuple;
    }

    private void terrainUpdateUsingClusterPoints()
    {
        var clusterCounter = 0;
        var verticeCounter = 0;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        foreach (var cluster in clusters)
        {
            foreach (var point in cluster.Value)
            {
                float x = point.x;
                float z = point.z;
                float y;
                if (clusterCounter == 0)
                {
                    y = customNoise(x, z, 20, octaves, persistance, lacunarity);
                    var result = checkHeight(maxNoiseHeight, minNoiseHeight, y);
                    maxNoiseHeight = result.Item1;
                    minNoiseHeight = result.Item2;
                }
                else if(clusterCounter == 1)
                {
                    y = customNoise(x, z, 20, octaves, persistance, lacunarity);
                    var result = checkHeight(maxNoiseHeight, minNoiseHeight, y);
                    maxNoiseHeight = result.Item1;
                    minNoiseHeight = result.Item2;
                }
                else if(clusterCounter == 2)
                {
                    y = customNoise(x, z, 20, octaves, persistance, lacunarity);
                    var result = checkHeight(maxNoiseHeight, minNoiseHeight, y);
                    maxNoiseHeight = result.Item1;
                    minNoiseHeight = result.Item2;
                }
                else if (clusterCounter == 3)
                {
                    y = customNoise(x, z, 20, octaves, persistance, lacunarity);
                    var result = checkHeight(maxNoiseHeight, minNoiseHeight, y);
                    maxNoiseHeight = result.Item1;
                    minNoiseHeight = result.Item2;
                }
                else
                {
                    y = customNoise(x, z, 20, octaves, persistance, lacunarity);
                    var result = checkHeight(maxNoiseHeight, minNoiseHeight, y);
                    maxNoiseHeight = result.Item1;
                    minNoiseHeight = result.Item2;
                }

                for(int i=0; i< vertices.Length; i++)
                {
                    if(vertices[i].x == x & vertices[i].z == z)
                    {
                        vertices[i].y = y;
                    }
                }
                verticeCounter++;
            }
            clusterCounter++;
        }

        clusterCounter = 0;

        foreach (var cluster in clusters)
        {
            foreach (var point in cluster.Value)
            {
                float x = point.x;
                float z = point.z;

                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].x == x & vertices[i].z == z & clusterCounter == 0)
                    {
                        vertices[i].y = heightCurveCluster1.Evaluate(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, vertices[i].y)) * heightMultiplier;
                    }
                    else if (vertices[i].x == x & vertices[i].z == z & clusterCounter == 1)
                    {
                        vertices[i].y = heightCurveCluster2.Evaluate(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, vertices[i].y)) * heightMultiplier;
                    }
                    else if (vertices[i].x == x & vertices[i].z == z & clusterCounter == 2)
                    {
                        vertices[i].y = heightCurveCluster3.Evaluate(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, vertices[i].y)) * heightMultiplier;
                    }
                    else if (vertices[i].x == x & vertices[i].z == z & clusterCounter == 3)
                    {
                        vertices[i].y = heightCurveCluster4.Evaluate(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, vertices[i].y)) * heightMultiplier;
                    }
                    else if (vertices[i].x == x & vertices[i].z == z & clusterCounter == 4)
                    {
                        vertices[i].y = heightCurveCluster5.Evaluate(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, vertices[i].y)) * heightMultiplier;
                    }
                }
            }
            clusterCounter++;
        }
    }

    private float customNoise(float x, float z, float scale, int octaves, float persistance, float lacunarity)
    {
        float amplitudeValue = 1;
        float frequencyValue = 1;
        float noiseHeightValue = 0;

        for (int i=0; i < octaves; i++)
        {
            float xSample = x / scale * frequencyValue;
            float zSample = z / scale * frequencyValue;

            float perlinNoise = Mathf.PerlinNoise(xSample, zSample) * 2 - 1;
            noiseHeightValue += perlinNoise * amplitudeValue;

            amplitudeValue *= persistance;
            frequencyValue *= lacunarity;
        }

        return noiseHeightValue;
    }

    private void createColors()
    {
        terrainColors_script.addColors(width, depth, vertices);
    }

    private void createUVs()
    {
        uvs = new Vector2[vertices.Length];
        var vertexIndex = 0;
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                uvs[vertexIndex] = new Vector2((float)x / width, (float)z / depth);
                vertexIndex++;
            }
        }
    }


    private void createTriangles()
    {
        triangles = new int[width * depth * 6];
        int currentVertexPoint = 0;
        int currentTrianglePoint = 0;
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

    private void Update()
    {
        if (enemiesAdd)
        {
            enemyMove();
        }

    }

    private void enemyMove()
    {
        if (shouldPlayerMove)
        {
            var nextCellToVisit = playerPath[pathIndex1];

            enemyInstance.transform.position = Vector3.MoveTowards(enemyInstance.transform.position, nextCellToVisit, 3 * Time.deltaTime);
            enemyInstance.transform.LookAt(nextCellToVisit);

            if (enemyInstance.transform.position == nextCellToVisit)
                pathIndex1--;

            if (pathIndex1 < 0)
            {
                shouldPlayerMove = false;
                pathIndex2 = 0;
            }
        }
        else
        {
            var nextCellToVisit = playerPath[pathIndex2];

            enemyInstance.transform.position = Vector3.MoveTowards(enemyInstance.transform.position, nextCellToVisit, 3 * Time.deltaTime);
            enemyInstance.transform.LookAt(nextCellToVisit);

            if (enemyInstance.transform.position == nextCellToVisit)
                pathIndex2++;

            if (pathIndex2 > playerPath.Count - 1)
            {
                shouldPlayerMove = true;
                pathIndex1 = playerPath.Count - 1;
            }
        }
    }
}
