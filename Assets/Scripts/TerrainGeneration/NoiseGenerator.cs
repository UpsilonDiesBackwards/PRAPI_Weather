using System;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour {
    public int width, height;

    public int xOrg, yOrg;

    public float strength = 0.5f;
    public float scale = 1.0f;

    public float detailStrength = 0.2f;
    public float detailScale = 4.0f;
    
    private Terrain _terrain;
    private TerrainData _terrainData;

    private void Start() {
        _terrain = GetComponent<Terrain>();
        _terrainData = _terrain.terrainData;

        if (_terrainData.heightmapResolution != width + 1 || _terrainData.heightmapResolution != height + 1) {
            Debug.LogError($"Terrain heightmap resolution must be {width + 1}x{height + 1}. " +
                           $"Current resolution is {_terrainData.heightmapResolution}.");
            return;
        }
        
        GenerateNoise();
    }

    void GenerateNoise() {
        float[,] heightmap = new float[width + 1, height + 1];

        for (int y = 0; y <= height; y++) {
            for (int x = 0; x <= width; x++) {
                float xCoord = xOrg + x / (float)width * scale;
                float yCoord = yOrg + y / (float)height * scale;

                float sampleBase = Mathf.PerlinNoise(xCoord, yCoord) * strength;
                
                float detailXCoord = xOrg + x / (float)width * detailScale;
                float detailYCoord = yOrg + y / (float)height * detailScale;

                float detailedSample = Mathf.PerlinNoise(detailXCoord, detailYCoord) * detailStrength;
                
                heightmap[x, y] = sampleBase + detailedSample;
            }
        }
        
        _terrainData.SetHeights(0, 0, heightmap);
    }
}
