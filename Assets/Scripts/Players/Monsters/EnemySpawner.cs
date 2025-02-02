using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject strongEnemyPrefab;
    public Transform player;
    public float spawnRadius = 50f;
    public float spawnInterval = 2f;
    public float strongEnemyInterval = 60f;
    public LayerMask groundLayer; // Assign ground layers in Inspector

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnStrongEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy(enemyPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator SpawnStrongEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(strongEnemyInterval);
            SpawnEnemy(strongEnemyPrefab);
        }
    }

    private void SpawnEnemy(GameObject enemyType)
    {
        if (player == null) return;

        Vector3 randomOffset = Random.onUnitSphere * spawnRadius;
        randomOffset.y = 10f; // Start above the terrain
        Vector3 spawnPosition = player.position + randomOffset;

        // Raycast downward to find ground
        if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, 20f, groundLayer))
        {
            spawnPosition = hit.point; // Place enemy on the actual surface
            Instantiate(enemyType, spawnPosition, Quaternion.identity);
        }
    }
}
