using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : InteractableElement
{
    [Header("State")]
    public bool isEmpty = true; // All slots start out empty.
    private InventoryItem inventoryItem; // The item in the slot.
    private int quantity;

    // Use this for initialization
    void Awake()
    {
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }

    /// <summary>
    /// Increase the quantity of the item by 1.
    /// </summary>
    public void SetQuantity(int quantity)
    {
        this.quantity = quantity;
        inventoryItem.SetQuantity(quantity);
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
    /// Plays hover sound.
    /// </summary>
    void OnMouseEnter()
    {
        if (!isEmpty && !InventoryItem.dragging)
        {
            if (enterSound != null)
            {
                audioSource.clip = enterSound;
                audioSource.Play();
            }
        }
    }

    // Highlight the image when hovering over it
    void OnMouseOver()
    {
        // Shift clicking will clear the slot.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SendMessageUpwards("ClearSlot", index);
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            return;
        }

        if (!isEmpty && !InventoryItem.dragging)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            inventoryItem.Highlight();
            SendMessageUpwards("ShowHoverTooltip", index);

        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        if (!isEmpty)
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);

        if (inventoryItem.gameObject.activeSelf && !InventoryItem.dragging)
        {
            inventoryItem.Dehighlight();
            SendMessageUpwards("HideHoverTooltip");
            if (exitSound != null)
            {
                audioSource.clip = exitSound;
                audioSource.Play();
            }
        }
    }

    /// <summary>
    /// Deletes the item gameobject on the slot.
    /// </summary>
    public void DeleteSlot()
    {
        inventoryItem.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        inventoryItem.SetQuantity(0);
        Destroy(inventoryItem.item.gameObject);
        inventoryItem.gameObject.SetActive(false);
        isEmpty = true;
    }

    /// <summary>
    /// "Empty" the slot.
    /// </summary>
    public void ClearSlot()
    {
        inventoryItem.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        inventoryItem.SetQuantity(0);
        inventoryItem.SetItem(null, 0);
        inventoryItem.gameObject.SetActive(false);
        isEmpty = true;
    }

    /// <summary>
    /// Sets the item of the inventory slot.
    /// </summary>
    /// <param name="item"></param>
    public virtual void SetItem(Item item, int? quantity = null)
    {
        inventoryItem.gameObject.SetActive(true);
        this.quantity = (item.stackable) ? quantity ?? 1 : 0;
        inventoryItem.SetItem(item, this.quantity);
        if (item != null)
        {
            inventoryItem.item.gameObject.SetActive(false);
            isEmpty = false;
        }
        else
        {
            inventoryItem.gameObject.SetActive(false);
            isEmpty = true;
        }
    }

    // Gets the item of the inventory slot.
    public Item GetItem()
    {
        return inventoryItem.item;
    }

    /// <summary>
    /// Gets the count of the item in the slot.
    /// </summary>
    /// <returns></returns>
    public int GetQuantity()
    {
        return quantity;
    }
}
