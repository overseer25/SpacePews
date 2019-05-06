using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a pool that will manage the maximum number of items that can exist in the world.
/// If the maximum number of items is reached, and more items are being spawned, the pool will need
/// to clear out the oldest items in the world, and replace them with the newly spawned ones.
/// </summary>
public class ItemPool : BasePool
{
    public static ItemPool current;

    [SerializeField]
    private Item defaultItem;

    private static List<Item> itemPool;

    private void Awake()
    {
        itemPool = new List<Item>();
		if(current == null)
			current = this;    
    }

	/// <summary>
	/// Set the size of the pool.
	/// </summary>
	/// <param name="size">The new size.</param>
	public override void SetPoolSize(int size)
	{
		if (itemPool == null)
			itemPool = new List<Item>();

		if (size == poolSize)
			return;

		int difference = poolSize - size;

		if(size > poolSize)
		{
			// Add on to the list.
			for (int i = poolSize; i < size; i++)
			{
				var item = Instantiate(defaultItem, gameObject.transform);
				item.gameObject.SetActive(false);
				itemPool.Add(item);
			}
		}
		else
		{
			// Subtract from the list.
			for (int i = poolSize - 1; i >= poolSize - difference; i--)
			{
				Destroy(itemPool[itemPool.Count - 1].gameObject);
				itemPool.RemoveAt(itemPool.Count - 1);
			}
		}

		poolSize = itemPool.Count;
		if (oldest > poolSize)
			oldest = 0;
	}

    /// <summary>
    /// Gets an unused object in the object pool, if it exists.
    /// </summary>
    /// <returns> Unused GameObject in object pool, or the oldest active one (LRU). </returns>
    public override object GetPooledObject()
    {
        if (itemPool == null)
            return null;

        // Attempt to find an unused object in the pool.
        foreach (var item in itemPool)
        {
            if (item != null && !item.gameObject.activeInHierarchy)
            {
                return item;
            }
        }

        // Otherwise, get the least recently used object in the pool.
        var result = itemPool[oldest++];
		oldest %= poolSize;
        return result;
    }
}
