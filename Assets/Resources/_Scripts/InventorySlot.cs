using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public Sprite empty; // The default image for the slot, used if no item is present.
    private Item item; // The item in the slot.
    private Image slot;
    public bool isEmpty = true; // All slots start out empty
    public int quantity = 0; // The amount of the item stored in inventory.

	// Use this for initialization
	void Start () {

        slot = GetComponentInChildren<Image>();
        slot.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }
	
	// Update is called once per frame
	void Update () {

        if(item != null)
        {
            slot.sprite = item.inventorySprite;
            GetComponentInChildren<Text>().text = quantity + "";
        }
        else
        {
            slot.sprite = empty;
        }
		
	}

    // Highlight the image when hovering over it
    void OnMouseOver()
    {
        slot.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        slot.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }

    public void SetItem(Item item)
    {
        isEmpty = false;
        this.item = item;
    }
    public Item GetItem()
    {
        return item;
    }
}
