using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // This is used for debugging and testing.
    public Item StartingItem;
    public int StartingQuantity;

    // Item data
    private int quantity;
    private bool swapping;
    private bool rightClickSwapping;
    private Item item;
    private int animFrameIndex;

    private Image image;
    // For mounting slots.
    private bool mounting;
    private MountSlot mountSlot;

    // Tracks and displays the quantity of this inventory item.
    private TextMeshProUGUI count;

    // If swapping slots, send off these positions.
    private int[] positions;
    internal static bool dragging = false;
    internal static bool rightClickDragging = false;
    // If the inventory item is not visible, it is not draggable.
    internal static bool draggable;
    internal bool destroying = false;
    private bool highlighted = false;

    void Awake()
    {
        image = GetComponent<Image>();
        count = GetComponentInChildren<TextMeshProUGUI>();
        positions = new int[2];
        positions[0] = GetComponentInParent<SlotBase>().GetIndex();

        if (StartingItem != null)
        {
            if (!StartingItem.stackable || StartingQuantity > 0)
            {
                SetItem(StartingItem, StartingQuantity);
            }
        }
    }

    private void Update()
    {
        if(item != null && item.sprites.Length > 0)
        {
            // Player sprite animation
            if (Time.time > item.changeSprite)
            {
                item.changeSprite = Time.time + item.playspeed;
                animFrameIndex++;
                if (animFrameIndex >= item.sprites.Length) { animFrameIndex = 0; } // Restart animation
                image.sprite = item.sprites[animFrameIndex];
            }
        }
        if (Input.GetMouseButtonDown(1) && draggable && highlighted)
        {
            if (rightClickDragging)
                OnDrag(null);
            else
                OnBeginDrag(null);
        }
        else if(Input.GetMouseButtonUp(1) && rightClickDragging)
        {
            OnEndDrag(null);
        }
           
    }

    /// <summary>
    /// Set the item.
    /// </summary>
    /// <param name="incomingItem"></param>
    /// <param name="quantity"></param>
    public void SetItem(Item incomingItem, int quantity)
    {
        this.quantity = quantity;

        // If the current item for this inventory item is not empty, destroy the old item first.
        if (item != null)
        {
            Destroy(item.gameObject);
            item = Instantiate(incomingItem, transform.position, Quaternion.identity, transform) as Item;
        }
        else if (incomingItem != null)
            item = Instantiate(incomingItem, transform.position, Quaternion.identity, transform) as Item;
        else
            Clear();

        // Hide the item so that it isn't displayed in the game world.
        item.gameObject.SetActive(false);

        image.sprite = (item != null) ? item.sprites[0] : null;
        image.color = (item != null) ? new Color(1.0f, 1.0f, 1.0f, 0.7f) : new Color(1.0f, 1.0f, 1.0f, 0.0f);

        if (count != null)
        {
            if (item != null && item.stackable)
                count.text = quantity.ToString();
            else
                count.text = "";
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Get the item.
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return item;
    }

    /// <summary>
    /// Set the quantity of the item represented by this inventory item. Typically called when the item represented
    /// already exists in the inventory, is stackable, and the player picked up more of this item or removed some.
    /// </summary>
    /// <param name="quantity"></param>
    public void SetQuantity(int quantity)
    {
        if (!IsStackable())
            return;
        
        this.quantity = quantity;
        count.text = this.quantity + "";
    }

    /// <summary>
    /// Add to the current quantity of this inventory item.
    /// </summary>
    /// <param name="quantity"></param>
    public void AddQuantity(int quantity)
    {
        SetQuantity(this.quantity + quantity);
    }

    /// <summary>
    /// Subtract from the current quantity of this inventory item.
    /// </summary>
    /// <param name="quantity"></param>
    public void SubtractQuantity(int quantity)
    {
        SetQuantity(this.quantity - quantity);
    }

    /// <summary>
    /// Get the quantity of the item represented by this inventory item.
    /// </summary>
    /// <returns></returns>
    public int GetQuantity()
    {
        return quantity;
    }

    /// <summary>
    /// Get the item type of the item represented by this inventory item.
    /// </summary>
    /// <returns></returns>
    public ItemType GetItemType()
    {
        return item.GetItemType();
    }

    /// <summary>
    /// Get whether the item represented by this inventory item is stackable.
    /// </summary>
    /// <returns></returns>
    public bool IsStackable()
    {
        return item.stackable;
    }

    /// <summary>
    /// Get the maximum stack size of the item. If the item is not stackable, the method will return 0.
    /// </summary>
    /// <returns></returns>
    public int GetStackSize()
    {
        return item.stackSize;
    }

    /// <summary>
    /// Called when the gameobject is disabled.
    /// </summary>
    void OnDisable()
    {
        highlighted = false;
    }

    /// <summary>
    /// Highlight the image.
    /// </summary>
    public void Highlight()
    {
        if (!highlighted && image != null)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            highlighted = true;
        }
    }

    /// <summary>
    /// Dehighlight the image.
    /// </summary>
    public void Dehighlight()
    {
        if (highlighted && image != null)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            highlighted = false;
        }
    }

    /// <summary>
    /// Clear the inventory item.
    /// </summary>
    public void Clear()
    {
        image.sprite = null;
        quantity = 0;
        Destroy(item.gameObject);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Disable the tool tip when dragging.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && draggable && item != null)
        {
            if(GetComponentInParent<SlotBase>() != null)
                positions[0] = GetComponentInParent<SlotBase>().GetIndex();
            dragging = true;
            SendMessageUpwards("HideHoverTooltip");
        }
        else if(Input.GetMouseButtonDown(1) && draggable && item != null)
        {
            if (GetComponentInParent<SlotBase>() != null)
                positions[0] = GetComponentInParent<SlotBase>().GetIndex();
            rightClickDragging = true;
            SendMessageUpwards("HideHoverTooltip");
        }

    }

    /// <summary>
    /// Drag the item.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && draggable && item != null)
        {
            if (image == null) { return; }

            var newPos = Input.mousePosition;
            newPos.z = transform.position.z - Camera.main.transform.position.z;
            transform.position = Camera.main.ScreenToWorldPoint(newPos);
        }
        else if (Input.GetMouseButton(1) && draggable && item != null)
        {
            if (image == null) { return; }

            var newPos = Input.mousePosition;
            newPos.z = transform.position.z - Camera.main.transform.position.z;
            transform.position = Camera.main.ScreenToWorldPoint(newPos);
        }

    }

    /// <summary>
    /// Return the item to it's original position, if no valid slot is present.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (image == null || !draggable || item == null) { return; }
        dragging = false;
        rightClickDragging = false;

        if (destroying)
        {
            SendMessageUpwards("DeleteSlot", positions[0]);
            destroying = false;
        }
        else if (swapping || mounting)
        {
            SendMessageUpwards("SwapSlots", positions);
            swapping = false;
        }
        else if(rightClickSwapping)
        {
            SendMessageUpwards("PromptUserForAmountToSwap", positions);
            rightClickSwapping = false;
        }

        transform.position = GetComponentInParent<SlotBase>().gameObject.transform.position;

    }

    /// <summary>
    /// Deals with changing slots.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case ("InventorySlot"):
                positions[1] = collider.gameObject.GetComponentInParent<InventorySlot>().GetIndex();
                if (dragging)
                    swapping = true;
                else if (rightClickDragging)
                    rightClickSwapping = true;
                break;
            case ("MountSlot"):
                mountSlot = collider.gameObject.GetComponent<MountSlot>();
                positions[1] = mountSlot.GetIndex();
                mounting = true;
                break;
            case ("HotbarSlot"):
                var hotbarSlot = collider.gameObject.GetComponent<HotbarSlot>();
                positions[1] = hotbarSlot.GetIndex();
                if (dragging)
                    swapping = true;
                else if (rightClickDragging)
                    rightClickSwapping = true;
                break;
            case ("DeleteZone"):
                collider.gameObject.GetComponent<Image>();
                destroying = true;
                break;
        }
    }

    /// <summary>
    /// Deals with changing slots.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerStay2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case ("InventorySlot"):
                positions[1] = collider.gameObject.GetComponentInParent<InventorySlot>().GetIndex();
                if (dragging)
                    swapping = true;
                else if (rightClickDragging)
                    rightClickSwapping = true;
                break;
            case ("MountSlot"):
                mountSlot = collider.gameObject.GetComponent<MountSlot>();
                positions[1] = mountSlot.GetIndex();
                mounting = true;
                break;
            case ("HotbarSlot"):
                positions[1] = collider.gameObject.GetComponent<HotbarSlot>().GetIndex();
                if (dragging)
                    swapping = true;
                else if (rightClickDragging)
                    rightClickSwapping = true;
                break;
            case ("DeleteZone"):
                destroying = true;
                break;
        }
    }

    /// <summary>
    /// Deals with resetting newSlotPosition to null.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerExit2D(Collider2D collider)
    {
        swapping = false;
        rightClickSwapping = false;
        destroying = false;
        mounting = false;
        mountSlot = null;
    }
}
