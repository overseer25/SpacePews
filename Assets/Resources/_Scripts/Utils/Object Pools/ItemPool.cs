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
    [SerializeField]
    private int amountPooled = 64;
    private static List<Item> itemPool;
    private static int oldestIndex = 0;


    private void Awake()
    {
        current = this;    
    }

    // Start is called before the first frame update
    void Start()
    {
        itemPool = new List<Item>();

        // Populate the list.
        for (int i = 0; i < amountPooled; i++)
        {
            var item = Instantiate(defaultItem);
            item.gameObject.SetActive(false);
            itemPool.Add(item);
        }
    }

    /// <summary>
    /// Gets an unused object in the object pool, if it exists.
    /// </summary>
    /// <returns> Unused GameObject in object pool, or the oldest active one (LRU). </returns>
    public Item GetPooledObject()
    {
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
