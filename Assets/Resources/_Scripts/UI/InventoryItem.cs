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
    internal bool destroying = false;
    private bool highlighted = false;

    void Start()
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

        if (Input.GetMouseButton(0))
        {
            positions[0] = GetComponentInParent<InventorySlot>().GetIndex();
            dragging = true;
            SendMessageUpwards("HideHoverTooltip");
        }
    }

    /// <summary>
    /// Drag the item.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
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
        if (image == null) { return; }
        dragging = false;

        if(mounting)
        {
            if(item.type == mountSlot.type)
            {
                var result = Instantiate(item, mountSlot.GetInventoryItem().transform.position, Quaternion.identity, mountSlot.GetInventoryItem().transform) as Item;
                result.transform.parent = mountSlot.GetInventoryItem().transform;
                mountSlot.SetItem(result); // Add to the slot
                SendMessageUpwards("DeleteSlot", positions[0]);
                mounting = false;
            }
        }
        if (destroying)
        {
            SendMessageUpwards("DeleteSlot", positions[0]);
            destroying = false;
        }
        if (swapping)
        {
            SendMessageUpwards("SwapSlots", positions);
            swapping = false;
        }

        transform.localPosition = Vector3.zero;

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
                mountSlot = collider.gameObject.GetComponent<MountSlot>();
                positions[1] = collider.gameObject.GetComponentInParent<InventorySlot>().GetIndex();
                swapping = true;
                break;
            case ("MountSlot"):
                mountSlot = collider.gameObject.GetComponent<MountSlot>();
                mounting = true;
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
                mountSlot = collider.gameObject.GetComponent<MountSlot>();
                positions[1] = collider.gameObject.GetComponentInParent<InventorySlot>().GetIndex();
                swapping = true;
                break;
            case ("MountSlot"):
                mounting = true;
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
