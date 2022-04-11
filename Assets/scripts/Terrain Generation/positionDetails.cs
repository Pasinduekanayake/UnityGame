using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionDetails : MonoBehaviour
{
    public GameObject accessGrid;
    private tileGrid grid_script;

    public GameObject accessClustering;
    private clustering clustering_script;

    public Vector3 StartNode;

    public HashSet<Vector3> WalkableCellsHash;

    public List<List<Vector3>> ClustersList = new List<List<Vector3>>();

    public List<Vector3> Cluster1points = new List<Vector3>();
    public List<Vector3> Cluster2points = new List<Vector3>();
    public List<Vector3> Cluster3points = new List<Vector3>();
    public List<Vector3> Cluster4points = new List<Vector3>();
    public List<Vector3> Cluster5points = new List<Vector3>();

    public List<GameObject> WalkableCellsObjects;

    public Vector3 getStartNode()
    {
        grid_script = accessGrid.GetComponent<tileGrid>();
        clustering_script = accessClustering.GetComponent<clustering>();
        WalkableCellsHash = grid_script.WalkableCellsHash;

        foreach (var vertice in WalkableCellsHash)
        {
            if (clustering_script.clusters[clustering_script.centroids[0]].Contains(vertice))
            {
                Cluster1points.Add(vertice);
            }
            else if (clustering_script.clusters[clustering_script.centroids[1]].Contains(vertice))
            {
                Cluster2points.Add(vertice);
            }
            else if (clustering_script.clusters[clustering_script.centroids[2]].Contains(vertice))
            {
                Cluster3points.Add(vertice);
            }
            else if (clustering_script.clusters[clustering_script.centroids[3]].Contains(vertice))
            {
                Cluster4points.Add(vertice);
            }
            else if (clustering_script.clusters[clustering_script.centroids[4]].Contains(vertice))
            {
                Cluster5points.Add(vertice);
            }
        }

        initializeClusterList();

        int randomIndex = Random.Range(1, ClustersList[0].Count);
        StartNode = ClustersList[0][randomIndex];

        return StartNode;
    }

    public void initializeClusterList()
    {
        ClustersList.Add(Cluster1points);
        ClustersList.Add(Cluster2points);
        ClustersList.Add(Cluster3points);
        ClustersList.Add(Cluster4points);
        ClustersList.Add(Cluster5points);
    }
}
