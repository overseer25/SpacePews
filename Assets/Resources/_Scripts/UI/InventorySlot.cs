using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : InteractableElement
{
    [Header("State")]
    public bool isEmpty = true; // All slots start out empty
    public InventoryItem inventoryItem; // The item in the slot.

    private Image slot_sprite; // The default image for the slot.
    private int index;
    private int quantity;

    // Use this for initialization
    void Awake()
    {
        slot_sprite = GetComponent<Image>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
        slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }

    /// <summary>
    /// Sets the index of the slot in the inventory slot array.
    /// </summary>
    /// <param name="index"></param>
    public void SetIndex(int index)
    {
        this.index = index;
    }

    /// <summary>
    /// Get the index of the slot.
    /// </summary>
    /// <param name="index"></param>
    public int GetIndex()
    {
        return index;
    }

    /// <summary>
    /// Increase the quantity of the item by 1.
    /// </summary>
    public void IncrementQuantity()
    {
        quantity++;
        inventoryItem.SetQuantity(quantity);
    }

    // Highlight the image when hovering over it
    void OnMouseOver()
    {
        // Shift clicking will clear the slot.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SendMessageUpwards("ClearSlot", index);
            slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            return;
        }

        if (!isEmpty && !InventoryItem.dragging)
        {
            slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            inventoryItem.Highlight();
            SendMessageUpwards("ShowHoverTooltip", index);
        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        if (!isEmpty)
            slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);

        if (inventoryItem.gameObject.activeSelf && !InventoryItem.dragging)
        {
            inventoryItem.Dehighlight();
            SendMessageUpwards("HideHoverTooltip");
        }
    }

    /// <summary>
    /// "Empty" the slot.
    /// </summary>
    public void ClearSlot()
    {
        inventoryItem.gameObject.SetActive(false);
        isEmpty = true;
    }

    /// <summary>
    /// Sets the item of the inventory slot.
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item)
    {
        quantity = 1;
        inventoryItem.SetItem(item, quantity);
        inventoryItem.gameObject.SetActive(true);
        isEmpty = false;
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
