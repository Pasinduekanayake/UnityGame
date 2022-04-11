using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clustering : MonoBehaviour
{
    public GameObject accessNew;
    private terrain terrain_script;

    private int Width;
    private int Depth;
    private int Centroids = 5;
    private Vector3 Point;
    public GameObject Centroid;
    public Transform PointsHolder;
    public Transform CentroidsHolder;
    int finishMark = 0;
    List<Vector3> posNew;
    public List<GameObject> centroids;
    List<Color> colors;
    public Dictionary<GameObject, List<Vector3>> clusters; 
    List<Vector3> CentroidsList; 

    // Start is called before the first frame update
    public void start()
    {
        terrain_script = accessNew.GetComponent<terrain>();

        Width = terrain_script.width;
        Depth = terrain_script.depth;

        StartKMeansClustering();
    }

    public void StartKMeansClustering()
    {
        ClearData();

        posNew = new List<Vector3>(terrain_script.vertices);
        centroids = GenerateGameObjects(Centroid, Centroids, CentroidsHolder);
        CentroidsList = GetCentroidsList();
        colors = GenerateColors();
        SetColorsToCentroids();

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
        clusters = ClusterInitialisation();

        AddVerticesToClusters();

        EmptyClustersCheck();

        CentroidPositionsRecompute();

        EndingCheck();

        PreviousCentroidsUpdate();
    }

    private void ClearData()
    {
        DeleteChildren(PointsHolder);
        DeleteChildren(CentroidsHolder);
    }

    private void DeleteChildren(Transform parent)
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }

    private void PreviousCentroidsUpdate()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            CentroidsList[i] = centroids[i].transform.position;
        }
    }

    private void EndingCheck()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            if (centroids[i].transform.position != CentroidsList[i])
            {
                finishMark = 0;
                return;
            }
            else
            {
                finishMark++;
            }
        }
    }

    private void CentroidPositionsRecompute()
    {
        var clusterCounter = 0;
        foreach (var cluster in clusters)
        {
            var sum = Vector3.zero;

            foreach (var vertice in cluster.Value)
            {
                sum += vertice;
            }

            var average = sum / cluster.Value.Count;
            centroids[clusterCounter].transform.position = average;
            clusterCounter++;
        }
    }

    private void EmptyClustersCheck()
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

    private Vector3 ExtractClosestPointToCluster(Vector3 clusterPosition)
    {
        Vector3 closestPos = Point;
        GameObject clusterThePosBelongsTo = null;
        var minDistance = float.MaxValue;

        foreach (var cluster in clusters)
        {
            foreach (var vertice in cluster.Value)
            {
                var distance = Vector3.Distance(vertice, clusterPosition);
                if (distance < minDistance && cluster.Value.Count > 1)
                {
                    closestPos = vertice;
                    minDistance = distance;
                    clusterThePosBelongsTo = cluster.Key;
                }
            }
        }

        clusters[clusterThePosBelongsTo].Remove(closestPos);
        return closestPos;
    }

    private Dictionary<GameObject, List<Vector3>> ClusterInitialisation()
    {
        var result = new Dictionary<GameObject, List<Vector3>>();

        for (int i = 0; i < Centroids; i++)
        {
            result.Add(centroids[i], new List<Vector3>());
        }

        return result;
    }

    private void AddVerticesToClusters()
    {
        for (int i = 0; i < posNew.Count; i++)
        {
            var pointPosition = posNew[i];
            var minDistance = float.MaxValue;
            var closestCentroid = centroids[0];

            for (int j = 0; j < Centroids; j++)
            {
                var distance = Vector3.Distance(pointPosition, centroids[j].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCentroid = centroids[j];
                }
            }

            clusters[closestCentroid].Add(posNew[i]);
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
        Color newColor3 = new Color(0.1f, 0.1f, 0.1f);           //black
        Color newColor4 = new Color(0.91f, 0.29f, 0.21f);        //red
        Color newColor5 = new Color(1f, 1f, 1f);                 //white

        result.Add(newColor1);
        result.Add(newColor2);
        result.Add(newColor3);
        result.Add(newColor4);
        result.Add(newColor5);

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

            var positionY = prefab.transform.position.y + 4f;

            var newPosition = new Vector3(positionX, positionY, positionZ);
            var newGameObject = Instantiate(prefab, newPosition, Quaternion.identity, parent);

            result.Add(newGameObject);
        }

        return result;
    }
}
