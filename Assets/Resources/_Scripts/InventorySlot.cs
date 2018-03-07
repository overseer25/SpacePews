using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public Sprite empty; // The default image for the slot, used if no item is present.
    public GameObject tooltip;
    public Button infoButton;
    public Button destroyButton;
    public Button destroySelectedButton;
    public Image item_sprite;
    public bool isEmpty = true; // All slots start out empty
    public int quantity = 0; // The amount of the item stored in inventory.

    private Item item; // The item in the slot.
    private Image slot_sprite;
    private bool isDragging = false;

	// Use this for initialization
	void Start () {

        slot_sprite = GetComponentInChildren<Image>();
        slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);

        destroyButton.onClick.AddListener(Destroy);
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
        }
        else
        {
            item_sprite.enabled = false;
        }
		
	}

    // Highlight the image when hovering over it
    void OnMouseOver()
    {
        slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        item_sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        slot_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        item_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }

    // Handle mouse down input on the inventory slot
    void OnMouseDown()
    {
        // Left click
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = transform.position.z;

            tooltip.transform.position = newPos;
            tooltip.gameObject.SetActive(true);
        }
    }

    // Handle mouse up input on the inventory slot
    void OnMouseUp()
    {
        // Left click
        if (Input.GetMouseButtonUp(0))
        {
            tooltip.gameObject.SetActive(false);
        }
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

    void Destroy()
    {
        Debug.Log("Destroying");
        isEmpty = true;
        item = null;
        UpdateSlot();
    }

    void Information()
    {

    }
}
