using ProceduralNoiseProject;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MarchingCubesProject;
using System.Collections;
public class WorldGenerator : MonoBehaviour
{
    public Material material;
    public MARCHING_MODE mode = MARCHING_MODE.CUBES;

    public int chunkSize = 16; // Size of each chunk
    public int worldWidth = 4; // Number of chunks in x-direction
    public int worldHeight = 4; // Number of chunks in z-direction
    public int height = 32; // Height of the world (in voxels)
    public Transform player; 
    private Vector2Int lastPlayerChunk;
    private Queue<Vector2Int> chunkQueue = new Queue<Vector2Int>();
    private bool isGeneratingChunks = false;
    public int viewDistance = 2;
    public int seed = 0;
    public bool smoothNormals = false;

    public int lod0Distance = 2; // High resolution
    public int lod1Distance = 4; // Medium resolution
    public int lod2Distance = 6; // Low resolution

    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();


    private void Update()
    {
        // Check if the player has moved to a new chunk
        Vector2Int currentChunk = GetChunkCoord(player.position);
        if (currentChunk != lastPlayerChunk)
        {
            lastPlayerChunk = currentChunk;
            UpdateChunks();
        }
        //UpdateLOD();
    }

    /// <summary>
    /// Coroutine to handle chunk updates asynchronously.
    /// </summary>
    private IEnumerator ChunkUpdater()
    {
        while (true)
        {
            if (chunkQueue.Count > 0 && !isGeneratingChunks)
            {
                Vector2Int chunkCoord = chunkQueue.Dequeue();
                yield return StartCoroutine(CreateChunkAsync(chunkCoord));
            }
            yield return null;
        }
    }

    /// <summary>
    /// Update the list of active chunks based on the player's position.
    /// </summary>
    private void UpdateChunks()
    {
        Vector2Int playerChunk = GetChunkCoord(player.position);

        // Load chunks within view distance
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2Int chunkCoord = playerChunk + new Vector2Int(x, z);

                if (!chunks.ContainsKey(chunkCoord) && !chunkQueue.Contains(chunkCoord))
                {
                    chunkQueue.Enqueue(chunkCoord);
                }
            }
        }   

        // Unload chunks outside the view distance
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunkCoord in chunks.Keys)
        {
            if (Vector2Int.Distance(chunkCoord, playerChunk) > viewDistance)
            {
                chunksToRemove.Add(chunkCoord);
            }
        }

        foreach (var chunkCoord in chunksToRemove)
        {
            Destroy(chunks[chunkCoord].gameObject);
            chunks.Remove(chunkCoord);
        }
    }



    /// <summary>
    /// Asynchronously create a chunk at the given coordinate.
    /// </summary>
    private IEnumerator CreateChunkAsync(Vector2Int chunkCoord)
    {
        isGeneratingChunks = true;

        GameObject chunkObject = new GameObject($"Chunk {chunkCoord}");
        chunkObject.transform.parent = transform;
        chunkObject.transform.position = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);

        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.Initialize(chunkSize, height, seed, mode, material, smoothNormals, chunkCoord);

        chunks[chunkCoord] = chunk;

        yield return null; // Yield to spread out chunk creation over frames
        isGeneratingChunks = false;
    }

    /// <summary>
    /// Get the chunk coordinate from a world position.
    /// </summary>
    private Vector2Int GetChunkCoord(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int z = Mathf.FloorToInt(position.z / chunkSize);
        return new Vector2Int(x, z);
    }
    private void Start()
    {
        GenerateWorld();
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned.");
            return;
        }

        lastPlayerChunk = GetChunkCoord(player.position);
        StartCoroutine(ChunkUpdater());
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldHeight; z++)
            {
                CreateChunk(new Vector2Int(x, z));
            }
        }
    }

    private void CreateChunk(Vector2Int chunkCoord)
    {
        GameObject chunkObject = new GameObject($"Chunk {chunkCoord}");
        chunkObject.transform.parent = transform;
        chunkObject.transform.position = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);

        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.Initialize(chunkSize, height, seed, mode, material, smoothNormals, chunkCoord);
        chunks[chunkCoord] = chunk;
    }

    public void ModifyTerrain(Vector3 worldPosition, float radius, float value)
    {
        Vector2Int chunkCoord = new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / chunkSize),
            Mathf.FloorToInt(worldPosition.z / chunkSize)
        );

        if (chunks.TryGetValue(chunkCoord, out Chunk chunk))
        {
            Vector3 localPosition = new Vector3(
                worldPosition.x % chunkSize,
                worldPosition.y,
                worldPosition.z % chunkSize
            );

            chunk.ModifyVoxel(localPosition, radius, value);
        }
    }
}

