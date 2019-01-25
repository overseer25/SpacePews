using UnityEngine;
using UnityEngine.UI;

public abstract class SlotBase : MonoBehaviour
{

    [Header("Sounds")]
    public AudioClip enterSound;
    public AudioClip exitSound;

    // Highlight the image when hovering over it
    internal Image image;
    internal int index;
    internal InventoryItem inventoryItem;
    internal AudioSource audioSource;

    /// <summary>
    /// Sets the index of the slot.
    /// </summary>
    public void SetIndex(int i)
    {
        index = i;
    }

    /// <summary>
    /// Gets the index of the slot.
    /// </summary>
    /// <returns></returns>
    public int GetIndex()
    {
        return index;
    }

    /// <summary>
    /// Get the inventory item of this mount slot.
    /// </summary>
    /// <returns></returns>
    public InventoryItem GetInventoryItem()
    {
        return inventoryItem;
    }

    /// <summary>
    /// Gets the item of the mount slot.
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return inventoryItem.GetItem();
    }

    /// <summary>
    /// Is the inventory slot empty?
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return inventoryItem.GetItem() == null;
    }

    /// <summary>
    /// Sets the item of the slot.
    /// </summary>
    /// <param name="item"></param>
    public abstract void SetItem(Item item);

    /// <summary>
    /// Clear the slot of its item.
    /// </summary>
    public abstract void ClearSlot();
}
