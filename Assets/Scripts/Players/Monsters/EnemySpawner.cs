using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject strongEnemyPrefab;
    public Transform player;

    public float spawnRadius = 50f;
    public float waveInterval = 5f; // Waves spawn every 5 seconds
    public int maxEnemiesPerWave = 10; // Maximum enemies per wave
    public LayerMask groundLayer;

    private float spawnBuffer = 0f; // Accumulates over time
    private bool playerInSafeZone = true; // If true, no spawns happen

    // Cost values for different enemy types
    private const int normalEnemyCost = 1;
    private const int strongEnemyCost = 3; // Strong enemies cost more buffer

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        StartCoroutine(WaveSpawner());
    }

    private void Update()
    {
        if (!playerInSafeZone)
        {
            spawnBuffer += Time.deltaTime; // Buffer increases when outside safe zone
        }
    }

    private IEnumerator WaveSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);

            if (!playerInSafeZone) // Only spawn when outside safe zone
            {
                SpawnWave();
            }
        }
    }

    private void SpawnWave()
    {
        float availableBuffer = spawnBuffer; // Copy buffer value for this wave
        int enemiesSpawned = 0;

        while (availableBuffer > 0 && enemiesSpawned < maxEnemiesPerWave)
        {
            GameObject enemyType;
            int enemyCost;

            if (availableBuffer >= strongEnemyCost && Random.value > 0.7f) // 30% chance for strong enemy if enough buffer
            {
                enemyType = strongEnemyPrefab;
                enemyCost = strongEnemyCost;
            }
            else
            {
                enemyType = enemyPrefab;
                enemyCost = normalEnemyCost;
            }

            // If we can afford this enemy, spawn it
            if (availableBuffer >= enemyCost)
            {
                SpawnEnemy(enemyType);
                availableBuffer -= enemyCost;
                enemiesSpawned++;
            }
            else
            {
                break; // Stop if not enough buffer
            }
        }

        spawnBuffer -= (spawnBuffer - availableBuffer); // Deduct spent buffer
    }

    private void SpawnEnemy(GameObject enemyType)
    {
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 10f; // Start above terrain
        Vector3 spawnPosition = player.position + randomOffset;

        // Raycast downward to find the ground
        if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, 20f, groundLayer))
        {
            spawnPosition = hit.point; // Adjust to terrain surface
            Instantiate(enemyType, spawnPosition, Quaternion.identity);
        }
    }

    public void SetSafeZoneStatus(bool isInside)
    {
        if (playerInSafeZone && !isInside) // Leaving safe zone
        {
            
            SpawnWave(); // Immediate enemy spawn upon exit
        }
        playerInSafeZone = isInside;
    }
}
