using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placeEnemies : MonoBehaviour
{
    public GameObject accessPlaceObjects;
    private placeObjects place_script;

    public GameObject accessPathf;
    private pathFinding path_script;

    List<List<Vector3>> randomClusters = new List<List<Vector3>>();
    private List<Vector3> cluster5ObjectsList = new List<Vector3>();
    public HashSet<Vector3> placableCellsHash = new HashSet<Vector3>();
    public List<Vector3> startNode = new List<Vector3>();
    public List<Vector3> endNode = new List<Vector3>();

    public GameObject playerInstance;

    public GameObject PlaceEnemies(int numberOfEnimies, GameObject player)
    {
        place_script = accessPlaceObjects.GetComponent<placeObjects>();
        path_script = accessPathf.GetComponent<pathFinding>();

        randomClusters = place_script.randomClusters;
        cluster5ObjectsList = place_script.cluster5ObjectsList;
        int count = 0;

        foreach (var vertice in randomClusters[4])
        {
            if ((!cluster5ObjectsList.Contains(vertice)) && count < numberOfEnimies)
            {
                startNode.Add(new Vector3(vertice.x, vertice.y + 6f, vertice.z));
                playerInstance = Instantiate(player, new Vector3(vertice.x, vertice.y + 6f, vertice.z), Quaternion.identity, transform);
                count++;
            } 
            else
            {
                placableCellsHash.Add(vertice);
            }
        }
        for (int i=0; i<startNode.Count; i++)
        {
            foreach (var vertice in placableCellsHash)
            {
                if (path_script.Search3(startNode[i], vertice, 2))
                {
                    endNode.Add(vertice);
                    break;
                }
            }
        }

        return playerInstance;
    }

    public List<Vector3> pathDetails(Dictionary<Vector3, Vector3> cellParents, Vector3 StartPosition, Vector3 EndPosition)
    {
        var path = new List<Vector3>();
        var current = cellParents[EndPosition];

        path.Add(new Vector3(EndPosition.x, EndPosition.y + 6f, EndPosition.z));

        while (current != StartPosition)
        {
            path.Add(new Vector3(current.x, current.y + 6f, current.z));
            current = cellParents[current];
        }

        return path;
    }
}
