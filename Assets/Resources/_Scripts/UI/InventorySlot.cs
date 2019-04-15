using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : SlotBase
{

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        Dehighlight();
    }

    /// <summary>
    /// Plays hover sound.
    /// </summary>
    void OnMouseEnter()
    {
        if (canHighlight && !IsEmpty() && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
        {
            if (enterSound != null)
            {
                audioSource.clip = enterSound;
                audioSource.Play();
            }
        }
    }

    /// <summary>
    /// Highlight the slot.
    /// </summary>
    public override void Highlight()
    {
        if(image != null)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if(inventoryItem.GetItem() != null)
                inventoryItem.Highlight();
        }
    }

    /// <summary>
    /// Dehighlight the slot.
    /// </summary>
    public override void Dehighlight()
    {
        if(image != null)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            if (inventoryItem.GetItem() != null)
                inventoryItem.Dehighlight();
        }
    }

    /// <summary>
    /// Highlight the image when hovering over it
    /// </summary>
    void OnMouseOver()
    {
        // Shift right-clicking will swap slots.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        {
            if(inventoryItem.GetItem() != null)
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
            if(inventoryItem.GetItem() != null)
            {
                SendMessageUpwards("ClearSlot", index);
                Dehighlight();
                return;
            }
        }

        if(canHighlight)
        {
            if (!IsEmpty() && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
            {
                Highlight();
				InfoScreen.current.SetInfo(GetItem(), GetQuantity());
				InfoScreen.current.Show();
            }
            else if (IsEmpty())
            {
                Dehighlight();
				InfoScreen.current.Hide();
            }
        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        Dehighlight();
        if (inventoryItem.gameObject.activeSelf && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
        {
			InfoScreen.current.Hide();
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
        Dehighlight();
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
    /// Set the quantity of the item in this slot.
    /// </summary>
    /// <param name="quantity"></param>
    public void SetQuantity(int quantity)
    {
        if(inventoryItem.IsStackable())
            inventoryItem.SetQuantity(quantity);
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
