using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Abstract class that all scripts involving loot tables should inherit from. Provides functionality for spawning
/// loot from a loot table.
/// </summary>
public abstract class Lootable : MonoBehaviour
{
    public LootTable lootTable;
    public int maxResourceCount;
    public int minResourceCount;

    /// <summary>
    /// Spawns loot at the lootable object's location.
    /// </summary>
    public void SpawnLoot()
    {
        int amount = Random.Range(minResourceCount, maxResourceCount);
        float percentage;

        for (int i = 0; i < amount; i++)
        {
            percentage = Random.Range(0f, 1f);
            foreach (var loot in lootTable.lootList.OrderBy(e => e.chance))
            {
                if (loot.chance >= percentage)
                {
                    Item pooledItem = ItemPool.current.GetPooledObject() as Item;
                    pooledItem.Initialize(transform.position, other:loot.item);
                    int angle = Random.Range(0, 360);
                    Vector2 movementVector = ((Vector2)transform.up).Rotate(angle) * 0.25f;
                    pooledItem.Activate(movementVector);
                }
            }
        }
    }
}
