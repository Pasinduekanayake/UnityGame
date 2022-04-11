using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visualizeClusters : MonoBehaviour
{
    public GameObject accessGrid;
    private tileGrid grid_script;

    public GameObject accessPositions;
    private positionDetails position_script;

    public GameObject accessClustering;
    private clustering clustering_script;

    public void VisualizeClusters()
    {
        grid_script = accessGrid.GetComponent<tileGrid>();
        position_script = accessPositions.GetComponent<positionDetails>();
        clustering_script = accessClustering.GetComponent<clustering>();

        List<List<Vector3>> clusterlist = position_script.ClustersList;
        HashSet<Vector3> WalkableCellsHash = grid_script.WalkableCellsHash;
        List<GameObject> tiles = grid_script.WalkableCells;
        List<GameObject> centroids = clustering_script.centroids;

        int verticeCount = 0;
        foreach(var vertice in WalkableCellsHash)
        {
            for(int j=0; j< clusterlist.Count; j++)
            {
                if (clusterlist[j].Contains(vertice))
                {
                    tiles[verticeCount].GetComponent<MeshRenderer>().material.color = centroids[j].GetComponent<MeshRenderer>().material.color;
                }
            }
            verticeCount++;
        }
    }
}
