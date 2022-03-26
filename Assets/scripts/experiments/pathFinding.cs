using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class pathFinding : MonoBehaviour
{
    public GameObject accessPositions;
    private positionDetails position_script;

    Stack<Vector3> stack;

    SimplePriorityQueue<Vector3, int> priorityQueue;  //for Astar

    HashSet<Vector3> visited;

    public List<Vector3> PlayerReachableCells;

    // Keep track of visited notes + which nodes did we get from
    // Necessary later for building the path
    Dictionary<Vector3, Vector3> cellParents;

    /*public void Search(Vector3 StartNode)
    {
        position_script = accessPositions.GetComponent<positionDetails>();
        stack = new Stack<Vector3>();
        visited = new HashSet<Vector3>();
        cellParents = new Dictionary<Vector3, Vector3>();

        foreach (var endPoint in position_script.WalkableCells)
        {
            var endPointNew = new Vector3(endPoint.x, endPoint.y, endPoint.z);
            ClearData();

            stack.Push(StartNode);
            visited.Add(StartNode);

            while (stack.Count > 0)
            {
                var currentCell = stack.Pop();
                //Debug.Log(StartNode + ": starting node");
                //Debug.Log(currentCell + ": current position");

                if (currentCell.x.Equals(endPointNew.x) && currentCell.z.Equals(endPointNew.z))
                {
                    //Debug.Log(endPointNew + ": Destination reached!");
                    PlayerReachableCells.Add(endPointNew);
                    break;
                }

                var neighbours = position_script.GetNeighbours(currentCell);
                //Debug.Log(neighbours.Count + ": neighbours");
                foreach (var neighbour in neighbours)
                {
                    //Debug.Log(neighbour + ": Neighbour");
                    if (!visited.Contains(neighbour))
                    {
                        stack.Push(neighbour);
                        visited.Add(neighbour);
                        cellParents[neighbour] = currentCell;
                    }
                }
            }

            //Debug.Log(endPointNew + ": Could not reach the destination.");
        }
    }*/

    /*public void Search2(Vector3 StartNode)
    {
        position_script = accessPositions.GetComponent<positionDetails>();
        priorityQueue = new SimplePriorityQueue<Vector3, int>();
        visited = new HashSet<Vector3>();
        cellParents = new Dictionary<Vector3, Vector3>();

        foreach (var endPoint in position_script.ThreeClusterpoints)
        {
            var endPointNew = new Vector3(endPoint.x, 0, endPoint.z);
            ClearLists();

            // A list of all nodes that are walkable, initialized to have infinity distance from start
            // g(x)
            Dictionary<Vector3, int> distances = position_script.WalkableCellsHash.ToDictionary(x => x, x => int.MaxValue);

            // The distance from the start to itself is 0
            distances[StartNode] = 0;

            priorityQueue.Enqueue(StartNode, 0);
            visited.Add(StartNode);

            while (priorityQueue.Count > 0)
            {

                var currentCell = priorityQueue.Dequeue();

                if (currentCell.x.Equals(endPointNew.x) && currentCell.z.Equals(endPointNew.z))
                {
                    // If the goal position is the lowest position in the priority queue then there are
                    //    no other nodes that could possibly have a shorter path.
                    //Debug.Log("Destination reached!");
                    PlayerReachableCells.Add(endPointNew);
                    break;
                }

                var neighbours = position_script.GetNeighbours(currentCell);
                //Debug.Log("Before the foreach neighboure");

                foreach (Vector3 neighbour in neighbours)
                {
                    //Debug.Log(position_script.WalkableCells.Count + ": number of cells");
                    //Debug.Log(distances.Count + ": number of distances");
                    //Debug.Log(currentCell);

                    if (!visited.Contains(neighbour))
                    {
                        int dist = distances[currentCell] + 1;

                        //If the distance to the parent, PLUS the distance added by the neighbor,
                        //is less than the current distance to the neighbor,
                        //update the neighbor's parent to curr as well as its shortest distance
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

            //Debug.Log("Could not reach the destination.");
        }
        
    }*/

    public bool Search3(Vector3 StartNode, Vector3 endPoint)
    {
        position_script = accessPositions.GetComponent<positionDetails>();
        priorityQueue = new SimplePriorityQueue<Vector3, int>();
        visited = new HashSet<Vector3>();
        cellParents = new Dictionary<Vector3, Vector3>();

        
        var endPointNew = new Vector3(endPoint.x, 0, endPoint.z);
        ClearLists();

        // A list of all nodes that are walkable, initialized to have infinity distance from start
        // g(x)
        Dictionary<Vector3, int> distances = position_script.WalkableCellsHash.ToDictionary(x => x, x => int.MaxValue);

        // The distance from the start to itself is 0
        distances[StartNode] = 0;

        priorityQueue.Enqueue(StartNode, 0);
        visited.Add(StartNode);

        while (priorityQueue.Count > 0)
        {

            var currentCell = priorityQueue.Dequeue();

            if (currentCell.x.Equals(endPointNew.x) && currentCell.z.Equals(endPointNew.z))
            {
                // If the goal position is the lowest position in the priority queue then there are
                //    no other nodes that could possibly have a shorter path.
                //Debug.Log("Destination reached!");
                PlayerReachableCells.Add(endPointNew);
                return true;
            }

            var neighbours = position_script.GetNeighbours(currentCell);
            //Debug.Log("Before the foreach neighboure");

            foreach (Vector3 neighbour in neighbours)
            {
                //Debug.Log(position_script.WalkableCells.Count + ": number of cells");
                //Debug.Log(distances.Count + ": number of distances");
                //Debug.Log(currentCell);

                if (!visited.Contains(neighbour))
                {
                    int dist = distances[currentCell] + 1;

                    //If the distance to the parent, PLUS the distance added by the neighbor,
                    //is less than the current distance to the neighbor,
                    //update the neighbor's parent to curr as well as its shortest distance
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
        //Debug.Log("Could not reach the destination.");
        

    }

    private int DistanceEstimate(Vector3 node, Vector3 endPointNew)
    {
        var x = Mathf.Pow(node.x - endPointNew.x, 2);
        var y = Mathf.Pow(node.y - endPointNew.y, 2);
        var z = Mathf.Pow(node.z - endPointNew.z, 2);

        return (int)Mathf.Sqrt(x + y + z);
    }

    public void printValue()
    {
        foreach(var pos in PlayerReachableCells)
        {
            foreach(var posNext in position_script.WalkableCellsObjects)
            {
                if (posNext.transform.position.x.Equals(pos.x) && posNext.transform.position.z.Equals(pos.z))
                {
                    posNext.GetComponent<MeshRenderer>().material.color = new Color(0.91f, 0.29f, 0.21f);
                }
            }
        }
        //Debug.Log(PlayerReachableCells.Count + ": player reachable cells  -  " + position_script.WalkableCells.Count + ": total walkable cells");
    }

    private void ClearData()
    {
        stack.Clear();
        visited.Clear();
        cellParents.Clear();
    }

    private void ClearLists()
    {
        priorityQueue.Clear();
        visited.Clear();
        cellParents.Clear();
    }
}
