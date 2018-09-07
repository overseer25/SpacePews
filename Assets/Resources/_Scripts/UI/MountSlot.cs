using System;
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
    private float speed = 3.0f;

    // The mount associated with this slot.
    private ShipMount mount;
    private InventoryItem inventoryItem;

    [HideInInspector]
    public bool isEmpty;

    void Awake()
    {
        image = GetComponent<Image>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
        audioSource = GetComponent<AudioSource>();
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (mount != null)
        {
            var x = Camera.main.ScreenToWorldPoint(mount.transform.position).x + distanceFromMount * Math.Cos(Math.PI / 180 * angleFromMount);
            var y = Camera.main.ScreenToWorldPoint(mount.transform.position).y + distanceFromMount * Math.Sin(Math.PI / 180 * angleFromMount);
            var vect = new Vector3((float)x, (float)y, mount.transform.position.z);
            transform.position = Vector3.Slerp(transform.position, vect, Time.deltaTime * speed);
        }
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
        type = mount.GetMountType();
        tier = mount.GetMountTier();
        itemClass = mount.GetMountClass();
        distanceFromMount = mount.distanceFromShip;
        angleFromMount = mount.uiAngle;
        index = i;
    }

}
