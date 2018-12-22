using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Item")]
    public Item item;

    private Image image;
    private int quantity;
    private bool swapping;

    // For mounting slots.
    private bool mounting;
    private MountSlot mountSlot;

    // Tracks and displays the quantity of this inventory item.
    private TextMeshProUGUI count;

    // If swapping slots, send off these positions.
    private int[] positions;
    internal static bool dragging = false;
    // If the inventory item is not visible, it is not draggable.
    internal static bool draggable;
    internal bool destroying = false;
    private bool highlighted = false;

    void Awake()
    {
        image = GetComponent<Image>();
        count = GetComponentInChildren<TextMeshProUGUI>();
        positions = new int[2];
        positions[0] = GetComponentInParent<InteractableElement>().GetIndex();
    }

    /// <summary>
    /// Set the quantity of the inventory item.
    /// </summary>
    public void SetQuantity(int num)
    {
        quantity = num;

        if (count != null)
        {
            if (item != null && item.stackable && num > 0)
                count.text = quantity.ToString();
            else
                count.text = "";
        }
    }

    public void SetItem(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;

        image.sprite = (item != null) ? item.inventorySprite : null;
        image.color = (item != null) ? new Color(1.0f, 1.0f, 1.0f, 0.7f) : new Color(1.0f, 1.0f, 1.0f, 0.0f);

        if (count != null)
        {
            if (item != null && item.stackable && quantity > 0)
                count.text = quantity.ToString();
            else
                count.text = "";
        }
        gameObject.SetActive(true);
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
        if (!highlighted && item != null)
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
        if (highlighted && item != null)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            highlighted = false;
        }
    }

    /// <summary>
    /// Disable the tool tip when dragging.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {

        if (Input.GetMouseButton(0) && draggable)
        {
            if(GetComponentInParent<InteractableElement>() != null)
                positions[0] = GetComponentInParent<InteractableElement>().GetIndex();
            dragging = true;
            SendMessageUpwards("HideHoverTooltip");
        }
    }

    /// <summary>
    /// Drag the item.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && draggable)
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
        if (image == null || !draggable) { return; }
        dragging = false;

        if (destroying)
        {
            SendMessageUpwards("DeleteSlot", positions[0]);
            destroying = false;
        }
        if (swapping || mounting)
        {
            SendMessageUpwards("SwapSlots", positions);
            swapping = false;
        }

        transform.position = GetComponentInParent<InteractableElement>().gameObject.transform.position;

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
                swapping = true;
                break;
            case ("MountSlot"):
                mountSlot = collider.gameObject.GetComponent<MountSlot>();
                positions[1] = mountSlot.GetIndex();
                mounting = true;
                break;
            case ("HotbarSlot"):
                var hotbarSlot = collider.gameObject.GetComponent<HotbarSlot>();
                positions[1] = hotbarSlot.GetIndex();
                swapping = true;
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
                swapping = true;
                break;
            case ("MountSlot"):
                mountSlot = collider.gameObject.GetComponent<MountSlot>();
                positions[1] = mountSlot.GetIndex();
                mounting = true;
                break;
            case ("HotbarSlot"):
                positions[1] = collider.gameObject.GetComponent<HotbarSlot>().GetIndex();
                swapping = true;
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
        destroying = false;
        mounting = false;
        mountSlot = null;
    }
}
