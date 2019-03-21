using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{

    public static ParticlePool current;

    // The object pooled.
    public ParticleEffect defaultParticle;

    // How many of the object to pool.
    private int amountPooled;

    // The internal list of objects.
    private List<ParticleEffect> objectPool;

    private static int oldestIndex = 0;


    private void Awake()
    {
        current = this;
    }

	/// <summary>
	/// Set the size of the pool.
	/// </summary>
	public void SetPoolSize(int size)
	{
		if (objectPool == null)
			objectPool = new List<ParticleEffect>();

		if (size == amountPooled)
			return;

		int difference = amountPooled - size;

		if (size > amountPooled)
		{
			// Add on to the list.
			for (int i = amountPooled; i < size; i++)
			{
				var item = Instantiate(defaultParticle);
				item.gameObject.SetActive(false);
				objectPool.Add(item);
			}
		}
		else
		{
			// Subtract from the list.
			for (int i = amountPooled - 1; i >= amountPooled - difference; i--)
			{
				var effect = objectPool[objectPool.Count - 1];
				objectPool.RemoveAt(objectPool.Count-1);
				Destroy(effect);
			}
		}

		amountPooled = objectPool.Count;
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
