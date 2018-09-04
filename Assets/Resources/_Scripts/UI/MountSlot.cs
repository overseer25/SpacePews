using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MountSlot : InventorySlot
{
    // The mount associated with this slot.
    private ShipMount mount;
    private Image sprite_slot; // The default image for the slot.

    void Start()
    {
        sprite_slot = GetComponent<Image>();
    }

    void Update()
    {

    }

    /// <summary>
    /// Sets the item of the mount slot.
    /// </summary>
    /// <param name="item"></param>
    public override void SetItem(Item item)
    {
        inventoryItem.SetItem(item, 0);
        inventoryItem.gameObject.SetActive(true);
        isEmpty = false;
    }

    /// <summary>
    /// Initialize a slot mount with the data of a mount.
    /// </summary>
    /// <param name="mount"></param>
    public void Initialize(ShipMount mount)
    {
        this.mount = mount;
    }

}
