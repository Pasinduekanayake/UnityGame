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

    [Header("Medicine")]
    public GameObject bandages;
    public GameObject medKit;

    [Header("Weapons")]
    public GameObject knife;
    public GameObject gun;

    [Header("Powers")]
    public GameObject superSpeed;
    public GameObject invisibility;

    [Header("Points")]
    public GameObject coin;
    public GameObject cash;

    private int count = 0;

    //this cluster will have 50% points and 50% weapons
    private List<Vector3> cluster1ObjectsList;
    //this cluster will have 40% points, 30% weapons and 30% medicine
    private List<Vector3> cluster2ObjectsList;
    //this cluster will have 20% points, 20% weapons, 20% powers and 40% medicine
    private List<Vector3> cluster3ObjectsList;
    //this cluster will have 20% points, 30% weapons, 30% powers and 20% medicine
    private List<Vector3> cluster4ObjectsList;
    //this cluster will have 30% weapons, 20% powers and 50% medicine
    public List<Vector3> cluster5ObjectsList;

    public List<List<Vector3>>  randomClusters = new List<List<Vector3>>();

    public void placeObjectsMethod(Vector3 StartNode, int numberOfObjects)
    {
        pos_script = accessPosi.GetComponent<positionDetails>();
        path_script = accessPathf.GetComponent<pathFinding>();

        cluster1ObjectsList = new List<Vector3>();
        cluster2ObjectsList = new List<Vector3>();
        cluster3ObjectsList = new List<Vector3>();
        cluster4ObjectsList = new List<Vector3>();
        cluster5ObjectsList = new List<Vector3>();
        initializeRandomClusters();

        createRandomClusters(randomClusters[0]);
        createRandomClusters(randomClusters[1]);
        createRandomClusters(randomClusters[2]);
        createRandomClusters(randomClusters[3]);
        createRandomClusters(randomClusters[4]);

        callSearchAlgo(randomClusters[0], StartNode, 1, numberOfObjects);
        callSearchAlgo(randomClusters[1], StartNode, 2, numberOfObjects);
        callSearchAlgo(randomClusters[2], StartNode, 3, numberOfObjects);
        callSearchAlgo(randomClusters[3], StartNode, 4, numberOfObjects);
        callSearchAlgo(randomClusters[4], StartNode, 5, numberOfObjects);
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

    public void callSearchAlgo(List<Vector3> cluster, Vector3 StartNode, int clusterId, int numberOfObjects)
    {
        count = 0;
        foreach (var clusterPoint in cluster)
        {
            if (count > (numberOfObjects-1))
            {
                return;
            }
            if ((clusterId == 1) && path_script.Search3(StartNode, clusterPoint, 1))
            {
                count++;
                cluster1ObjectsList.Add(clusterPoint);
            }
            else if ((clusterId == 2) && path_script.Search3(StartNode, clusterPoint, 1))
            {
                count++;
                cluster2ObjectsList.Add(clusterPoint);
            }
            else if ((clusterId == 3) && path_script.Search3(StartNode, clusterPoint, 1))
            {
                count++;
                cluster3ObjectsList.Add(clusterPoint);
            }
            else if ((clusterId == 4) && path_script.Search3(StartNode, clusterPoint, 1))
            {
                count++;
                cluster4ObjectsList.Add(clusterPoint);
            }
            else if ((clusterId == 5) && path_script.Search3(StartNode, clusterPoint, 1))
            {
                count++;
                cluster5ObjectsList.Add(clusterPoint);
            }
        }
    }

    public void createPickableObjects(int numberOfObjects)
    {
        for (int i = 0; i < cluster1ObjectsList.Count; i++)
        {
            int randomVal = UnityEngine.Random.Range(0, 2);
            double points = System.Math.Round(numberOfObjects * ((double)50 / 100));
            if (i < points)
            {
                Instantiate(coin, new Vector3(cluster1ObjectsList[i].x + 0.5f, cluster1ObjectsList[i].y + 2.4f, cluster1ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
            else 
            {
                if (randomVal == 0)
                {
                    Instantiate(knife, new Vector3(cluster1ObjectsList[i].x + 0.5f, cluster1ObjectsList[i].y + 2.4f, cluster1ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(gun, new Vector3(cluster1ObjectsList[i].x + 0.5f, cluster1ObjectsList[i].y + 2.4f, cluster1ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
            }
        }

        for (int i = 0; i < cluster2ObjectsList.Count; i++)
        {
            int randomVal = UnityEngine.Random.Range(0, 2);
            double points = System.Math.Round(numberOfObjects * ((double)40 / 100));
            double weapons = System.Math.Round(numberOfObjects * ((double)30 / 100)) + points;
            if (i < points)
            {
                if (randomVal == 0)
                {
                    Instantiate(coin, new Vector3(cluster2ObjectsList[i].x + 0.5f, cluster2ObjectsList[i].y + 2.4f, cluster2ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(cash, new Vector3(cluster2ObjectsList[i].x + 0.5f, cluster2ObjectsList[i].y + 2.4f, cluster2ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
            }else if (i < weapons)
            {
                Instantiate(gun, new Vector3(cluster2ObjectsList[i].x + 0.5f, cluster2ObjectsList[i].y + 2.4f, cluster2ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
            else
            {
                Instantiate(bandages, new Vector3(cluster2ObjectsList[i].x + 0.5f, cluster2ObjectsList[i].y + 2.4f, cluster2ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
        }

        for (int i = 0; i < cluster3ObjectsList.Count; i++)
        {
            int randomVal = UnityEngine.Random.Range(0, 2);
            double points = System.Math.Round(numberOfObjects * ((double)20 / 100));
            double weapons = System.Math.Round(numberOfObjects * ((double)20 / 100)) + points;
            double powers = System.Math.Round(numberOfObjects * ((double)20 / 100)) + weapons;
            if (i < points)
            {
                Instantiate(cash, new Vector3(cluster3ObjectsList[i].x + 0.5f, cluster3ObjectsList[i].y + 2.4f, cluster3ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
            else if (i < weapons)
            {
                if(randomVal == 0)
                {
                    Instantiate(knife, new Vector3(cluster3ObjectsList[i].x + 0.5f, cluster3ObjectsList[i].y + 2.4f, cluster3ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(gun, new Vector3(cluster3ObjectsList[i].x + 0.5f, cluster3ObjectsList[i].y + 2.4f, cluster3ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
                
            }
            else if (i < powers)
            {
                Instantiate(superSpeed, new Vector3(cluster3ObjectsList[i].x + 0.5f, cluster3ObjectsList[i].y + 2.4f, cluster3ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
            else
            {
                Instantiate(medKit, new Vector3(cluster3ObjectsList[i].x + 0.5f, cluster3ObjectsList[i].y + 2.4f, cluster3ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
        }

        for (int i = 0; i < cluster4ObjectsList.Count; i++)
        {
            int randomVal = UnityEngine.Random.Range(0, 2);
            double points = System.Math.Round(numberOfObjects * ((double)20 / 100));
            double weapons = System.Math.Round(numberOfObjects * ((double)30 / 100)) + points;
            double powers = System.Math.Round(numberOfObjects * ((double)30 / 100)) + weapons;
            if (i < points)
            {
                Instantiate(cash, new Vector3(cluster4ObjectsList[i].x + 0.5f, cluster4ObjectsList[i].y + 2.4f, cluster4ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
            else if (i < weapons)
            {
                if (randomVal == 0)
                {
                    Instantiate(knife, new Vector3(cluster4ObjectsList[i].x + 0.5f, cluster4ObjectsList[i].y + 2.4f, cluster4ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(gun, new Vector3(cluster4ObjectsList[i].x + 0.5f, cluster4ObjectsList[i].y + 2.4f, cluster4ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
            }
            else if (i < powers)
            {
                Instantiate(invisibility, new Vector3(cluster4ObjectsList[i].x + 0.5f, cluster4ObjectsList[i].y + 2.4f, cluster4ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
            else
            {
                if (randomVal == 0)
                {
                    Instantiate(medKit, new Vector3(cluster4ObjectsList[i].x + 0.5f, cluster4ObjectsList[i].y + 2.4f, cluster4ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(bandages, new Vector3(cluster4ObjectsList[i].x + 0.5f, cluster4ObjectsList[i].y + 2.4f, cluster4ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
            }
        }

        for (int i = 0; i < cluster5ObjectsList.Count; i++)
        {
            int randomVal = UnityEngine.Random.Range(0, 2);
            double weapons = System.Math.Round(numberOfObjects * ((double)30 / 100));
            double powers = System.Math.Round(numberOfObjects * ((double)20 / 100)) + weapons;
            if (i < weapons)
            {
                Instantiate(gun, new Vector3(cluster5ObjectsList[i].x + 0.5f, cluster5ObjectsList[i].y + 2.4f, cluster5ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
            }
            else if (i < powers)
            {
                if (randomVal == 0)
                {
                    Instantiate(superSpeed, new Vector3(cluster5ObjectsList[i].x + 0.5f, cluster5ObjectsList[i].y + 2.4f, cluster5ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(invisibility, new Vector3(cluster5ObjectsList[i].x + 0.5f, cluster5ObjectsList[i].y + 2.4f, cluster5ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
            }
            else
            {
                if (randomVal == 0)
                {
                    Instantiate(medKit, new Vector3(cluster5ObjectsList[i].x + 0.5f, cluster5ObjectsList[i].y + 2.4f, cluster5ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(bandages, new Vector3(cluster5ObjectsList[i].x + 0.5f, cluster5ObjectsList[i].y + 2.4f, cluster5ObjectsList[i].z + 0.5f), Quaternion.identity, transform);
                }
            }
        }
    }

}

