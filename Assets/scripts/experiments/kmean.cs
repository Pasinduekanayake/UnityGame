using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class kmean : MonoBehaviour
{
    public GameObject accessNew;
    private terrain terrain_script;

    // We don't have a data set so we we generate a random one
    private int Width;
    private int Depth;
    private int Points;
    private int Centroids = 4;
    private GameObject Point;
    public GameObject Centroid;
    public Transform PointsHolder;
    public Transform CentroidsHolder;
    int finishMark = 0;
    // public GameObject DoneText;

    List<GameObject> points;
    List<GameObject> centroids;
    List<Color> colors; // Each time we generate a new data set, we will also generate new colours for our clusters that we can work with
    public Dictionary<GameObject, List<GameObject>> clusters; // Keys are centroid gameobjects (clusters), values are gameobjects that represents the points that belong to the cluster
    List<Vector3> previousCentroids; // This is needed to determine when it's the time to stop. If the positions of centroids in the current iteration is the same as the positions from the previous iteration

    // Start is called before the first frame update
    public void start()
    {
        terrain_script = accessNew.GetComponent<terrain>();

        Width = terrain_script.width;
        Depth = terrain_script.depth;

        Points = terrain_script.vertices.Length;

        Point = terrain_script.pointsPrefab;
        //Debug.Log("point" + Point);
        //Debug.Log("points" + Points);

        StartKMeansClustering();
    }

    public void StartKMeansClustering()
    {
        ClearData();

        // Initialization
        points = terrain_script.verticesList;
        centroids = GenerateGameObjects(Centroid, Centroids, CentroidsHolder);
        previousCentroids = GetCentroidsList();
        colors = GenerateColors();
        SetColorsToCentroids();

        // Start with an execution of the algorithm
        while (finishMark != centroids.Count)
        {
            Cluster();
        }
    }

    private List<Vector3> GetCentroidsList()
    {
        var result = new List<Vector3>();

        foreach (var item in centroids)
        {
            result.Add(item.transform.position);
        }

        return result;
    }

    public void Cluster()
    {
        // Construct clusters dictionary
        clusters = InitializeClusters();

        // Add points to clusters they belong
        AddPointsToClusters();

        // If there's a cluster with no points extract the closest point and add it to the empty cluster
        CheckForEmptyClusters();

        // Set colors to points from each cluster
        SetColorToClusterPoints();

        // Take the sum of all the positions in the cluster and divide by the number of points
        RecomputeCentroidPositions();

        // Check if no centroids changed their position
        CheckForEnd();

        // Update previous centroids to the positions of current
        UpdatePreviousCentroids();
    }

    private void ClearData()
    {
        DeleteChildren(PointsHolder);
        DeleteChildren(CentroidsHolder);
        // DoneText.SetActive(false);
    }

    private void DeleteChildren(Transform parent)
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }

    private void UpdatePreviousCentroids()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            previousCentroids[i] = centroids[i].transform.position;
        }
    }

    private void CheckForEnd()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            if (centroids[i].transform.position != previousCentroids[i])
            {
                finishMark = 0;
                return;
            }
            else
            {
                finishMark++;
                //Debug.Log("Finish");
            }
        }

        // DoneText.SetActive(true);
    }

    private void RecomputeCentroidPositions()
    {
        var clusterCounter = 0;
        foreach (var cluster in clusters)
        {
            var sum = Vector3.zero;

            foreach (var point in cluster.Value)
            {
                sum += point.transform.position;
            }

            var average = sum / cluster.Value.Count;
            centroids[clusterCounter].transform.position = average;
            clusterCounter++;
        }
    }

    private void SetColorToClusterPoints()
    {
        var clusterCounter = 0;
        foreach (var cluster in clusters)
        {
            foreach (var point in cluster.Value)
            {
                point.GetComponent<MeshRenderer>().material.color = colors[clusterCounter];
            }
            clusterCounter++;
        }
    }

    private void CheckForEmptyClusters()
    {
        foreach (var cluster in clusters)
        {
            if (cluster.Value.Count == 0)
            {
                var closestPoint = ExtractClosestPointToCluster(cluster.Key.transform.position);
                cluster.Value.Add(closestPoint);
            }
        }
    }

    private GameObject ExtractClosestPointToCluster(Vector3 clusterPosition)
    {
        var closestPoint = points[0];
        GameObject clusterThePointBelongsTo = null;
        var minDistance = float.MaxValue;

        // Looping through points is not a good idea because we need to find a cluster that has more than 1 item
        // That's why we will loop through all the clusters and the points in clusters
        // We only take the point if the cluster has more than 1 item, otherwise we'd take the item from the cluster that has 1 item
        // Which means that cluster would end up with no items and we will have the same problem
        foreach (var cluster in clusters)
        {
            foreach (var point in cluster.Value)
            {
                var distance = Vector3.Distance(point.transform.position, clusterPosition);
                if (distance < minDistance && cluster.Value.Count > 1)
                {
                    closestPoint = point;
                    minDistance = distance;
                    clusterThePointBelongsTo = cluster.Key;
                }
            }
        }

        clusters[clusterThePointBelongsTo].Remove(closestPoint);
        return closestPoint;
    }

    private Dictionary<GameObject, List<GameObject>> InitializeClusters()
    {
        // At this point we will have the centroids already generated
        var result = new Dictionary<GameObject, List<GameObject>>();

        for (int i = 0; i < Centroids; i++)
        {
            result.Add(centroids[i], new List<GameObject>());
        }

        return result;
    }

    private void AddPointsToClusters()
    {
        for (int i = 0; i < Points; i++)
        {
            var pointPosition = points[i].transform.position;
            var minDistance = float.MaxValue;
            var closestCentroid = centroids[0]; // We can randomly pick any centroid as this will update later

            for (int j = 0; j < Centroids; j++)
            {
                var distance = Vector3.Distance(pointPosition, centroids[j].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCentroid = centroids[j];
                }
            }

            clusters[closestCentroid].Add(points[i]);
        }
    }

    private void SetColorsToCentroids()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            centroids[i].GetComponent<MeshRenderer>().material.color = colors[i];
        }
    }

    private List<Color> GenerateColors()
    {
        var result = new List<Color>();

        Color newColor1 = new Color(0.91f, 0.21f, 0.81f);        //pink
        Color newColor2 = new Color(0.24f, 0.21f, 0.91f);        //blue
        Color newColor3 = new Color(0.1f, 0.1f, 0.1f);           //green
        Color newColor4 = new Color(0.91f, 0.29f, 0.21f);        //red

        result.Add(newColor1);
        result.Add(newColor2);
        result.Add(newColor3);
        result.Add(newColor4);

        return result;
    }

    private List<GameObject> GenerateGameObjects(GameObject prefab, int size, Transform parent)
    {
        var result = new List<GameObject>();

        for (int i = 0; i < size; i++)
        {
            var prefabXScale = prefab.transform.localScale.x;
            var positionX = UnityEngine.Random.Range(-Width / 2 + prefabXScale, Width / 2 - prefabXScale);

            var prefabZScale = prefab.transform.localScale.z;
            var positionZ = UnityEngine.Random.Range(-Depth / 2 + prefabZScale, Depth / 2 - prefabZScale);

            var positionY = prefab.transform.position.y + 2f;

            var newPosition = new Vector3(positionX, positionY, positionZ);
            var newGameObject = Instantiate(prefab, newPosition, Quaternion.identity, parent);

            result.Add(newGameObject);
        }

        return result;
    }
}
