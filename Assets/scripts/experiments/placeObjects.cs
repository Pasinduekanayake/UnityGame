using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class placeObjects : MonoBehaviour
{
    public GameObject accessPosi;
    private positionDetails pos_script;

    public GameObject accessPathf;
    private pathFinding path_script;

    public GameObject pickables;

    private int count = 0;

    public List<Vector3> objectsList;

    private List<List<Vector3>>  randomClusters = new List<List<Vector3>>();

    public void placeObjectsMethod(Vector3 StartNode)
    {
        pos_script = accessPosi.GetComponent<positionDetails>();
        path_script = accessPathf.GetComponent<pathFinding>();
        objectsList = new List<Vector3>();
        initializeRandomClusters();

        createRandomClusters(randomClusters[0]);
        createRandomClusters(randomClusters[1]);
        createRandomClusters(randomClusters[2]);
        createRandomClusters(randomClusters[3]);

        callSearchAlgo(randomClusters[1], StartNode);
        callSearchAlgo(randomClusters[2], StartNode);
        callSearchAlgo(randomClusters[3], StartNode);

    }

    public void initializeRandomClusters()
    {
        for (int i=0; i < pos_script.ClustersList.Count; i++)
        {
            randomClusters.Add(pos_script.ClustersList[i]);
        }
    }

    public void createRandomClusters(List<Vector3> cluster)
    {
        for (int i = 0; i < cluster.Count; i++)
        {
            Vector3 clusterCurrentIndex = cluster[i];
            int randomIndex = Random.Range(i, cluster.Count);
            cluster[i] = cluster[randomIndex];
            cluster[randomIndex] = clusterCurrentIndex;
        }
    }

    public void callSearchAlgo(List<Vector3> cluster, Vector3 StartNode)
    {
        count = 0;
        foreach (var clusterPoint in cluster)
        {
            if (count > 4)
            {
                return;
            }

            if (path_script.Search3(StartNode, clusterPoint))
            {
                count++;
                objectsList.Add(clusterPoint);
            }
        }
    }

    public void createPickableObjects()
    {
        foreach(var objects in objectsList){
            Instantiate(pickables, new Vector3(objects.x, objects.y + 2.4f, objects.z), Quaternion.identity, transform);
        }
    }

}

