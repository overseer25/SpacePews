using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : Item, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Sound")]
    public AudioClip hoverSound;
    public AudioClip swapSound;

    private Image image;
    private bool swapping;

    // Tracks and displays the quantity of this inventory item.
    private TextMeshProUGUI count;
    private AudioSource audioSource;

    // If swapping slots, send off these positions.
    private int[] positions;
    internal static bool dragging = false;
    internal bool destroying = false;
    internal bool hidden = true;
    private bool highlighted = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

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
        if (quantity != 0)
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
        Debug.Log("Dragged Type: " + this);
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

        if(destroying)
        {
            SendMessageUpwards("ClearSlot", positions[0]);
            destroying = false;
        }
        if (swapping)
        {
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
                positions[1] = collider.gameObject.GetComponentInParent<InventorySlot>().GetIndex();
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
    }
}
