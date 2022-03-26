using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileGrid : MonoBehaviour
{
    public GameObject accessTerrain;
    private terrain terrain_script;

    public int Width;
    public int Depth;
    private float Height;

    public GameObject WalkableTile;
    public List<Vector3> WalkableCells;
    public HashSet<Vector3> WalkableCellsHash;

    public List<GameObject> vertices;

    public List<GameObject> WalkableCellsObjects;

    // Start is called before the first frame update
    public void start()
    {
        terrain_script = accessTerrain.GetComponent<terrain>();
        Width = terrain_script.width;
        Depth = terrain_script.depth;
        vertices = terrain_script.verticesList;
        WalkableCellsHash = new HashSet<Vector3>();

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        ClearLists();

        // Place Walkable tiles
        for (int z = 0; z < Depth; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                getHeight(new Vector3(x, 0, z));
                //if not occupied Instantiate walkableTile
                if (Height < 2.1f)
                {
                    var tile = Instantiate(WalkableTile, new Vector3(x, Height, z), Quaternion.identity, transform);
                    //add walkable tile to the WalkableCells list
                    WalkableCellsObjects.Add(tile);
                    WalkableCells.Add(tile.transform.position);
                    WalkableCellsHash.Add(new Vector3(tile.transform.position.x, 0, tile.transform.position.z));
                }
            }
        }
    }

    private void ClearLists()
    {
        WalkableCells.Clear();
        WalkableCellsHash.Clear();
    }

    private void getHeight(Vector3 position)
    {
        Height = 0;
        foreach (var item in vertices)
        {
            if (item.transform.position.x == position.x && item.transform.position.z == position.z)
            {
                Height = item.transform.position.y;
            }
        }
    }
}
