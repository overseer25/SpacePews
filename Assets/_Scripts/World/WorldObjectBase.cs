using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A World Object is an object whose state is maintained by a world chunk. A World Object can have an associated loot table.
/// </summary>
public abstract class WorldObjectBase : MonoBehaviour
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
    /// Enables the world object.
    /// </summary>
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Disables the world object.
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

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
    /// Spawns loot at the world object's location.
    /// </summary>
    public void SpawnLoot()
    {
        int amount = Random.Range(minResourceCount, maxResourceCount);
        float chance;
        for (int i = 0; i < amount; i++)
        {
            chance = Random.Range(0f, 1f);
            foreach (var loot in lootTable.lootList)
            {
                if (chance >= loot.rangeMin && chance <= loot.rangeMax)
                {
                    Item pooledItem = ItemPool.current.GetPooledObject() as Item;
                    if (pooledItem == null)
                        return;
                    pooledItem.Initialize(transform.position, other: loot.item);
                    int angle = Random.Range(0, 360);
                    float randomSpeed = Random.Range(0.05f, 0.25f);
                    Vector2 movementVector = ((Vector2)transform.up).Rotate(angle) * randomSpeed;
                    pooledItem.Activate(movementVector);
                    break;
                }
            }
        }
    }
}
