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
    public GameObject Trees;
    public GameObject Grass;
    public GameObject Rocks;
    public List<GameObject> WalkableCells;
    public HashSet<Vector3> WalkableCellsHash;

    public Vector3[] vertices;

    public bool displayTiles = false;

    public void start()
    {
        terrain_script = accessTerrain.GetComponent<terrain>();
        Width = terrain_script.width;
        Depth = terrain_script.depth;
        vertices = terrain_script.vertices;
        WalkableCellsHash = new HashSet<Vector3>();
    }

    public void GenerateGrid()
    {
        start();
        ClearLists();

        // Place Walkable tiles
        for (int z = 0; z < Depth; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                getHeight(new Vector3(x, 0, z));
                //if not occupied Instantiate walkableTile
                if (Height < 2f && Height > 1f)
                {
                    if (displayTiles)
                    {
                        var tile = Instantiate(WalkableTile, new Vector3(x + 0.5f, Height + 0.5f, z + 0.5f), Quaternion.identity, transform);
                        WalkableCells.Add(tile);
                    }
                    WalkableCellsHash.Add(new Vector3(x, 0, z));
                }
                
            }
        }
    }

    public void GenerateVegetation()
    {
        start();
        for (int z = 0; z < Depth; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                int randomVal = UnityEngine.Random.Range(0, 5);
                getHeight(new Vector3(x, 0, z));
                if (Height < 2f && Height > 1.5f)
                {
                    if(randomVal < 3)
                    {
                        Instantiate(Trees, new Vector3(x + 0.5f, Height, z + 0.5f), Quaternion.identity, transform);
                    }
                }
                if (Height < 2f && Height > 1.15f)
                {
                    Instantiate(Grass, new Vector3(x + 0.5f, Height, z + 0.5f), Quaternion.identity, transform);
                }
                if (Height < 2.5f && Height > 1.8f)
                {
                    if (randomVal < 2)
                    {
                        Instantiate(Rocks, new Vector3(x + 0.5f, Height - 0.1f, z + 0.5f), Quaternion.identity, transform);
                    }
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
            if (item.x == position.x && item.z == position.z)
            {
                Height = item.y;
            }
        }
    }
}
