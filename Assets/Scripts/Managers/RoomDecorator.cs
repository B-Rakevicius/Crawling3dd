using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomDecorator : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab; // The object to spawn
        public int weight; // Weight of the object (e.g., 1 for small, 2-3 for large)
        public float spawnChance; // Chance to spawn this object (0 to 1)
        public string requiredTag; // Tag of the tile where this object can spawn (e.g., "Floor", "Wall", "Ceiling")
    }

    [Header("Spawnable Objects")]
    public SpawnableObject[] spawnableObjects; // List of objects that can be spawned

    [Header("Room Settings")]
    public int maxWeightPerRoom = 10; // Maximum weight allowed per room

    private int seed; // Seed for random number generation

    public void Initialize(int seed)
    {
        this.seed = seed;
        Random.InitState(seed); // Initialize the random number generator with the seed
        DecorateRoom();
    }

    void DecorateRoom()
    {
        int currentWeight = 0;

        // Shuffle the list of spawnable objects to randomize spawning order
        Shuffle(spawnableObjects);

        foreach (var spawnable in spawnableObjects)
        {
            if (currentWeight >= maxWeightPerRoom)
                break; // Stop if the room has reached its weight limit

            // Find all tiles with the required tag
            GameObject[] tiles = GameObject.FindGameObjectsWithTag(spawnable.requiredTag);
            // Shuffle the tiles to randomize spawning locations
            Shuffle(tiles);

            foreach (var tile in tiles)
            {
                if (currentWeight + spawnable.weight > maxWeightPerRoom)
                    break; // Stop if adding this object would exceed the weight limit
                // if the tile doesnt match the room, dont spawn anynthing
                if (!tile.transform.IsChildOf(this.gameObject.transform))
                { 

                }
                else if (Random.value < spawnable.spawnChance)
                {
                    Debug.Log("Spawning object " + spawnable.prefab.ToString() + " at room " + this.gameObject.name);
                    // Spawn the object at the tile's position
                    float offset = Random.value;
                    //wall objects should spawn somewhere on the wall, while floor and ceiling ones can spawn in the middle of tile for now
                    if(spawnable.requiredTag == "Wall")
                        Instantiate(spawnable.prefab, tile.transform.position + new Vector3(0,1f*3+offset*10,0), Quaternion.identity, transform);
                    else if(spawnable.requiredTag == "Ceiling")
                        Instantiate(spawnable.prefab, tile.transform.position, Quaternion.identity, transform);
                    else
                        Instantiate(spawnable.prefab, tile.transform.position, Quaternion.identity, transform);
                    currentWeight += spawnable.weight; // Add to the room's total weight
                }   
            }
        }
    }

    // Helper method to shuffle an array
    void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            T temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
        }
    }
}