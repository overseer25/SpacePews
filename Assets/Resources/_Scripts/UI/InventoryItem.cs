using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : Item, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Image image;
    private bool canSwap;

    // Tracks and displays the quantity of this inventory item.
    private TextMeshProUGUI count;

    // If swapping slots, send off these positions.
    private int[] positions;
    internal bool dragging = false;
    internal bool hidden = true;

    void Start()
    {
        image = GetComponent<Image>();
        count = GetComponentInChildren<TextMeshProUGUI>();
        positions = new int[2];
        positions[0] = GetComponentInParent<InventorySlot>().GetIndex();
    }

    public void Display()
    {
        if (hidden)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            image.sprite = sprite;
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        }
    }

    // Update is called once per frame.
    void FixedUpdate()
    {
        if(quantity != 0)
            count.text = quantity.ToString();

        if (spriteAnim.Length > 0)
        {
            // Player sprite animation
            if (Time.time > changeSprite)
            {
                changeSprite = Time.time + playspeed;
                index++;
                if (index >= spriteAnim.Length) { index = 0; } // Restart animation
                GetComponentInChildren<SpriteRenderer>().sprite = spriteAnim[index];
            }
        }
    }

    /// <summary>
    /// Highlight the image.
    /// </summary>
    public void Highlight()
    {
        image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    /// <summary>
    /// Dehighlight the image.
    /// </summary>
    public void Dehighlight()
    {
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }

    /// <summary>
    /// Disable the tool tip when dragging.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        positions[0] = GetComponentInParent<InventorySlot>().GetIndex();
        dragging = true;
        SendMessageUpwards("HideHoverTooltip");
    }

    /// <summary>
    /// Drag the item.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (image == null) { return; }

        var newPos = Input.mousePosition;
        newPos.z = transform.position.z - Camera.main.transform.position.z;
        transform.position = Camera.main.ScreenToWorldPoint(newPos);
    }

    /// <summary>
    /// Return the item to it's original position, if no valid slot is present.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (image == null) { return; }

        dragging = false;

        if (canSwap)
        {
            SendMessageUpwards("SwapSlots", positions);
            canSwap = false;
        }

        transform.localPosition = Vector3.zero;

    }

    /// <summary>
    /// Deals with object collisions.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case ("InventorySlot"):
                positions[1] = collider.gameObject.GetComponentInParent<InventorySlot>().GetIndex();
                canSwap = true;
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
                canSwap = true;
                break;
        }
    }

    /// <summary>
    /// Deals with resetting newSlotPosition to null.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerExit2D(Collider2D collider)
    {
        canSwap = false;
    }
}
