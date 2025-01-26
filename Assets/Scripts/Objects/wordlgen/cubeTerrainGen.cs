using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTerrain : MonoBehaviour
{
    
    [Header("Terrain Settings")]
    public int width = 16; // Width of the terrain in chunks
    public int depth = 16; // Depth of the terrain in chunks
    public int height = 32; // Maximum height of the terrain
    public float scale = 20f; // Scale for Perlin noise

    [Header("Voxel Settings")]
    public GameObject voxelPrefab; // Prefab for the voxel (e.g., cube)
    public int chunkSize = 16; // Number of voxels per chunk

    [Header("Performance Settings")]
    public bool useCulling = true; // Enable/disable culling for better performance

    private Dictionary<Vector3Int, GameObject> chunkObjects = new Dictionary<Vector3Int, GameObject>();

    void Start()
    {
        GenerateTerrain();
    }

    // Generate the entire terrain
    void GenerateTerrain()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                GenerateChunk(x, z);
            }
        }
    }

    // Generate a chunk of terrain
    void GenerateChunk(int chunkX, int chunkZ)
    {
        GameObject chunk = new GameObject($"Chunk_{chunkX}_{chunkZ}");
        chunk.transform.position = new Vector3(chunkX * chunkSize, 0, chunkZ * chunkSize);
        chunkObjects[new Vector3Int(chunkX, 0, chunkZ)] = chunk;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                // World position of the voxel
                int worldX = chunkX * chunkSize + x;
                int worldZ = chunkZ * chunkSize + z;

                // Get the height at this point
                int heightValue = Mathf.FloorToInt(HeightAt(worldX, worldZ));

                for (int y = 0; y <= heightValue; y++)
                {
                    Vector3 voxelPosition = new Vector3(worldX, y, worldZ);
                    InstantiateVoxel(voxelPosition, chunk.transform);
                }
            }
        }

        if (useCulling)
        {
            // Add a MeshRenderer and MeshCollider for optimized rendering
            CombineMeshes(chunk);
        }
    }

    // Instantiate a voxel at a given position
    void InstantiateVoxel(Vector3 position, Transform parent)
    {
        GameObject voxel = Instantiate(voxelPrefab, position, Quaternion.identity, parent);
        voxel.name = $"Voxel_{position.x}_{position.y}_{position.z}";
    }

    // Combine meshes for performance optimization
    void CombineMeshes(GameObject chunk)
    {
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        MeshFilter chunkMeshFilter = chunk.AddComponent<MeshFilter>();
        chunkMeshFilter.mesh = combinedMesh;

        MeshRenderer chunkRenderer = chunk.AddComponent<MeshRenderer>();
        chunkRenderer.sharedMaterial = voxelPrefab.GetComponent<MeshRenderer>().sharedMaterial;

        // Optional: Add collider for interaction
        chunk.AddComponent<MeshCollider>().sharedMesh = combinedMesh;

        // Destroy individual voxel GameObjects to save performance
        foreach (Transform voxel in chunk.transform)
        {
            Destroy(voxel.gameObject);
        }
    }

    // Calculate the height using Perlin noise
    float HeightAt(float x, float z)
    {
        float xCoord = x / scale;
        float zCoord = z / scale;
        return Mathf.PerlinNoise(xCoord, zCoord) * height;
    }
}
