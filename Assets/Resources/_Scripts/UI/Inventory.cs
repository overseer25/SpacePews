using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Tool Tip")]
    public InfoScreen infoScreen;

    [Header("Sounds")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip swapSound;
    public AudioClip clearSlotSound;

    private List<InventorySlot> inventorySlots;
    internal AudioSource audioSource;
    internal bool isOpen = false;

    [Header("Other")]
    [SerializeField]
    private ShipMountController mountController;
    [SerializeField]
    private GameObject inventorySlot;
    [SerializeField]
    private GameObject mountUI;
    [SerializeField]
    private GameObject inventoryUI;
    private ShipMount[] mounts;
    private List<MountSlot> mountSlotsUI;

    // All possible items in the game. This reference is required in order to maintain inventory sprites when objects are destroyed.
    private List<Item> itemList;
    private int inventorySize;


    // Use this for initialization
    void Start()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>().ToList();

        var i = 0;
        foreach (var slot in inventorySlots)
            slot.SetIndex(i++);

        itemList = new List<Item>();
        foreach (var obj in Resources.LoadAll("_Prefabs/Items"))
        {
            GameObject itemObj = obj as GameObject;
            itemList.Add(itemObj.GetComponent<Item>());
        }
        audioSource = GetComponent<AudioSource>();

        mountSlotsUI = new List<MountSlot>();
        mounts = mountController.GetAllMounts();
        foreach (var mount in mounts)
        {
            var slot = Instantiate(Resources.Load("_Prefabs/UI/MountingSystem/MountSlot"), mount.transform.position, mount.transform.rotation, mountUI.transform) as GameObject;
            slot.GetComponent<MountSlot>().Initialize(mount, mount.GetMountType(), mount.GetMountTier(), mount.GetMountClass(), i++);
            mountSlotsUI.Add(slot.GetComponent<MountSlot>());
        }
    }

    /// <summary>
    /// Adds slots to the inventory.
    /// </summary>
    /// <param name="size"></param>
    public void AddSlots(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var slot = Instantiate(inventorySlot, inventoryUI.transform);
            slot.GetComponent<InventorySlot>().SetIndex(i + inventorySize);
            inventorySlots.Add(slot.GetComponent<InventorySlot>());
        }
        int j = size + inventorySize;
        foreach (var mountSlot in mountSlotsUI)
            mountSlot.SetIndex(j++);

        inventorySize += size;
    }

    /// <summary>
    /// Remove Slots from the inventory.
    /// </summary>
    /// <param name="size"></param>
    public void RemoveSlots(int size)
    {
        for (int i = inventorySize - 1; i > inventorySize - 1 - size; i--)
        {
            var slot = inventorySlots[i];
            inventorySlots.RemoveAt(i);
            Destroy(slot.gameObject);
        }
        int j = inventorySize - size;
        foreach (var mountSlot in mountSlotsUI)
            mountSlot.SetIndex(j++);

        inventorySize -= size;
    }

    /// <summary>
    /// Called to toggle display to inventory UI.
    /// </summary>
    public virtual void Toggle()
    {
        if (!isOpen)
        {
            audioSource.clip = openSound;
            audioSource.Play();
            gameObject.GetComponent<Canvas>().enabled = true;
            foreach (var inventorySlot in inventorySlots)
                inventorySlot.gameObject.SetActive(true);
            foreach (var mount in mountSlotsUI)
                mount.gameObject.SetActive(true);
            isOpen = true;
        }
        else
        {
            audioSource.clip = closeSound;
            audioSource.Play();
            gameObject.GetComponent<Canvas>().enabled = false;
            foreach (var inventorySlot in inventorySlots)
                inventorySlot.gameObject.SetActive(false);
            foreach (var mount in mountSlotsUI)
                mount.gameObject.SetActive(false);
            isOpen = false;
        }
    }

    /// <summary>
    /// Populates the tooltip with the information of the item in the inventory slot at the given index
    /// and displays the tooltip.
    /// </summary>
    /// <param name="index"></param>
    public virtual void ShowHoverTooltip(int index)
    {
        // If the provided index is greater than the number of inventory slots, then it is a mount slot.
        if (index >= inventorySlots.Count())
        {
            index -= inventorySlots.Count();
            if (mountSlotsUI[index].isEmpty)
            {
                infoScreen.Hide();
            }
            else if (!infoScreen.gameObject.activeSelf)
            {
                var item = mountSlotsUI[index].GetItem();

                infoScreen.SetInfo(item, 0);
                infoScreen.Show();
            }
        }
        else
        {
            if (inventorySlots[index].isEmpty)
            {
                infoScreen.Hide();
            }
            else if (!infoScreen.gameObject.activeSelf)
            {
                var item = inventorySlots[index].GetItem();

                infoScreen.SetInfo(item, inventorySlots[index].GetQuantity());
                infoScreen.Show();
            }
        }
    }

    /// <summary>
    /// Hide the slot tooltip.
    /// </summary>
    public void HideHoverTooltip()
    {
        infoScreen.Hide();
    }

    /// <summary>
    /// Deletes the item in a slot in the inventory, given the index of the slot.
    /// </summary>
    public void DeleteSlot(int index)
    {
        if (index < inventorySlots.Count())
            inventorySlots[index].DeleteSlot();
        else
        {
            var slot = mountSlotsUI[index - inventorySlots.Count()];
            var item = slot.GetItem();
            // If the item being deleted is a mounted storage component, we need to remove the slots it added to the player ship.
            if (item is StorageComponent)
            {
                RemoveSlots((slot.GetItem() as StorageComponent).slotCount);
            }
            slot.DeleteMountedItem();
        }

        // Play the delete sound.
        audioSource.Stop();
        audioSource.clip = clearSlotSound;
        audioSource.Play();
        infoScreen.Hide();
    }

    /// <summary>
    /// Clears a slot in the inventory, given the index of the slot.
    /// </summary>
    public void ClearSlot(int index)
    {
        if (index < inventorySlots.Count())
            inventorySlots[index].ClearSlot();
        else
            mountSlotsUI[index - inventorySlots.Count()].ClearSlot();

        // Play the delete sound.
        audioSource.Stop();
        audioSource.clip = clearSlotSound;
        audioSource.Play();
        infoScreen.Hide();
    }

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        if (item == null || !item.gameObject.activeSelf)
            return;

        Item temp = itemList.Find(x => (x.name.Equals(item.name))); // Find element in item list with name equivalent to the parameter.

        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.isEmpty && slot.GetItem().name.Equals(temp.name))
            {
                if (slot.GetItem().stackable && slot.GetQuantity() < slot.GetItem().stackSize)
                {
                    slot.SetQuantity(slot.GetQuantity() + 1);
                    return;
                }
            }
        }
        // If the item doesn't yet exist in the list, add it to an empty slot.
        foreach (InventorySlot slot in inventorySlots.OrderBy(s => s.transform.GetSiblingIndex()))
        {
            if (slot.isEmpty)
            {
                var result = Instantiate(temp, slot.GetInventoryItem().transform.position, Quaternion.identity, slot.GetInventoryItem().transform) as Item;
                slot.SetItem(result); // Add to the slot
                return;
            }
        }
    }

    /// <summary>
    /// Swap the items of the two slots.
    /// </summary>
    /// <param name="originalIndex"></param>
    /// <param name="newIndex"></param>
    public void SwapSlots(int[] indices)
    {
        var index1 = indices[0];
        var index2 = indices[1];
        audioSource.clip = swapSound;

        // If swapping between inventory slots.
        if (index1 < inventorySlots.Count() && index2 < inventorySlots.Count())
        {
            var slot1 = inventorySlots[index1];
            var slot2 = inventorySlots[index2];
            // Swap the items of the two, if they both contain an item.
            if (!slot1.isEmpty && !slot2.isEmpty)
            {
                slot1.GetItem().gameObject.transform.parent = slot2.GetInventoryItem().transform;
                slot2.GetItem().gameObject.transform.parent = slot1.GetInventoryItem().transform;
                var tempItem = slot2.GetItem();
                var tempQuantity = slot2.GetQuantity();

                slot2.SetItem(slot1.GetItem());
                slot2.SetQuantity(slot1.GetQuantity());
                slot1.SetItem(tempItem);
                slot1.SetQuantity(tempQuantity);

            }
            else if (slot2.isEmpty)
            {
                slot1.GetItem().gameObject.transform.parent = slot2.GetInventoryItem().transform;
                slot2.SetItem(slot1.GetItem());
                slot2.SetQuantity(slot1.GetQuantity());
                slot1.ClearSlot();
            }
            audioSource.Play();

        }
        // If swapping from a mounting slot.
        else if (index1 >= inventorySlots.Count() && index2 < inventorySlots.Count())
        {
            var mountSlot = mountSlotsUI[index1 - inventorySlots.Count()];
            var invSlot = inventorySlots[index2];

            // Swap the items of the two, if they both contain an item.
            if (!mountSlot.isEmpty && !invSlot.isEmpty)
            {
                ShipComponent invComp = invSlot.GetItem() as ShipComponent;
                ShipComponent mountComp = mountSlot.GetItem() as ShipComponent;

                if (invComp != null && mountComp != null)
                {
                    // Only swap if the components are the same type/tier/class.
                    if (invComp.IsSameComponentType(mountComp))
                    {
                        // Update size of inventory, if the component is a storage component.
                        if (invComp is StorageComponent)
                        {
                            RemoveSlots((mountComp as StorageComponent).slotCount);
                            AddSlots((invComp as StorageComponent).slotCount);
                        }
                        mountSlot.GetItem().gameObject.transform.parent = invSlot.GetInventoryItem().transform;
                        invSlot.GetItem().gameObject.transform.parent = mountSlot.GetInventoryItem().transform;
                        var temp = invSlot.GetItem();

                        invSlot.SetItem(mountSlot.GetItem());

                        mountSlot.SetItem(temp);
                        audioSource.Play();
                    }
                }
            }
            else if (invSlot.isEmpty)
            {
                mountSlot.GetItem().gameObject.transform.parent = invSlot.GetInventoryItem().transform;
                invSlot.SetItem(mountSlot.GetItem());
                if (mountSlot.GetItem() is StorageComponent)
                {
                    RemoveSlots((mountSlot.GetItem() as StorageComponent).slotCount);
                }
                mountSlot.ClearSlot();
                audioSource.Play();
            }
        }
        // If swapping to a mounting slot.
        else if (index1 < inventorySlots.Count() && index2 >= inventorySlots.Count())
        {
            var mountSlot = mountSlotsUI[index2 - inventorySlots.Count()];
            var invSlot = inventorySlots[index1];

            // Swap the items of the two, if they both contain an item.
            if (!mountSlot.isEmpty && !invSlot.isEmpty)
            {
                ShipComponent invComp = invSlot.GetItem() as ShipComponent;
                ShipComponent mountComp = mountSlot.GetItem() as ShipComponent;

                if (invComp != null && mountComp != null)
                {
                    // Only swap if the types of the mount and the inventory slot are the same.
                    if (invComp.IsSameComponentType(mountComp))
                    {
                        // Update size of inventory, if the component is a storage component.
                        if (invComp is StorageComponent)
                        {
                            // If the mount slot gave more inventory space than the one being swapped in, remove slots.
                            if((mountComp as StorageComponent).slotCount > (invComp as StorageComponent).slotCount)
                                RemoveSlots((mountComp as StorageComponent).slotCount - (invComp as StorageComponent).slotCount);
                            else
                                AddSlots((invComp as StorageComponent).slotCount - (mountComp as StorageComponent).slotCount);
                        }

                        mountSlot.GetItem().gameObject.transform.parent = invSlot.GetInventoryItem().transform;
                        invSlot.GetItem().gameObject.transform.parent = mountSlot.GetInventoryItem().transform;
                        var temp = invSlot.GetItem();

                        invSlot.SetItem(mountSlot.GetItem());
                        mountSlot.SetItem(temp);
                        audioSource.Play();
                    }
                }
            }
            else if (mountSlot.isEmpty)
            {
                if (mountSlot.IsComponentCompatible(invSlot.GetItem() as ShipComponent))
                {
                    invSlot.GetItem().gameObject.transform.parent = mountSlot.GetInventoryItem().transform;
                    mountSlot.SetItem(invSlot.GetItem());
                    if (invSlot.GetItem() is StorageComponent)
                    {
                        AddSlots((invSlot.GetItem() as StorageComponent).slotCount);
                    }
                    invSlot.ClearSlot();
                    audioSource.Play();
                }
            }
        }
        // If swapping between mounting slots.
        else
        {
            var slot1 = mountSlotsUI[index1 - inventorySlots.Count()];
            var slot2 = mountSlotsUI[index2 - inventorySlots.Count()];
            // Only swap their items if they are the same type of mount.
            if (slot1.IsSameSlotType(slot2))
            {
                // Swap the items of the two, if they both contain an item.
                if (!slot1.isEmpty && !slot2.isEmpty)
                {
                    slot1.GetItem().gameObject.transform.parent = slot2.GetInventoryItem().transform;
                    slot2.GetItem().gameObject.transform.parent = slot1.GetInventoryItem().transform;
                    var temp = slot2.GetItem();

                    slot2.SetItem(slot1.GetItem());
                    slot1.SetItem(temp);

                }
                else if (slot2.isEmpty)
                {
                    slot1.GetItem().gameObject.transform.parent = slot2.GetInventoryItem().transform;
                    slot2.SetItem(slot1.GetItem());
                    slot1.ClearSlot();
                }
                audioSource.Play();
            }
        }
    }



    /// <summary>
    /// Checks the list of Inventory Slots to see if an empty one exists.
    /// </summary>
    /// <returns></returns>
    public bool ContainsEmptySlot()
    {
        // Since we can swap inventory slots around, we need to search based on their ordering in the object hierarchy.
        foreach (InventorySlot slot in inventorySlots.OrderBy(s => s.GetIndex()))
        {
            if (slot.isEmpty) { return true; } // Empty slot found
        }

        return false;
    }

    /// <summary>
    /// Get an empty slot if it exists. Otherwise returns null.
    /// </summary>
    /// <returns></returns>
    public InventorySlot GetEmptySlot()
    {
        foreach (InventorySlot slot in inventorySlots.OrderBy(s => s.transform.GetSiblingIndex()))
        {
            if (slot.isEmpty) { return slot; } // Empty slot found
        }

        return null;
    }

    /// <summary>
    /// Checks to see if an inventory slot contains the given item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns> False if the inventory contains the item, but the item is at max stack. </returns>
    public bool ContainsItem(Item item)
    {
        // Find element in item list equivalent to the parameter.
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.GetItem() != null)
            {
                if (slot.GetItem().name.Equals(item.name) && slot.GetQuantity() < slot.GetItem().stackSize)
                    return true;
            }
        }
        return false; // Find element in item list equivalent to the parameter.
    }
}
