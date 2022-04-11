using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visualizePaths : MonoBehaviour
{
    public void VisualizePath(Dictionary<Vector3, Vector3> cellParents, Vector3 StartPosition, Vector3 EndPosition, GameObject PathPrefab, Transform PathCells)
    {
        var path = new List<Vector3>();
        var current = cellParents[EndPosition];

        path.Add(EndPosition);

        while (current != StartPosition)
        {
            path.Add(current);
            current = cellParents[current];
        }

        for (int i = 1; i < path.Count; i++)
        {
            var pathCellPosition = path[i];
            pathCellPosition.y = PathPrefab.transform.position.y;
            Instantiate(PathPrefab, pathCellPosition, Quaternion.identity, PathCells);
        }
    }
}
