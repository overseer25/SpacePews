using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a pool that will manage the maximum number of items that can exist in the world.
/// If the maximum number of items is reached, and more items are being spawned, the class will need
/// to clear out the oldest items in the world, and replace them with the newly spawned ones.
/// </summary>
public class ItemPool : MonoBehaviour
{
    public static ItemPool current;

    [SerializeField]
    private Item defaultItem;

	private int amountPooled;
    private static List<Item> itemPool;
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
		if (itemPool == null)
			itemPool = new List<Item>();

		if (size == amountPooled)
			return;

		int difference = amountPooled - size;

		if(size > amountPooled)
		{
			// Add on to the list.
			for (int i = amountPooled; i < size; i++)
			{
				var item = Instantiate(defaultItem);
				item.gameObject.SetActive(false);
				itemPool.Add(item);
			}
		}
		else
		{
			// Subtract from the list.
			for (int i = amountPooled-1; i >= amountPooled - difference; i--)
			{
				Destroy(itemPool[itemPool.Count - 1].gameObject);
				itemPool.RemoveAt(itemPool.Count - 1);
			}
		}

		amountPooled = itemPool.Count;
	}

    /// <summary>
    /// Gets an unused object in the object pool, if it exists.
    /// </summary>
    /// <returns> Unused GameObject in object pool, or the oldest active one (LRU). </returns>
    public Item GetPooledObject()
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
        Item result = itemPool[oldestIndex++];
        oldestIndex %= amountPooled - 1;
        return result;
    }
}
