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
    /// Use this for a <see cref="DynamicParticle"/> effect.
    /// </summary>
    public DynamicParticle destroyParticleDynamic;
    /// <summary>
    /// Use this for a <see cref="ParticleEffect"/> effect.
    /// </summary>
    public ParticleEffect destroyParticle;

    /// <summary>
    /// Play the destroy effect(s). Can play both if they are each not null.
    /// </summary>
    protected void PlayDestroyEffect()
    {
        if (destroyParticle != null)
        {
            ParticleManager.PlayParticle(destroyParticle, transform.position);
        }

        if (destroyParticleDynamic != null)
        {
            var effect = Instantiate(destroyParticleDynamic, transform.position, destroyParticleDynamic.transform.rotation);
            effect.Play();
        }
    }

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
                    if (pooledItem == null)
                        return;
                    pooledItem.Initialize(transform.position, other:loot.item);
                    int angle = Random.Range(0, 360);
                    float randomSpeed = Random.Range(0.05f, 0.25f);
                    Vector2 movementVector = ((Vector2)transform.up).Rotate(angle) * randomSpeed;
                    pooledItem.Activate(movementVector);
                }
            }
        }
    }
}
