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

    protected override void Awake()
    {
        base.Awake();
        line = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Reset position of slot when enabled.
    /// </summary>
    private void OnEnable()
    {
        if(mount != null)
            transform.position = new Vector3(mount.transform.position.x, mount.transform.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        if (mount != null)
        {
            // Slot position
            var parentZ = mount.transform.rotation.eulerAngles.z;
            var x = mount.transform.position.x + distanceFromMount * Math.Cos(Math.PI / 180 * (angleFromMount + parentZ));
            var y = mount.transform.position.y + distanceFromMount * Math.Sin(Math.PI / 180 * (angleFromMount + parentZ));
            var vect = new Vector3((float)x, (float)y, gameObject.transform.parent.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, vect, Time.deltaTime * speed);

            // Line position
            Vector3 v = transform.position - mount.transform.position;
            Vector3 startPos = (transform.position) - (v / 3);
            Vector3 endPos = mount.transform.position + (v / 5);
            line.SetPositions(new Vector3[] { startPos, new Vector3(endPos.x, endPos.y, transform.position.z) });
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
		var component = item as ShipComponentBase;
		component.mounted = true;
		mount.SetComponent(component);
		inventoryItem.SetItem(mount.GetShipComponent(), 0, false);
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
    /// Highlight the slot.
    /// </summary>
    public override void Highlight()
    {
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
            if (inventoryItem != null)
                inventoryItem.Highlight();
        }
    }

    /// <summary>
    /// Dehighlight the slot.
    /// </summary>
    public override void Dehighlight()
    {
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.7f);
            if (inventoryItem != null)
                inventoryItem.Dehighlight();
        }
    }

    /// <summary>
    /// Plays hover sound.
    /// </summary>
    void OnMouseEnter()
    {
        if (canHighlight && !IsEmpty() && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
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

        // Right-clicking will swap slots.
        if (Input.GetMouseButtonDown(1))
        {
            SendMessageUpwards("QuickSwapWithInventorySlot", index);
        }

        if(canHighlight)
        {
            if (!IsEmpty() && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
            {
                Highlight();
				InfoScreen.current.SetInfo(GetItem());
				InfoScreen.current.Show();
			}
            else if (IsEmpty())
            {
                Dehighlight();
				InfoScreen.current.Hide();
            }
        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        Dehighlight();

        if (inventoryItem.gameObject.activeSelf && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
        {
			InfoScreen.current.Hide();
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
