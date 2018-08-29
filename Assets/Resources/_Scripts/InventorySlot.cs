using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public Sprite empty; // The default image for the slot, used if no item is present.
    public Image item_sprite;
    public bool isEmpty = true; // All slots start out empty
    public int quantity = 0; // The amount of the item stored in inventory.

    private Item item; // The item in the slot.
    private Image slot_sprite;
    private bool isDragging = false;
    private bool tooltipActive = false;
    private int index;

	// Use this for initialization
	void Start () {

        slot_sprite = GetComponentInChildren<Image>();
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
    /// Update the item and quantity of the slot. Called whenever Inventory attempts to add and item.
    /// </summary>
	public void UpdateSlot () {

        if(item != null)
        {
            item_sprite.enabled = true;
            item_sprite.sprite = item.sprite;
            item_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            GetComponentInChildren<Text>().text = quantity + "";
            GetComponentInChildren<Text>().enabled = true;
        }
        else
        {
            item_sprite.enabled = false;
            GetComponentInChildren<Text>().enabled = false;
        }
		
	}

    // Highlight the image when hovering over it
    void OnMouseOver()
    {
        slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        item_sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        SendMessageUpwards("HoverTooltip", index);
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        item_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }


    public void SetItem(Item item)
    {
        if(item == null)
        {
            isEmpty = true;
            quantity = 0;
        }
        else
        {
            isEmpty = false;
        }
        
        this.item = item;
    }
    public Item GetItem()
    {
        return item;
    }

}
