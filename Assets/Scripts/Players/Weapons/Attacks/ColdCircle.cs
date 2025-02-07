using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ColdCircle : MonoBehaviour
{
    public float maxRadius = 5f;
    public float expansionSpeed = 3f;
    public float damage = 30f;
    public float duration = 1.5f;
    public float height = 0.2f;
    public LayerMask enemyLayer = 8;
    public int resolution = 64;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Start()
    {
        // Create Mesh Components
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = CreateColdMaterial(); // Assign procedural shader
        
        // Generate the Circle Mesh
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        gameObject.transform.localScale += new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        //coldCircle.transform.localScale += new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        GenerateCircleMesh(0.1f);

        StartCoroutine(ExpandAndDestroy());
    }

    private IEnumerator ExpandAndDestroy()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newRadius = Mathf.Lerp(0, maxRadius, elapsedTime / duration);
            transform.localScale = new Vector3(newRadius * 2f, height, newRadius * 2f); // Adjust height
            GenerateCircleMesh(newRadius);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);

            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage((int)damage, Vector3.zero, 0f);
            }
        }
    }

    private void GenerateCircleMesh(float radius)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float noiseStrength = 0.1f;

        // Top and bottom circles
        for (int j = 0; j <= 1; j++) // 0 = bottom, 1 = top
        {
            float yOffset = j * height; // Raise the top vertices slightly
            vertices.Add(new Vector3(0, yOffset, 0)); // Center point
            uvs.Add(new Vector2(0.5f, 0.5f));

            for (int i = 0; i <= resolution; i++)
            {
                float angle = (i / (float)resolution) * Mathf.PI * 2;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                float noise = Mathf.PerlinNoise(x * 2f, y * 2f) * noiseStrength;
                Vector3 point = new Vector3(x, yOffset, y) * (radius + noise);

                vertices.Add(point);
                uvs.Add(new Vector2(x * 0.5f + 0.5f, y * 0.5f + 0.5f));

                if (i > 0)
                {
                    int baseIndex = j * (resolution + 2);
                    triangles.Add(baseIndex);
                    triangles.Add(baseIndex + i);
                    triangles.Add(baseIndex + i + 1);
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    private Material CreateColdMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.SetColor("_Color", new Color(0.5f, 0.8f, 1f, 0.5f));
        mat.SetFloat("_Glossiness", 0.1f);
        mat.SetFloat("_Metallic", 0.2f);

        Texture2D tex = new Texture2D(256, 256);
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                float brightness = Mathf.Lerp(0.2f, 1f, noise);
                tex.SetPixel(x, y, new Color(0.5f * brightness, 0.8f * brightness, 1f * brightness, 1f));
            }
        }
        tex.Apply();
        mat.mainTexture = tex;

        return mat;
    }
}
