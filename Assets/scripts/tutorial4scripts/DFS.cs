using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFS : MonoBehaviour
{
    public GridGenerator GridData;
    private List<Vector3> visitedNodes;
    private Stack<Vector3> stack;
    // Keep track of visited notes + which nodes did we get from
    // Necessary later for building the path
    IDictionary<Vector3, Vector3> nodeParents;
    // Start is called before the first frame update
    void Start()
    {
        visitedNodes = new List<Vector3>();
        stack = new Stack<Vector3>();
        nodeParents = new Dictionary<Vector3, Vector3>();
    }

    public void StartDFS()
    {
        // Necessary when generating a new grid and starting a new search
        ClearLists();

        uint nodeVisitCount = 0;
        float timeNow = Time.realtimeSinceStartup;

        stack.Push(GridData.StartNode);
        visitedNodes.Add(GridData.StartNode);

        while (stack.Count != 0)
        {
            var currentNode = stack.Pop();
            nodeVisitCount++;

            if (currentNode == GridData.GoalNode)
            {
                print("DFS time: " + (Time.realtimeSinceStartup - timeNow).ToString());
                print(string.Format("DFS visits: {0} ({1:F2}%)", nodeVisitCount, (nodeVisitCount / (double)GridData.WalkableCells.Count) * 100));
                GridData.BuildPath(nodeParents);
                return;
            }

            var nearbyNodes = GridData.GetWalkableNodes(currentNode);
            foreach (var item in nearbyNodes)
            {
                if(!visitedNodes.Contains(item))
                {
                    stack.Push(item);
                    visitedNodes.Add(item);
                    nodeParents.Add(item, currentNode);
                }
            }
        }

        GridData.BuildPath(nodeParents);
    }

    private void ClearLists()
    {
        visitedNodes.Clear();
        stack.Clear();
        nodeParents.Clear();
    }
}
