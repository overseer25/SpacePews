using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{

    public static ParticlePool current;

    // The object pooled.
    public ParticleEffect defaultParticle;

    // How many of the object to pool.
    public int amountPooled = 20;

    // The internal list of objects.
    private List<ParticleEffect> objectPool;

    private static int oldestIndex = 0;


    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        objectPool = new List<ParticleEffect>();

        for (int i = 0; i < amountPooled; i++)
        {
            var obj = Instantiate(defaultParticle);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
    }

    /// <summary>
    /// Gets an unused object in the object pool, if it exists.
    /// </summary>
    /// <returns> Unused GameObject in object pool, or the oldest active one (LRU). </returns>
    public ParticleEffect GetPooledObject()
    {
        foreach (var obj in objectPool)
        {
            if (obj != null && !obj.gameObject.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}