public class Chunk : MonoBehaviour
{
    private int chunkSize;
    private int height;
    private MARCHING_MODE mode;
    private Material material;
    private bool smoothNormals;

    private float[,,] voxels;
    private List<GameObject> meshes = new List<GameObject>();

    private Marching marching;

    public void Initialize(int chunkSize, int height, int seed, MARCHING_MODE mode, Material material, bool smoothNormals, Vector2Int chunkCoord)
    {
        this.chunkSize = chunkSize;
        this.height = height;
        this.mode = mode;
        this.material = material;
        this.smoothNormals = smoothNormals;

        INoise perlin = new PerlinNoise(seed, 1.0f);
        FractalNoise fractal = new FractalNoise(perlin, 3, 1.0f);

        marching = mode == MARCHING_MODE.TETRAHEDRON ? (Marching)new MarchingTertrahedron() : new MarchingCubes();
        marching.Surface = 0.0f;

        // Include extra boundary voxels for seamless chunk connection
        voxels = new float[chunkSize + 1, height, chunkSize + 1];

        for (int x = 0; x <= chunkSize; x++) // Note: `<=` for boundary
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z <= chunkSize; z++) // Note: `<=` for boundary
                {
                    // Calculate global voxel coordinates
                    float globalX = (chunkCoord.x * chunkSize + x) / (float)(chunkSize * 4);
                    float globalY = y / (float)height;
                    float globalZ = (chunkCoord.y * chunkSize + z) / (float)(chunkSize * 4);

                    // Sample noise for voxel value
                    voxels[x, y, z] = fractal.Sample3D(globalX, globalY, globalZ);
                }
            }
        }

        RebuildMesh();
    }

    public void ModifyVoxel(Vector3 localPosition, float radius, float value)
    {
        int startX = Mathf.Max(0, Mathf.FloorToInt(localPosition.x - radius));
        int endX = Mathf.Min(chunkSize - 1, Mathf.CeilToInt(localPosition.x + radius));

        int startY = Mathf.Max(0, Mathf.FloorToInt(localPosition.y - radius));
        int endY = Mathf.Min(height - 1, Mathf.CeilToInt(localPosition.y + radius));

        int startZ = Mathf.Max(0, Mathf.FloorToInt(localPosition.z - radius));
        int endZ = Mathf.Min(chunkSize - 1, Mathf.CeilToInt(localPosition.z + radius));

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                for (int z = startZ; z <= endZ; z++)
                {
                    Vector3 voxelPos = new Vector3(x, y, z);
                    if (Vector3.Distance(voxelPos, localPosition) <= radius)
                    {
                        voxels[x, y, z] += value;
                    }
                }
            }
        }

        RebuildMesh();
    }

    private void RebuildMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        // Generate the mesh using the marching cubes algorithm
        marching.Generate(voxels, verts, indices);

        // Create the Unity mesh
        Mesh mesh = new Mesh
        {
            indexFormat = IndexFormat.UInt32
        };
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        if (meshes.Count == 0)
        {
            GameObject meshObject = new GameObject("Mesh");
            meshObject.transform.parent = transform;
            meshObject.transform.position = transform.position;
            meshObject.AddComponent<MeshFilter>();
            meshObject.AddComponent<MeshRenderer>();
            meshObject.AddComponent<MeshCollider>();
            meshObject.GetComponent<Renderer>().material = material;

            meshes.Add(meshObject);
        }

        // Assign the new mesh to the GameObject
        meshes[0].GetComponent<MeshFilter>().mesh = mesh;
        meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;
    }

}