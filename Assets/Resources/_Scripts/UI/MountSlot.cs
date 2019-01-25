﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MountSlot : SlotBase
{
    [Header("Other")]
    public TextMeshProUGUI header;
    public TextMeshProUGUI footer;

    private ItemType type;
    private ItemTier tier;
    private ItemClass itemClass;
    // Distance from the physical mount.
    private float distanceFromMount;
    private float angleFromMount;
    private float speed = 7.0f;

    // The mount associated with this slot.
    private ShipMount mount;
    private LineRenderer line;
    private Color color;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        image = GetComponent<Image>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Reset position of slot when enabled.
    /// </summary>
    private void OnEnable()
    {
        if(mount != null)
            transform.position = mount.transform.position;
    }

    /// <summary>
    /// Smooth movement.
    /// </summary>
    void FixedUpdate()
    {
        if (mount != null)
        {
            var parentZ = mount.transform.parent.transform.rotation.eulerAngles.z;
            var x = mount.transform.position.x + distanceFromMount * Math.Cos(Math.PI / 180 * (angleFromMount + parentZ));
            var y = mount.transform.position.y + distanceFromMount * Math.Sin(Math.PI / 180 * (angleFromMount + parentZ));
            var vect = new Vector3((float)x, (float)y, gameObject.transform.parent.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, vect, Time.deltaTime * speed);
        }
    }

    private void LateUpdate()
    {
        Vector3 v = transform.position - mount.transform.position;
        Vector3 startPos = transform.position - (v / 3);
        Vector3 endPos = mount.transform.position + (v / 5);

        if (mount != null)
        {
            line.SetPositions(new Vector3[] { startPos, endPos });
        }
    }

    /// <summary>
    /// Get the physical mount associated with this UI mount.
    /// </summary>
    /// <returns></returns>
    public ShipMount GetMount()
    {
        return mount;
    }

    /// <summary>
    /// Get the type of mount.
    /// </summary>
    /// <returns></returns>
    public ItemType GetMountType()
    {
        return type;
    }


    /// <summary>
    /// Sets the item of the mount slot.
    /// </summary>
    /// <param name="item"></param>
    public override void SetItem(Item item)
    {
        var component = item as ShipComponent;
        component.mounted = true;
        inventoryItem.SetItem(component, 0);
        component = inventoryItem.GetItem() as ShipComponent;
        component.gameObject.SetActive(false);
        if (component is WeaponComponent)
            mount.SetComponent(component as WeaponComponent);
        else if (component is StorageComponent)
            mount.SetComponent(component as StorageComponent);
        else if (component is ThrusterComponent)
            mount.SetComponent(component as ThrusterComponent);
        else if (component is MiningComponent)
            mount.SetComponent(component as MiningComponent);
    }

    /// <summary>
    /// "Empty" the slot.
    /// </summary>
    public override void ClearSlot()
    {
        mount.ClearMount();
        inventoryItem.Clear();
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
        if (!IsEmpty() && !InventoryItem.dragging)
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

        // Shift right-clicking will swap slots.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        {
            SendMessageUpwards("QuickSwapWithInventorySlot", index);
        }

        // Shift clicking will clear the slot.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SendMessageUpwards("ClearSlot", index);
            image.color = new Color(color.r, color.g, color.b, 0.7f);
            return;
        }

        if (!IsEmpty() && !InventoryItem.dragging)
        {
            image.color = new Color(color.r, color.g, color.b, 1.0f);
            inventoryItem.Highlight();
            SendMessageUpwards("ShowHoverTooltip", index);

        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        if (!IsEmpty())
            image.color = new Color(color.r, color.g, color.b, 0.7f);

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
    public void Initialize(ShipMount mount, ItemType type, ItemTier tier, ItemClass itemClass, int index)
    {

        this.type = type;
        this.tier = tier;
        this.itemClass = itemClass;
        header.text = type.ToString();
        footer.text = itemClass.ToString();
        color = ItemColors.colors[(int)tier];
        image.color = new Color(color.r, color.g, color.b, 0.7f);

        this.mount = mount;
        type = mount.GetMountType();
        tier = mount.GetMountTier();
        itemClass = mount.GetMountClass();
        distanceFromMount = mount.distanceFromShip;
        angleFromMount = mount.uiAngle;

        if (mount.startingComponent != null)
        {
            SetItem(mount.startingComponent);
        }
        this.index = index;
    }

}
