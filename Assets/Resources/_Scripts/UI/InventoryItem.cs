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

    [Header("Sound")]
    public AudioClip hoverSound;
    public AudioClip swapSound;

    private Image image;
    private int quantity;
    private bool swapping;

    // For mounting slots.
    private bool mounting;
    private MountSlot mountSlot;

    // Tracks and displays the quantity of this inventory item.
    private TextMeshProUGUI count;
    private AudioSource audioSource;

    // If swapping slots, send off these positions.
    private int[] positions;
    internal static bool dragging = false;
    internal bool destroying = false;
    private bool highlighted = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        image = GetComponent<Image>();
        count = GetComponentInChildren<TextMeshProUGUI>();
        positions = new int[2];
        positions[0] = GetComponentInParent<InventorySlot>().GetIndex();
    }

    /// <summary>
    /// Set the quantity of the inventory item.
    /// </summary>
    public void SetQuantity(int num)
    {
        quantity = num;
    }

    public void SetItem(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;

        image.sprite = item.inventorySprite;
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }

    // Update is called once per frame.
    void FixedUpdate()
    {
        if(count != null)
        {
            if (item.stackable)
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
        if (!highlighted)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            audioSource.clip = hoverSound;
            audioSource.Play();
            highlighted = true;
        }
    }

    /// <summary>
    /// Dehighlight the image.
    /// </summary>
    public void Dehighlight()
    {
        if (highlighted)
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
            SendMessageUpwards("ClearSlot", positions[0]);
            mountSlot.SetItem(item);
            mounting = false;
        }
        if (destroying)
        {
            SendMessageUpwards("ClearSlot", positions[0]);
            destroying = false;
        }
        if (swapping)
        {
            if(mountSlot != null)
            {
                mountSlot.ClearSlot();
            }
            // Play the swap sound if swapping.
            audioSource.Stop();
            audioSource.clip = swapSound;
            audioSource.Play();

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
                Debug.Log("Hovering over inventory slot");
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
