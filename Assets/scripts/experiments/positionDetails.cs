using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionDetails : MonoBehaviour
{
    public GameObject accessGrid;
    private tileGrid grid_script;

    public Vector3 StartNode;

    public List<Vector3> WalkableCells;

    public HashSet<Vector3> WalkableCellsHash;

    private List<Vector3> newWalkableCellsList = new List<Vector3>();

    public List<List<Vector3>> ClustersList = new List<List<Vector3>>();

    public List<Vector3> Cluster1points = new List<Vector3>();
    public List<Vector3> Cluster2points = new List<Vector3>();
    public List<Vector3> Cluster3points = new List<Vector3>();
    public List<Vector3> Cluster4points = new List<Vector3>();

    public List<GameObject> WalkableCellsObjects;

    private Vector3 newPoint;

    public Vector3 getStartNode()
    {
        grid_script = accessGrid.GetComponent<tileGrid>();
        WalkableCells = grid_script.WalkableCells;
        WalkableCellsHash = grid_script.WalkableCellsHash;
        newWalkableCellsList = newWalkableCells();
        WalkableCellsObjects = grid_script.WalkableCellsObjects;

        Color newColor1 = new Color(0.91f, 0.21f, 0.81f);        //pink
        Color newColor2 = new Color(0.24f, 0.21f, 0.91f);        //blue
        Color newColor3 = new Color(0.1f, 0.1f, 0.1f);           //green
        Color newColor4 = new Color(0.91f, 0.29f, 0.21f);        //red

        foreach (var vertice in grid_script.vertices)
        {
            if (vertice.GetComponent<MeshRenderer>().material.color == newColor1)
            {
                newPoint = new Vector3(vertice.transform.position.x, 0, vertice.transform.position.z);
                Cluster1points.Add(newPoint);
            }else if (vertice.GetComponent<MeshRenderer>().material.color == newColor2)
            {
                newPoint = new Vector3(vertice.transform.position.x, 0, vertice.transform.position.z);
                Cluster2points.Add(newPoint);
            }
            else if (vertice.GetComponent<MeshRenderer>().material.color == newColor3)
            {
                newPoint = new Vector3(vertice.transform.position.x, 0, vertice.transform.position.z);
                Cluster3points.Add(newPoint);
            }
            else if (vertice.GetComponent<MeshRenderer>().material.color == newColor4)
            {
                newPoint = new Vector3(vertice.transform.position.x, 0, vertice.transform.position.z);
                Cluster4points.Add(newPoint);
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
    }


    public List<Vector3> GetNeighbours(Vector3 currentCell)
    {
        var neighbours = new List<Vector3>()
        {
            new Vector3(currentCell.x - 1, 0, currentCell.z), // Up
            new Vector3(currentCell.x + 1, 0, currentCell.z), // Down
            new Vector3(currentCell.x, 0, currentCell.z - 1), // Left
            new Vector3(currentCell.x, 0, currentCell.z + 1), // Right
        };

        //Debug.Log(neighbours.Count + ": neighbour count in getNeighbours method");

        var walkableNeighbours = new List<Vector3>();
        foreach (var neighbour in neighbours)
        {
            //Debug.Log(neighbour + ": neighbour inside method and inside foreach");
            if (IsInLevelBounds(neighbour) && IsCellFree(neighbour))
            {
                walkableNeighbours.Add(neighbour);
            }
                
        }

        //Debug.Log(walkableNeighbours.Count + ": walkable neighbour list count");

        return walkableNeighbours;
    }

    private bool IsCellFree(Vector3 position)
    {
        //Debug.Log("inside isCellOccupied");
        //Debug.Log(WalkableCells.Count + "walkablecell count");
        //Debug.Log(WalkableCells[1] + "walkablecell count");

        var newPosition = new Vector3(position.x, 0, position.z);

        if (newWalkableCellsList.Contains(newPosition))
        {
            return true;
        }
        
        return false;
    }

    private List<Vector3> newWalkableCells()
    {
        foreach (Vector3 cell in grid_script.WalkableCells)
        {
            var newCell = new Vector3(cell.x, 0, cell.z);
            newWalkableCellsList.Add(newCell);
        }

        return newWalkableCellsList;
    }

    private bool IsInLevelBounds(Vector3 neighbour)
    {
        //Debug.Log("inside IsInLevelBounds");
        if (neighbour.x > 0 && neighbour.x <= grid_script.Width - 1 && neighbour.z > 0 && neighbour.z <= grid_script.Depth - 1)
        {
            return true;
        }

        return false;
    }

}
