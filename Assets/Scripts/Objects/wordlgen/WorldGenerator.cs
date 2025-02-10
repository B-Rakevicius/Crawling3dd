using ProceduralNoiseProject;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MarchingCubesProject;
using System.Collections;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;
using Unity.Burst.CompilerServices;
using static UnityEditor.PlayerSettings;
public class WorldGenerator : MonoBehaviour
{
    public Material material;
    public MARCHING_MODE mode = MARCHING_MODE.CUBES;
    public int chunkSize = 16; // size of chunk
    public int startWidth = 4; // number of chunks in x-direction at start
    public int startLength = 4; // number of chunks in z-direction at start
    public int height = 128; // height multiplier
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
        /*
        if (Input.GetMouseButton(0)) // Left-click to mine
        {
            Debug.Log("mining or something");
            Vector3 worldPosition = GetMouseWorldPosition();
            Vector2Int chunkCoord = GetChunkCoord(worldPosition);
            Chunk chunk = chunks[chunkCoord];

            // Convert world position to local position in the chunk
            Vector3 localPosition = chunk.transform.InverseTransformPoint(worldPosition);

            // Start mining (remove voxels in a small radius, e.g., 1.0f)
            //ModifyTerrain(localPosition, 3f, -1f);
            chunk.ModifyVoxel(localPosition, 3.0f, -2.0f); // Negative value for digging
        }
        */
    }
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point; // Return the world position of the clicked block
        }
        return Vector3.zero;
    }
    /// <summary>
    /// Coroutine to handle chunk updates asynchronously.
    /// </summary>
    private IEnumerator ChunkUpdater()
    {
        while (true)
        {
            int chunksProcessed = 0; // Throttle

            while (chunkQueue.Count > 0 && chunksProcessed < 6) // Process 6 chunks per frame
            {
                Vector2Int chunkCoord = chunkQueue.Dequeue();
                yield return StartCoroutine(CreateChunkAsync(chunkCoord));
                chunksProcessed++;
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
        /*
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunkCoord in chunks.Keys)
        {
            if (Vector2Int.Distance(chunkCoord, playerChunk) > viewDistance)
            {
                //chunksToRemove.Add(chunkCoord);
            }
        }

        foreach (var chunkCoord in chunksToRemove)
        {
            Destroy(chunks[chunkCoord].gameObject);
            chunks.Remove(chunkCoord);
        }
        */
    }
    /// <summary>
    /// Dynamically update LOD levels based on player distance.
    /// </summary>
    private void UpdateLOD()
    {
        Vector2Int playerChunk = GetChunkCoord(player.position);

        foreach (var chunkCoord in chunks.Keys)
        {
            int distance = Mathf.RoundToInt(Vector2Int.Distance(playerChunk, chunkCoord));

            Chunk chunk = chunks[chunkCoord];
            if (distance <= lod0Distance)
            {
                chunk.SetLOD(0); // High resolution
            }
            else if (distance <= lod1Distance)
            {
                chunk.SetLOD(1); // Medium resolution
            }
            else if (distance <= lod2Distance)
            {
                chunk.SetLOD(2); // Low resolution
            }
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
        chunkObject.transform.localPosition = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);

        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.Initialize(chunkSize, height, seed, mode, material, smoothNormals, chunkCoord);

        chunks[chunkCoord] = chunk;

        yield return null; // Spread workload over frames
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
        CreateChunk(lastPlayerChunk);
        StartCoroutine(ChunkUpdater());
    }
    private void GenerateWorld()
    {
        for (int x = 0; x < startWidth; x++)
        {
            for (int z = 0; z < startLength; z++)
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
        //chunkObject.transform.parent.position = new Vector3(chunkCoord.x * chunkSize*(-1f), 0, chunkCoord.y * chunkSize*(-1f));
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
    private Dictionary<int, Mesh> lodMeshes = new Dictionary<int, Mesh>();
    private int currentLOD = -1;
    private GameObject meshObject;
    public void Initialize(int chunkSize, int height, int seed, MARCHING_MODE mode, Material material, bool smoothNormals, Vector2Int chunkCoord)
    {
        this.chunkSize = chunkSize;
        this.height = height;
        this.mode = mode;
        this.material = material;
        this.smoothNormals = smoothNormals;
        const int heightOffset = 0;
        int adjustedHeight = height + heightOffset + 200; // maxheight +100, +50 had unrendered voxels
        INoise basePerlin = new PerlinNoise(seed, 1.0f); // Base Perlin Noise
        FractalNoise fractal = new FractalNoise(basePerlin, 8, 1.5f); // Fractal Terrain
        FractalNoise riverNoise = new FractalNoise(new PerlinNoise(seed + 1, 1.0f), 2, 1.0f); // River Noise
        INoise mountainMask = new PerlinNoise(seed + 2, 0.2f); // Mountain noise
        marching = mode == MARCHING_MODE.TETRAHEDRON ? (Marching)new MarchingTertrahedron() : new MarchingCubes();
        marching.Surface = 0.0f;
        voxels = new float[chunkSize + 1, adjustedHeight, chunkSize + 1];

        for (int x = 0; x <= chunkSize; x++)
        {
            for (int z = 0; z <= chunkSize; z++)
            {
                float globalX = (chunkCoord.x * chunkSize + x) / (float)(chunkSize * 4);
                float globalZ = (chunkCoord.y * chunkSize + z) / (float)(chunkSize * 4);

                float baseNoise = fractal.Sample2D(globalX, globalZ);
                float amplifiedNoise = Mathf.Pow(baseNoise, 2.0f) * Mathf.Sign(baseNoise);
                float mountainFactor = Mathf.Clamp01(mountainMask.Sample2D(globalX, globalZ));
                float mountainHeightMultiplier = Mathf.Lerp(1.0f, 3.5f, Mathf.Pow(mountainFactor, 1.0f));
                int surfaceHeight = Mathf.FloorToInt(((amplifiedNoise + 1.0f) / 2.0f * height) * mountainHeightMultiplier) + heightOffset-35;
                float riverValue = riverNoise.Sample2D(globalX, globalZ);
                float riverDepth = Mathf.Lerp(0, 5, Mathf.Clamp01(1 - Mathf.Abs(riverValue)));

                for (int y = 0; y < adjustedHeight; y++)
                {
                    float globalY = y / (float)adjustedHeight;

                    if (y < surfaceHeight - riverDepth)
                    {
                        voxels[x, y, z] = 1.0f;
                    }
                    else if (y >= surfaceHeight - riverDepth && y <= surfaceHeight)
                    {
                        voxels[x, y, z] = 1.0f;
                    }
                    else
                    {
                        voxels[x, y, z] = -1.0f;
                    }
                }
                int minHeight = Mathf.Max(0, surfaceHeight - Mathf.CeilToInt(riverDepth));
                for (int y = 0; y < Mathf.Min(minHeight, height); y++)
                {
                    if (x >= 0 && x < chunkSize && y >= 0 && y < height && z >= 0 && z < chunkSize)
                    {
                        voxels[x, y, z] = 1.0f;
                    }
                }
            }
        }
        RebuildMesh();
    }
    public void Initialize2(int chunkSize, int height, int seed, MARCHING_MODE mode, Material material, bool smoothNormals, Vector2Int chunkCoord)
    {
        this.chunkSize = chunkSize;
        this.height = height;
        this.mode = mode;
        this.material = material;
        this.smoothNormals = smoothNormals;

        const int heightOffset = 20;
        int adjustedHeight = height + heightOffset;

        INoise perlin = new PerlinNoise(seed, 1.0f); // perlin
        FractalNoise fractal = new FractalNoise(perlin, 8, 1.5f); // fractal
        FractalNoise riverNoise = new FractalNoise(new PerlinNoise(seed + 1, 1.0f), 2, 1.0f); // river

        marching = mode == MARCHING_MODE.TETRAHEDRON ? (Marching)new MarchingTertrahedron() : new MarchingCubes();
        marching.Surface = 0.0f;

        voxels = new float[chunkSize + 1, adjustedHeight, chunkSize + 1];

        for (int x = 0; x <= chunkSize; x++)
        {
            for (int z = 0; z <= chunkSize; z++)
            {
                float globalX = (chunkCoord.x * chunkSize + x) / (float)(chunkSize * 4);
                float globalZ = (chunkCoord.y * chunkSize + z) / (float)(chunkSize * 4);

                float rawNoise = fractal.Sample2D(globalX, globalZ);
                float amplifiedNoise = Mathf.Pow(rawNoise, 2.0f) * Mathf.Sign(rawNoise);
                int surfaceHeight = Mathf.FloorToInt((amplifiedNoise + 1.0f) / 2.0f * height) + heightOffset; 

                float riverValue = riverNoise.Sample2D(globalX, globalZ);
                float riverDepth = Mathf.Lerp(0, 5, Mathf.Clamp01(1 - Mathf.Abs(riverValue)));

                for (int y = 0; y < adjustedHeight; y++)
                {
                    float globalY = y / (float)adjustedHeight;

                    if (y < surfaceHeight - riverDepth)
                    {
                        voxels[x, y, z] = 1.0f;
                    }
                    else if (y >= surfaceHeight - riverDepth && y <= surfaceHeight)
                    {
                        voxels[x, y, z] = 1.0f;
                    }
                    else
                    {
                        voxels[x, y, z] = -1.0f;
                    }
                }
                int minHeight = Mathf.Max(0, surfaceHeight - Mathf.CeilToInt(riverDepth));
                for (int y = 0; y <= minHeight; y++)
                {
                    voxels[x, y, z] = 1.0f;
                }
            }
        }

        RebuildMesh();
    }
    public void SetLOD(int lod)
    {
        if (lod == currentLOD) return;

        currentLOD = lod;

        if (!lodMeshes.ContainsKey(lod))
        {
            lodMeshes[lod] = GenerateMeshForLOD(lod);
        }

        if (meshObject == null)
        {
            meshObject = new GameObject("Mesh");
            meshObject.tag = "Ground";
            meshObject.layer = 7;
            meshObject.transform.parent = transform;
            meshObject.AddComponent<MeshFilter>();
            meshObject.AddComponent<MeshRenderer>();
            meshObject.AddComponent<MeshCollider>();
            meshObject.GetComponent<Renderer>().material = material;
        }

        Mesh mesh = lodMeshes[lod];
        meshObject.GetComponent<MeshFilter>().mesh = mesh;
        meshObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        meshObject.transform.localPosition = Vector3.zero;
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
                        if (value < 0)
                        {
                            voxels[x, y, z] = Mathf.Max(voxels[x, y, z] + value, 100);

                        }
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
        marching.Generate(voxels, verts, indices);
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
            meshObject.transform.localPosition = new Vector3(0, 0, 0);
            meshObject.tag = "Ground";
            meshObject.layer = 7;
            meshObject.AddComponent<MeshFilter>();
            meshObject.AddComponent<MeshRenderer>();
            meshObject.AddComponent<MeshCollider>();
            meshObject.GetComponent<Renderer>().material = material;
            meshes.Add(meshObject);
            //Debug.Log(this + " Highest terrain point " + FindHighestPoint());
        }
        meshes[0].GetComponent<MeshFilter>().mesh = mesh;
        meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;
        ApplyMaterialBasedOnHeight();
        // if high bounds, debug logs the height and the chunk #number
        if (meshes[0].GetComponent<MeshFilter>().mesh.bounds.size.y > 48)
        {
            // setup splitting mesh into four smaller meshes, to correctly apply materials and such
            Debug.Log(this + " this is above 48 " + meshes[0].GetComponent<MeshFilter>().mesh.bounds.size.y);
        }
    }
    private void ApplyMaterialBasedOnHeight()
    {
        float highestPoint = FindHighestPoint();

        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer == null) return;

        if (highestPoint > 96)
            renderer.material = ShaderManager.instance.whiteMountain;
        else if (highestPoint > 48)
            renderer.material = ShaderManager.instance.grayMountain;
        else if (highestPoint > 32)
            renderer.material = ShaderManager.instance.rockMountain;
        else if (highestPoint < 16)
            renderer.material = ShaderManager.instance.grayMountain;
    }
    private float FindHighestPoint()
    {
        float highestY = float.MinValue;

        MeshCollider meshCollider = meshes[0].GetComponent<MeshCollider>();

        if (meshCollider != null)
        {
            Vector3 rayOrigin = new Vector3(transform.position.x + chunkSize / 2, height * 2, transform.position.z + chunkSize / 2);
            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;

            if (meshCollider.Raycast(ray, out hit, height * 3))
            {
                highestY = hit.point.y;
                //Debug.Log($"Highest terrain point: {highestY} " + this);
            }
            else
            {
                //Debug.LogWarning("Raycast did not hit the terrain." + this);
            }
        }
        else
        {
            //Debug.LogError("MeshCollider is missing." + this);
        }
        return highestY;
    }
    private Mesh GenerateMeshForLOD(int lod)
    {
        int resolution = chunkSize / (1 << lod);
        float[,,] simplifiedVoxels = DownsampleVoxels(voxels, resolution);

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        marching.Generate(simplifiedVoxels, verts, indices);

        Mesh mesh = new Mesh
        {
            indexFormat = IndexFormat.UInt32
        };
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
    private float[,,] DownsampleVoxels(float[,,] originalVoxels, int resolution)
    {
        float[,,] result = new float[resolution + 1, height, resolution + 1];
        float step = chunkSize / (float)resolution;

        for (int x = 0; x <= resolution; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z <= resolution; z++)
                {
                    int origX = Mathf.FloorToInt(x * step);
                    int origZ = Mathf.FloorToInt(z * step);
                    result[x, y, z] = originalVoxels[origX, y, origZ];
                }
            }
        }

        return result;
    }
}