using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : BasePool
{
	[SerializeField]
    private Projectile defaultProjectile;

    // The internal list of objects.
    private List<Projectile> projectilePool;

    public static ProjectilePool current;

    private void Awake()
    {
        projectilePool = new List<Projectile>();
        if (current == null)
            current = this;
    }

	public override void SetPoolSize(int size)
	{
		if (projectilePool == null)
			projectilePool = new List<Projectile>();

		if (size == poolSize)
			return;

		int difference = poolSize - size;

		if (size > poolSize)
		{
			// Add on to the list.
			for (int i = poolSize; i < size; i++)
			{
				var proj = Instantiate(defaultProjectile, gameObject.transform);
				proj.gameObject.SetActive(false);
				projectilePool.Add(proj);
			}
		}
		else
		{
			// Subtract from the list.
			for (int i = poolSize - 1; i >= poolSize - difference; i--)
			{
				Destroy(projectilePool[projectilePool.Count - 1].gameObject);
				projectilePool.RemoveAt(projectilePool.Count - 1);
			}
		}

		poolSize = projectilePool.Count;
		if (oldest > poolSize)
			oldest = 0;
	}

	/// <summary>
	/// Gets an unused object in the object pool, if it exists.
	/// </summary>
	/// <returns> Unused GameObject in object pool, or null if none exists. </returns>
	public override object GetPooledObject()
    {
        foreach (var obj in projectilePool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                return obj;
            }
        }

        // Otherwise, get the least recently used object in the pool.
        var result = projectilePool[oldest++];
        oldest %= poolSize;
        return result;
    }
}
