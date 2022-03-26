using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainGenerator1 : MonoBehaviour
{
    public int width = 20;
    public int depth = 20;
    public GameObject vertexPrefab;
    public bool shouldVisualizeVertices;
    public Gradient gradient;
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;
    Vector2[] uvs;
    MeshFilter meshFilter;
    Mesh mesh;
    float minHeight;
    float maxHeight;

    void Start()
    {
        initializeMesh();
        createMesh();
        updateMesh();

        if (shouldVisualizeVertices)
            DrawVertice();
    }


    private void initializeMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "Procedural Mesh";
        //update the MeshFilter with empty mesh
        meshFilter.mesh = mesh;
    }

    private void DrawVertice()
    {
        for (int k = 0; k < vertices.Length; k++)
        {
            Instantiate(vertexPrefab, vertices[k], Quaternion.identity, transform);
        }
    }


    private void updateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }


    private void createMesh()
    {
        createVertices();
        createTriangles();
        createColors();
        createUVs();
    }

    private void createUVs()
    {
        uvs = new Vector2[vertices.Length];
        var vertexIndex = 0;
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                //uvs are used to map textures to the mesh
                //normalize the texture values
                //uvs range is from 0 to 1
                uvs[vertexIndex] = new Vector2((float)x / width, (float)z / depth);
                vertexIndex++;
            }
        }
    }

    private void createColors()
    {
        colors = new Color[vertices.Length];
        var currentVertexIndex = 0;
        //get the gradient based on the height
        //a and b is like min and max. Output is a normalize value
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                var height = Mathf.InverseLerp(minHeight, maxHeight, vertices[currentVertexIndex].y); // get the normalize value between min and max
                colors[currentVertexIndex] = gradient.Evaluate(height);
                currentVertexIndex++;
            }
        }
    }

    private void createTriangles()
    {
        triangles = new int[width * depth * 6];
        int currentVertexPoint = 0;
        int currentTrianglePoint = 0;
        //clock-wise because we do not want to render the bottom part of the vertices
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[currentTrianglePoint + 0] = currentVertexPoint + 0;
                triangles[currentTrianglePoint + 1] = currentVertexPoint + width + 1;
                triangles[currentTrianglePoint + 2] = currentVertexPoint + 1;

                triangles[currentTrianglePoint + 3] = currentVertexPoint + 1;
                triangles[currentTrianglePoint + 4] = currentVertexPoint + width + 1;
                triangles[currentTrianglePoint + 5] = currentVertexPoint + width + 2;

                currentVertexPoint++;
                currentTrianglePoint += 6;
            }
            currentVertexPoint++;
        }

        /*triangles[0] = 0;
        triangles[1] = width + 1;
        triangles[2] = 1;*/
    }

    private void createVertices()
    {
        vertices = new Vector3[(width + 1) * (depth + 1)];
        int vertexIndex = 0; //keep track of current index
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = Mathf.PerlinNoise(x * 0.4f, z * 0.4f) * 3f; // 0 to 1
            
                vertices[vertexIndex] = new Vector3(x, y, z); // y value will be updated with perlin noise

                if (y > maxHeight)
                {
                    maxHeight = y;
                }

                if (y < minHeight)
                {
                    minHeight = y;
                }
                vertexIndex++;
            }
        }
    }
}
