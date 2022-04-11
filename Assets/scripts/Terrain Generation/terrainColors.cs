using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainColors : MonoBehaviour
{
    public typeOfTerrain[] terrainRegions;
    public Color[] colorMap;
    public Renderer textureRenderer;

    public Texture2D colorMapTexture(Color[] colorMap, int width, int depth)
    {
        Texture2D texture = new Texture2D(width, depth);
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public void drawTexture2D(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
    }

    public void addColors(int width, int depth, Vector3[] vertices)
    {
        colorMap = new Color[(width + 1) * (depth + 1)];
        for (int i = 0; i < vertices.Length; i++)
        {
            float curHeight = vertices[i].y;
            for(int j=0; j<terrainRegions.Length; j++)
            {
                if (curHeight <= terrainRegions[j].height)
                {
                    colorMap[(int)(vertices[i].z * width + vertices[i].x)] = terrainRegions[j].color;
                    break;
                }
            }
        }

        drawTexture2D(colorMapTexture(colorMap, width, depth));
    }
}

[System.Serializable]
public struct typeOfTerrain
{
    public string name;
    public float height;
    public Color color;
}