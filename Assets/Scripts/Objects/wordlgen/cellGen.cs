using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellGen : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int terrainWidth = 256;
    public int terrainHeight = 256;
    public int terrainDepth = 50;
    public float scale = 20f;

    [Header("Resource Settings")]
    public GameObject resourcePrefab;
    public int resourceCount = 50;
    public float resourceSpawnHeight = 10f;

    [Header("Structure Settings")]
    public GameObject structurePrefab;
    public int structureCount = 10;
    public float structureSpawnHeight = 20f;

    [Header("Chunk Settings")]
    public int chunkSize = 32;

    private void Start()
    {
        GenerateTerrain();
        PlaceResources();
        PlaceStructures();
    }

    // Generate terrain using Perlin noise
    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);
    }

    TerrainData GenerateTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = terrainWidth + 1;
        terrainData.size = new Vector3(terrainWidth, terrainDepth, terrainHeight);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[terrainWidth, terrainHeight];
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                float xCoord = (float)x / terrainWidth * scale;
                float yCoord = (float)y / terrainHeight * scale;
                heights[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }
        return heights;
    }

    // Place resources dynamically on the terrain
    void PlaceResources()
    {
        for (int i = 0; i < resourceCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(0, terrainWidth),
                resourceSpawnHeight,
                Random.Range(0, terrainHeight)
            );

            position.y = TerrainHeightAt(position.x, position.z) + 1;
            Instantiate(resourcePrefab, position, Quaternion.identity);
        }
    }

    // Place structures dynamically on the terrain
    void PlaceStructures()
    {
        for (int i = 0; i < structureCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(0, terrainWidth),
                structureSpawnHeight,
                Random.Range(0, terrainHeight)
            );

            position.y = TerrainHeightAt(position.x, position.z) + 1;
            Instantiate(structurePrefab, position, Quaternion.identity);
        }
    }

    // Get terrain height at a specific position
    float TerrainHeightAt(float x, float z)
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;

        float normalizedX = x / terrainData.size.x;
        float normalizedZ = z / terrainData.size.z;

        return terrainData.GetHeight(
            Mathf.FloorToInt(normalizedX * terrainData.heightmapResolution),
            Mathf.FloorToInt(normalizedZ * terrainData.heightmapResolution)
        );
    }

}
