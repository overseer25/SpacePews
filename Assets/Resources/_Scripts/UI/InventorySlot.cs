using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : SlotBase
{

    // Use this for initialization
    void Awake()
    {
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }

    /// <summary>
    /// Plays hover sound.
    /// </summary>
    void OnMouseEnter()
    {
        if (!IsEmpty() && !InventoryItem.dragging)
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
        // Shift right-clicking will swap slots.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        {
            if(inventoryItem != null)
            {
                if (inventoryItem.GetItemType() == ItemType.Turret)
                    SendMessageUpwards("QuickSwapWithHotbarSlot", index);
                else if (inventoryItem.GetItemType() != ItemType.Item)
                    SendMessageUpwards("QuickSwapWithMountSlot", index);
            }
        }

        // Shift clicking will clear the slot.
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SendMessageUpwards("ClearSlot", index);
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            return;
        }

        if (!IsEmpty() && !InventoryItem.dragging)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            inventoryItem.Highlight();
            SendMessageUpwards("ShowHoverTooltip", index);

        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        if (!IsEmpty())
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
    /// "Empty" the slot.
    /// </summary>
    public override void ClearSlot()
    {
        inventoryItem.Clear();
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }

    /// <summary>
    /// Sets the item of the inventory slot.
    /// </summary>
    /// <param name="item"></param>
    public override void SetItem(Item item)
    {
        SetItem(item, 0);
    }

    /// <summary>
    /// Sets the item of the inventory slot, with a quantity supplied.
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item, int quantity)
    {
        if (item == null)
            return;
        else
        {
            inventoryItem.SetItem(item, quantity);
        }
    }

    /// <summary>
    /// Gets the count of the item in the slot.
    /// </summary>
    /// <returns></returns>
    public int GetQuantity()
    {
        return inventoryItem.GetQuantity();
    }
}
