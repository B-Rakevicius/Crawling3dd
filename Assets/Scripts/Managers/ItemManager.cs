using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("Loot Drop Chances")]
    public float dropChance = 0.5f; // 50% chance to drop loot
    public float rareChance = 0.1f; // 10% for rare
    public float uncommonChance = 0.3f; // 30% for uncommon, rest is common

    [Header("Item Database")]
    public List<ItemData> allItems = new List<ItemData>();

    private Dictionary<string, int> itemStackCounts = new Dictionary<string, int>(); // Tracks item stacking

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Determines if an item should drop and spawns it at a given location.
    /// </summary>
    public void TryDropLoot(Vector3 position)
    {
        if (Random.value > dropChance) return; // No drop

        // Determine rarity
        float roll = Random.value;
        ItemRarity rarity = ItemRarity.Common;

        if (roll < rareChance) rarity = ItemRarity.Rare;
        else if (roll < rareChance + uncommonChance) rarity = ItemRarity.Uncommon;

        // Get possible drops of that rarity
        List<ItemData> possibleDrops = allItems.FindAll(item => item.rarity == rarity);

        if (possibleDrops.Count > 0)
        {
            ItemData chosenItem = possibleDrops[Random.Range(0, possibleDrops.Count)];
            SpawnItem(chosenItem, position);
        }
    }

    /// <summary>
    /// Spawns an item in the world.
    /// </summary>
    private void SpawnItem(ItemData item, Vector3 position)
    {
        if (item.prefab != null)
        {
            Instantiate(item.prefab, position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Adds an item to the inventory and handles stacking.
    /// </summary>
    public void AddItem(string itemName)
    {
        if (itemStackCounts.ContainsKey(itemName))
        {
            itemStackCounts[itemName]++;
        }
        else
        {
            itemStackCounts[itemName] = 1;
        }
    }

    /// <summary>
    /// Returns the current stack count of an item.
    /// </summary>
    public int GetItemStackCount(string itemName)
    {
        return itemStackCounts.ContainsKey(itemName) ? itemStackCounts[itemName] : 0;
    }
}
public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemRarity rarity;
    public GameObject prefab;
}
