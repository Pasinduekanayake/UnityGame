using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int Width;
    public int Height;
    public int NumberOfObstacles;
    public GameObject Player;
    public GameObject Goal;
    public GameObject Obstacle;
    [HideInInspector]
    public List<Vector3> Obstacles;
    [HideInInspector]
    public List<Vector3> WalkableCells;
    [HideInInspector]
    public Vector3 StartNode;
    [HideInInspector]
    public Vector3 GoalNode;
    [Header("Debugging")]
    public GameObject WalkableTile;
    public GameObject PathTile;

    // Start is called before the first frame update
    void Start()
    {
        Obstacles = new List<Vector3>();
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        //Clear all lists
        ClearLists();

        // Place Obstacles
        int obstaclesToCreate = NumberOfObstacles;
        //Place given number of obstacles in the grid
        while (obstaclesToCreate > 0)
        {
            //Call CreateAtRandomPosition function to Instantiate obstacles
            var obstacle = CreateAtRandomPosition(Obstacle, 0.5f);
            if (obstacle != null)
            {
                //Add obstacles position details to the Obstacles list
                Obstacles.Add(obstacle.transform.position);
                obstaclesToCreate--;
            }
        }

        // Place Walkable tiles
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                //Check whether the cell is occupied or not
                if (!IsCellOccupied(new Vector3(x, 0, z)))
                {
                    //if not occupied Instantiate walkableTile
                    var tile = Instantiate(WalkableTile, new Vector3(x, 0.05f, z), Quaternion.identity, transform);
                    //add walkable tile to the WalkableCells list
                    WalkableCells.Add(tile.transform.position);
                }
            }
        }

        // Place Player and Goal
        GameObject startGameObject;
        while ((startGameObject = CreateAtRandomPosition(Player, 1f)) == null) ;
        StartNode = startGameObject.transform.position;

        GameObject goalGameObject;
        while ((goalGameObject = CreateAtRandomPosition(Goal, 0.5f)) == null) ;
        GoalNode = goalGameObject.transform.position;
    }

    private void ClearLists()
    {
        DeleteAllChildren(transform);
        Obstacles.Clear();
        WalkableCells.Clear();
    }

    public IList<Vector3> GetWalkableNodes(Vector3 curr)
    {

        IList<Vector3> walkableNodes = new List<Vector3>();

        IList<Vector3> possibleNodes = new List<Vector3>() {
            new Vector3 (curr.x + 1, 0, curr.z),
            new Vector3 (curr.x - 1, 0, curr.z),
            new Vector3 (curr.x, 0, curr.z + 1),
            new Vector3 (curr.x, 0, curr.z - 1),
            new Vector3 (curr.x + 1, 0, curr.z + 1),
            new Vector3 (curr.x + 1, 0, curr.z - 1),
            new Vector3 (curr.x - 1, 0, curr.z + 1),
            new Vector3 (curr.x - 1, 0, curr.z - 1)
        };

        foreach (Vector3 node in possibleNodes)
        {
            if (!IsCellOccupied(node) && (node.x >= 0 && node.x <= Width - 1) && (node.z >= 0 && node.z <= Height - 1))
            {
                //Add the node to the walkableNodes list
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }

    public void BuildPath(IDictionary<Vector3, Vector3> nodeParents)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curr = GoalNode;
        while (curr != StartNode)
        {
            path.Add(curr);
            curr = nodeParents[curr];
        }

        ShowPath(path);
    }
    private void ShowPath(List<Vector3> nodes)
    {
        for (int i = nodes.Count - 1; i > 0; i--)
        {
            var newGameObject = Instantiate(PathTile, new Vector3(nodes[i].x, 0.1f, nodes[i].z), Quaternion.identity, transform);
        }
    }

    private GameObject CreateAtRandomPosition(GameObject prefab, float PositionY)
    {
        //Calculate Xposition using Random.Range hint: 0 to width
        //Calculate Yposition using Random.Range hint: 0 to height
        var positionX = UnityEngine.Random.Range(0, Width);
        var positionZ = UnityEngine.Random.Range(0, Height); ;

        //Check wherher the location is already occupied or not
        if (IsCellOccupied(new Vector3(positionX, 0, positionZ)))
        {
            Debug.Log("CELL OCCUPIED - Recreating: " + prefab.name);
            //return null
            return null;
        }
        else
        {
            //If the cell is not occupied place the prefab at calculated position
            var newGameObject = Instantiate(prefab, new Vector3(positionX, PositionY, positionZ), Quaternion.identity, transform);
            //return newGameObject
            return newGameObject;
        }
    }

    private void DeleteAllChildren(Transform parentGameObject)
    {
        foreach (Transform item in parentGameObject)
        {
            Destroy(item.gameObject);
        }
    }

    private bool IsCellOccupied(Vector3 position)
    {
        foreach (var item in Obstacles)
        {
            if (item.x == position.x && item.z == position.z)
            {
                return true;
            }
        }
        return false;
    }
}
