using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{

    // The object pooled.
    public Projectile defaultProjectile;
    // How many of the object to pool.
    public int amountPooled = 100;

    // The internal list of objects.
    private List<Projectile> objectPool;
    private static int oldestIndex = 0;

    public static ProjectilePool current;

    private void Awake()
    {
        if (current == null)
            current = this;
    }

    private void Start()
    {
        // If the object pool is already created, don't make it again.
        if (objectPool != null)
            return;
        objectPool = new List<Projectile>();
        for (int i = 0; i < amountPooled; i++)
        {
            var obj = Instantiate(defaultProjectile);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
    }

    /// <summary>
    /// Gets an unused object in the object pool, if it exists.
    /// </summary>
    /// <returns> Unused GameObject in object pool, or null if none exists. </returns>
    public Projectile GetPooledObject()
    {
        foreach (var obj in objectPool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                return obj;
            }
        }

        // Otherwise, get the least recently used object in the pool.
        var result = objectPool[oldestIndex++];
        oldestIndex %= amountPooled - 1;
        return result;
    }
}
