using Priority_Queue;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class pathFinding : MonoBehaviour
{
    public GameObject accessPositions;
    private positionDetails position_script;

    public GameObject accessGrid;
    private tileGrid grid_script;

    SimplePriorityQueue<Vector3, int> priorityQueue;

    HashSet<Vector3> visited;

    public List<Vector3> PlayerReachableCells;

    Dictionary<Vector3, Vector3> cellParents;

    private List<Dictionary<Vector3, Vector3>> cellParentsList = new List<Dictionary<Vector3, Vector3>>();
    public List<Dictionary<Vector3, Vector3>> enemyParentsList = new List<Dictionary<Vector3, Vector3>>();

    private int Width;
    private int Depth;

    public GameObject accessPathVisualize;
    private visualizePaths pathVisualize_script;

    public Transform PathCells;
    public GameObject PathPrefab;

    public bool Search3(Vector3 StartNode, Vector3 endPoint, int option)
    {
        position_script = accessPositions.GetComponent<positionDetails>();
        grid_script = accessGrid.GetComponent<tileGrid>();

        Width = grid_script.Width;
        Depth = grid_script.Depth;

        priorityQueue = new SimplePriorityQueue<Vector3, int>();
        visited = new HashSet<Vector3>();
        cellParents = new Dictionary<Vector3, Vector3>();
        
        var endPointNew = new Vector3(endPoint.x, 0, endPoint.z);
        ClearLists();

        Dictionary<Vector3, int> distances = position_script.WalkableCellsHash.ToDictionary(x => x, x => int.MaxValue);

        distances[StartNode] = 0;

        priorityQueue.Enqueue(StartNode, 0);
        visited.Add(StartNode);

        while (priorityQueue.Count > 0)
        {

            var currentCell = priorityQueue.Dequeue();

            if (currentCell.x.Equals(endPointNew.x) && currentCell.z.Equals(endPointNew.z) && option == 1)
            {
                PlayerReachableCells.Add(endPointNew);
                cellParentsList.Add(cellParents);
                return true;
            } else if (currentCell.x.Equals(endPointNew.x) && currentCell.z.Equals(endPointNew.z) && option == 2)
            {
                enemyParentsList.Add(cellParents);
                return true;
            }

            var neighbours = GetNeighbours(currentCell);

            foreach (Vector3 neighbour in neighbours)
            {
                if (!visited.Contains(neighbour))
                {
                    int dist = distances[currentCell] + 1;

                    if (dist < distances[neighbour])
                    {
                        distances[neighbour] = dist;

                        var fScore = distances[neighbour] + DistanceEstimate(neighbour, endPointNew);

                        priorityQueue.Enqueue(neighbour, fScore);
                        visited.Add(neighbour);
                        cellParents[neighbour] = currentCell;
                    }
                }
            }
        }

        return false;

    }

    private int DistanceEstimate(Vector3 node, Vector3 endPointNew)
    {
        var x = Mathf.Pow(node.x - endPointNew.x, 2);
        var y = Mathf.Pow(node.y - endPointNew.y, 2);
        var z = Mathf.Pow(node.z - endPointNew.z, 2);

        return (int)Mathf.Sqrt(x + y + z);
    }

    private void ClearLists()
    {
        priorityQueue.Clear();
        visited.Clear();
        cellParents.Clear();
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

        var walkableNeighbours = new List<Vector3>();
        foreach (var neighbour in neighbours)
        {
            if (IsCellFree(neighbour))
            {
                walkableNeighbours.Add(neighbour);
            }

        }
        return walkableNeighbours;
    }

    private bool IsCellFree(Vector3 position)
    {
        var newPosition = new Vector3(position.x, 0, position.z);

        if (position_script.WalkableCellsHash.Contains(newPosition))
        {
            return true;
        }

        return false;
    }

    public void pathVisualize(Vector3 StartNode)
    {
        pathVisualize_script = accessPathVisualize.GetComponent<visualizePaths>();

        for (int i=0; i<cellParentsList.Count; i++)
        {
            pathVisualize_script.VisualizePath(cellParentsList[i], StartNode, PlayerReachableCells[i], PathPrefab, PathCells);
        }
    }
}
