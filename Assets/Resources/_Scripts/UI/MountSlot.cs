﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class MountSlot : InteractableElement
{
    [Header("Attributes")]
    public ItemType type;
    public ItemTier tier;
    public ItemClass itemClass;
    [Header("Other")]
    // Distance from the physical mount.
    private float distanceFromMount;
    private float angleFromMount;
    private float speed = 7.0f;

    // The mount associated with this slot.
    private ShipMount mount;
    private InventoryItem inventoryItem;
    private LineRenderer line;

    [HideInInspector]
    public bool isEmpty;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        image = GetComponent<Image>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
        audioSource = GetComponent<AudioSource>();
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Reset position of slot when enabled.
    /// </summary>
    private void OnEnable()
    {
        transform.position = mount.transform.position;
    }

    /// <summary>
    /// Smooth movement.
    /// </summary>
    void FixedUpdate()
    {
        if (mount != null)
        {
            var x = mount.transform.position.x + distanceFromMount * Math.Cos(Math.PI / 180 * (angleFromMount + mount.transform.rotation.eulerAngles.z));
            var y = mount.transform.position.y + distanceFromMount * Math.Sin(Math.PI / 180 * (angleFromMount + mount.transform.rotation.eulerAngles.z));
            var vect = new Vector3((float)x, (float)y, gameObject.transform.parent.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, vect, Time.deltaTime * speed);
        }
    }

    private void LateUpdate()
    {
        Vector3 v = transform.position - mount.transform.position;
        Vector3 startPos = transform.position - (v / 5);
        Vector3 endPos = mount.transform.position + (v / 5);

        if (mount != null)
        {
            line.SetPositions(new Vector3[] { startPos, endPos });
        }
    }

    /// <summary>
    /// Checks to see if the provided component can fit in this mount.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool IsComponentCompatible(ShipComponent component)
    {
        if (component == null)
            return false;
        return type == component.GetItemType() && tier == component.GetComponentTier() && itemClass == component.GetComponentClass();
    }

    /// <summary>
    /// Get the inventory item of this mount slot.
    /// </summary>
    /// <returns></returns>
    public InventoryItem GetInventoryItem()
    {
        return inventoryItem;
    }

    /// <summary>
    /// Gets the item of the mount slot.
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return inventoryItem.item;
    }

    /// <summary>
    /// Sets the item of the mount slot.
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item)
    {
        inventoryItem.SetItem(item, 0);
        inventoryItem.gameObject.SetActive(true);
        if (item is WeaponComponent)
            mount.SetComponent(item as WeaponComponent);
        isEmpty = false;
    }

    /// <summary>
    /// Deletes the item gameobject on the slot.
    /// </summary>
    public void DeleteSlot()
    {
        mount.ClearMount();
        inventoryItem.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        inventoryItem.SetQuantity(0);
        inventoryItem.SetItem(null, 0);
        inventoryItem.gameObject.SetActive(false);
        isEmpty = true;
    }

    /// <summary>
    /// "Empty" the slot.
    /// </summary>
    public void ClearSlot()
    {
        mount.ClearMount();
        inventoryItem.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        inventoryItem.SetQuantity(0);
        inventoryItem.SetItem(null, 0);
        inventoryItem.gameObject.SetActive(false);
        isEmpty = true;
    }

    /// <summary>
    /// Compare this mount slot to another, comparing their type, tier, and class.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsSameSlotType(MountSlot other)
    {
        return type == other.type && tier == other.tier && itemClass == other.itemClass;
    }
    /// <summary>
    /// Plays hover sound.
    /// </summary>
    void OnMouseEnter()
    {
        if (!isEmpty && !InventoryItem.dragging)
        {
            if (enterSound != null)
            {
                audioSource.clip = enterSound;
                audioSource.Play();
            }
        }
    }

    // Highlight the image when hovering over it
    void OnMouseOver()
    {
        // Shift clicking will clear the slot.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SendMessageUpwards("ClearSlot", index);
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            return;
        }

        if (!isEmpty && !InventoryItem.dragging)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            inventoryItem.Highlight();
            SendMessageUpwards("ShowHoverTooltip", index);

        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        if (!isEmpty)
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);

        if (inventoryItem.gameObject.activeSelf && !InventoryItem.dragging)
        {
            inventoryItem.Dehighlight();
            SendMessageUpwards("HideHoverTooltip");
            if (exitSound != null)
            {
                audioSource.clip = exitSound;
                audioSource.Play();
            }
        }
    }

    /// <summary>
    /// Initialize a slot mount with the data of a mount.
    /// </summary>
    /// <param name="mount"></param>
    public void Initialize(ShipMount mount, int i)
    {
        this.mount = mount;
        type = mount.GetMountType();
        tier = mount.GetMountTier();
        itemClass = mount.GetMountClass();
        distanceFromMount = mount.distanceFromShip;
        angleFromMount = mount.uiAngle;

        if (mount.GetShipComponent() != null)
        {
            inventoryItem.SetItem(mount.GetShipComponent(), 0);
            inventoryItem.gameObject.SetActive(true);
            isEmpty = false;
        }
        index = i;
    }

}
