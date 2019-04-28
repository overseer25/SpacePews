using System.Collections;
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
    internal bool canHighlight = false; // Allow the slot to highlight when hovered over.

    // For the flash animation. If this is true, then the slot is becoming more opaque.
    private bool increaseAlpha;

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
    }

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
    /// Get the inventory item of this slot.
    /// </summary>
    /// <returns></returns>
    public InventoryItem GetInventoryItem()
    {
        return inventoryItem;
    }

    /// <summary>
    /// Gets the item of the slot.
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return inventoryItem.GetItem();
    }

    /// <summary>
    /// Is the slot empty?
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return inventoryItem.GetItem() == null;
    }

    /// <summary>
    /// Check to see if this slot has the same item as another slot.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool ContainsSameItem(SlotBase other)
    {
        var thisItem = inventoryItem.GetItem();
        var otherItem = other.inventoryItem.GetItem();

        if (thisItem == null && otherItem == null) return true;
        else if (thisItem == null) return false;
        else if (otherItem == null) return false;

        return thisItem.itemName == otherItem.itemName;
    }

    /// <summary>
    /// Play a flashing animation for the slot.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Flash()
    {
        if(!increaseAlpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - 0.02f);
        }
        else
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + 0.02f);
        }

        if (image.color.a <= 0.4f)
            increaseAlpha = true;
        else if (image.color.a >= 1.0f)
            increaseAlpha = false;

        yield return new WaitForSeconds(0.1f);
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

    /// <summary>
    /// Highlight the slot and its item.
    /// </summary>
    public abstract void Highlight();

    /// <summary>
    /// Dehighlight the slot and its item.
    /// </summary>
    public abstract void Dehighlight();
}
