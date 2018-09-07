using UnityEngine;
using UnityEngine.UI;

public class MountSlot : InteractableElement
{
    [Header("Attributes")]
    public ItemType type;
    public ItemTier tier;
    public ItemClass itemClass;

    // The mount associated with this slot.
    private ShipMount mount;
    private Image sprite_slot; // The default image for the slot.
    private InventoryItem inventoryItem;
    private int index;

    [HideInInspector]
    public bool isEmpty;

    void Start()
    {
        inventoryItem = GetComponentInChildren<InventoryItem>();
        sprite_slot = GetComponent<Image>();
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
    /// Sets the index of the slot.
    /// </summary>
    public void SetIndex(int i)
    {
        index = i;
    }

    /// <summary>
    /// Gets the index of the slot.
    /// </summary>
    /// <returns></returns>
    public int GetIndex()
    {
        return index;
    }

    /// <summary>
    /// Sets the item of the mount slot.
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item)
    {
        inventoryItem.SetItem(item, 0);
        inventoryItem.gameObject.SetActive(true);
        isEmpty = false;
    }

    /// <summary>
    /// Initialize a slot mount with the data of a mount.
    /// </summary>
    /// <param name="mount"></param>
    public void Initialize(ShipMount mount, int i)
    {
        this.mount = mount;
        index = i;
    }

}
