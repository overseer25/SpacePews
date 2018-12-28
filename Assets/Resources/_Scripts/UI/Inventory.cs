using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Tool Tip")]
    public InfoScreen infoScreen;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip openSound;
    [SerializeField]
    private AudioClip closeSound;
    [SerializeField]
    private AudioClip swapSound;
    [SerializeField]
    private AudioClip clearSlotSound;
    [SerializeField]
    private AudioClip hotbarSelectSound;

    internal AudioSource audioSource;
    internal bool isOpen = false;
    // Is the game paused?
    internal bool isPaused = false;

    [Header("Other")]
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private ShipMountController mountController;
    [SerializeField]
    private WeaponController weaponController;
    [SerializeField]
    private GameObject inventoryUI;
    [SerializeField]
    private GameObject mountUI;
    [SerializeField]
    private GameObject hotbarUI;
    [SerializeField]
    private TMPro.TextMeshProUGUI selectedTextDisplay; // Displays the name of the currently selected hotbar item.

    private List<InventorySlot> inventorySlots;
    private List<MountSlot> mountSlots;
    private List<HotbarSlot> hotbarSlots;

    // All possible items in the game. This reference is required in order to maintain inventory sprites when objects are destroyed.
    private List<Item> itemList;
    private int inventorySize = 0;
    private int selectedHotbarSlotIndex;
    private bool dead = false;


    // Use this for initialization
    void Awake()
    {

        inventorySlots = new List<InventorySlot>();

        var index = 0;

        itemList = new List<Item>();
        foreach (var obj in Resources.LoadAll("_Prefabs/Items"))
        {
            var item = (obj as GameObject);
            itemList.Add(item.GetComponent<Item>());
        }
        audioSource = GetComponent<AudioSource>();

        mountSlots = new List<MountSlot>();
        foreach (var mount in mountController.GetAllMounts())
        {
            var slot = Instantiate(Resources.Load("_Prefabs/UI/MountingSystem/MountSlot"), mount.transform.position, mount.transform.rotation, mountUI.transform) as GameObject;
            slot.GetComponent<MountSlot>().Initialize(mount, mount.GetMountType(), mount.GetMountTier(), mount.GetMountClass(), index++);
            mountSlots.Add(slot.GetComponent<MountSlot>());
        }

        hotbarSlots = new List<HotbarSlot>();
        for (int i = 1; i <= 5; i++)
        {
            var slot = Instantiate(Resources.Load("_Prefabs/UI/Hotbar/HotbarSlot"), hotbarUI.transform) as GameObject;
            slot.GetComponent<HotbarSlot>().Initialize(index++, i);
            hotbarSlots.Add(slot.GetComponent<HotbarSlot>());
        }
        hotbarSlots[0].Select();

        CloseInventory();
    }

    /// <summary>
    /// Mainly controls updating the hotbar.
    /// </summary>
    void Update()
    {
        if (!dead && !isPaused)
        {
            // If scrolling up the hotbar.
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                // Wrap to the beginning if at the end of the hotbar.
                if (selectedHotbarSlotIndex + 1 >= hotbarSlots.Count)
                    SwitchHotbarSlot(0);
                // Otherwise, just move to the next hotbar slot.
                else
                {
                    SwitchHotbarSlot(selectedHotbarSlotIndex + 1);
                }
            }
            // If scrolling down the hotbar.
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                // Wrap to the end if at the beginning of the hotbar.
                if (selectedHotbarSlotIndex - 1 < 0)
                    SwitchHotbarSlot(hotbarSlots.Count - 1);
                // Otherwise, just move to the previous hotbar slot.
                else
                {
                    SwitchHotbarSlot(selectedHotbarSlotIndex - 1);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SwitchHotbarSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SwitchHotbarSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                SwitchHotbarSlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                SwitchHotbarSlot(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                SwitchHotbarSlot(4);
        }
        UpdateSelectedItemDisplay(GetSelectedHotbarSlot().GetItem());
    }

    /// <summary>
    /// Changes the current selected hotbar slot to the provided index.
    /// </summary>
    /// <param name="index"></param>
    private void SwitchHotbarSlot(int index)
    {
        hotbarSlots[selectedHotbarSlotIndex].Deselect();
        selectedHotbarSlotIndex = index;
        hotbarSlots[selectedHotbarSlotIndex].Select();
        var item = hotbarSlots[selectedHotbarSlotIndex].GetItem();
        weaponController.UpdateTurret(item as ShipComponent);
        audioSource.PlayOneShot(hotbarSelectSound);
        UpdateSelectedItemDisplay(item);
    }

    /// <summary>
    /// Update the text display for the selected item's name.
    /// </summary>
    /// <param name="item"></param>
    private void UpdateSelectedItemDisplay(Item item)
    {
        if (item != null)
            selectedTextDisplay.text = hotbarSlots[selectedHotbarSlotIndex].GetItem().name;
        else
            selectedTextDisplay.text = "";
    }

    /// <summary>
    /// Get the currently selected hotbar slot.
    /// </summary>
    /// <returns></returns>
    public HotbarSlot GetSelectedHotbarSlot()
    {
        return hotbarSlots[selectedHotbarSlotIndex];
    }

    /// <summary>
    /// Update the status of the player's pause menu is open.
    /// </summary>
    /// <param name="paused"></param>
    public void UpdatePaused(bool paused)
    {
        isPaused = paused;
        if (isPaused)
        {
            HideHoverTooltip();
            HideHotbar();
            CloseInventory(false);
            selectedTextDisplay.enabled = false;
        }
        else if (!dead)
        {
            ShowHotbar();
            selectedTextDisplay.enabled = true;
        }
    }

    /// <summary>
    /// Update the status of the player's death.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        dead = isDead;
        if (dead)
        {
            CloseInventory(false);
            HideHoverTooltip();
            HideHotbar();
            selectedTextDisplay.enabled = false;
        }
        else
        {
            ShowHotbar();
            selectedTextDisplay.enabled = true;
        }
    }

    /// <summary>
    /// Adds slots to the inventory.
    /// </summary>
    /// <param name="size"></param>
    public void AddSlots(int size)
    {
        var inventorySlot = Resources.Load("_Prefabs/UI/Inventory/Inventory_Slot") as GameObject;
        for (int i = 0; i < size; i++)
        {
            var slot = Instantiate(inventorySlot, inventoryUI.transform);
            slot.GetComponent<InventorySlot>().SetIndex(i + inventorySize);
            inventorySlots.Add(slot.GetComponent<InventorySlot>());
        }
        int j = size + inventorySize;
        foreach (var mountSlot in mountSlots)
            mountSlot.SetIndex(j++);
        foreach (var hotbarSlot in hotbarSlots)
            hotbarSlot.SetIndex(j++);

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
        foreach (var mountSlot in mountSlots)
            mountSlot.SetIndex(j++);
        foreach (var hotbarSlot in hotbarSlots)
            hotbarSlot.SetIndex(j++);

        inventorySize -= size;
    }

    /// <summary>
    /// Called to toggle display to inventory UI.
    /// </summary>
    public virtual void Toggle()
    {
        if (!isPaused)
        {
            if (!isOpen)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }
    }

    /// <summary>
    /// Put the inventory to the open state.
    /// </summary>
    public virtual void OpenInventory(bool playSound = true)
    {
        if (playSound)
            audioSource.PlayOneShot(openSound);
        inventoryUI.GetComponentInParent<CanvasGroup>().alpha = 1.0f;
        foreach (var mount in mountSlots)
            mount.gameObject.SetActive(true);
        isOpen = true;
        selectedTextDisplay.enabled = false;
        InventoryItem.draggable = true;
        AllowHotbarHoverEffect();
    }

    /// <summary>
    /// Put the inventory to the closed state.
    /// </summary>
    public virtual void CloseInventory(bool playSound = true)
    {
        if (playSound)
            audioSource.PlayOneShot(closeSound);
        inventoryUI.GetComponentInParent<CanvasGroup>().alpha = 0.0f;
        foreach (var mount in mountSlots)
            mount.gameObject.SetActive(false);
        isOpen = false;
        selectedTextDisplay.enabled = true;
        InventoryItem.draggable = false;
        ForbidHotbarHoverEffect();
    }


    /// <summary>
    /// Populates the tooltip with the information of the item in the inventory slot at the given index
    /// and displays the tooltip.
    /// </summary>
    /// <param name="index"></param>
    public virtual void ShowHoverTooltip(int index)
    {
        if (isOpen)
        {
            // If the provided index is greater than the number of inventory slots, then it is a mount slot.
            if (index >= inventorySlots.Count() + mountSlots.Count())
            {
                index -= (inventorySlots.Count() + mountSlots.Count());
                if (hotbarSlots[index].isEmpty)
                {
                    infoScreen.Hide();
                }
                else if (!infoScreen.gameObject.activeSelf)
                {
                    var item = hotbarSlots[index].GetItem();

                    infoScreen.SetInfo(item, 0);
                    infoScreen.Show();
                }
            }
            else if (index >= inventorySlots.Count())
            {
                index -= inventorySlots.Count();
                if (mountSlots[index].isEmpty)
                {
                    infoScreen.Hide();
                }
                else if (!infoScreen.gameObject.activeSelf)
                {
                    var item = mountSlots[index].GetItem();

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
    }

    /// <summary>
    /// Hide the slot tooltip.
    /// </summary>
    public void HideHoverTooltip()
    {
        infoScreen.Hide();
    }

    /// <summary>
    /// Show the hotbar.
    /// </summary>
    public void ShowHotbar()
    {
        foreach (var slot in hotbarSlots)
        {
            slot.gameObject.SetActive(true);
        }
        selectedTextDisplay.enabled = true;
    }

    /// <summary>
    /// Hide the hotbar.
    /// </summary>
    public void HideHotbar()
    {
        foreach (var slot in hotbarSlots)
        {
            slot.gameObject.SetActive(false);
        }
        selectedTextDisplay.enabled = false;
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
            var slot = mountSlots[index - inventorySlots.Count()];
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
            mountSlots[index - inventorySlots.Count()].ClearSlot();

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
            Item result;
            if (slot.isEmpty)
            {
                result = Instantiate(item, slot.GetInventoryItem().transform.position, Quaternion.identity, slot.GetInventoryItem().transform) as Item;
                slot.SetItem(result);
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

        // If swapping between inventory slots.
        if (index1 < inventorySlots.Count() && index2 < inventorySlots.Count())
        {
            var slot1 = inventorySlots[index1];
            var slot2 = inventorySlots[index2];
            SwapInventorySlots(slot1, slot2);
        }
        // If swapping from inventory to hotbar.
        else if (index1 < inventorySlots.Count() && index2 >= inventorySlots.Count() + mountSlots.Count())
        {
            var hotbarSlot = hotbarSlots[index2 - inventorySlots.Count() - mountSlots.Count()];
            var invSlot = inventorySlots[index1];
            SwapInventoryToHotbar(invSlot, hotbarSlot);
        }
        // If swapping from hotbar to inventory.
        else if (index1 >= inventorySlots.Count() + mountSlots.Count() && index2 < inventorySlots.Count())
        {
            var hotbarSlot = hotbarSlots[index1 - inventorySlots.Count() - mountSlots.Count()];
            var invSlot = inventorySlots[index2];
            SwapInventoryToHotbar(invSlot, hotbarSlot);
        }
        // If swapping from hotbar to hotbar.
        else if (index1 >= inventorySlots.Count() + mountSlots.Count() && index2 >= inventorySlots.Count() + mountSlots.Count())
        {
            var hotbarSlot = hotbarSlots[index1 - inventorySlots.Count() - mountSlots.Count()];
            var hotbarSlot2 = hotbarSlots[index2 - inventorySlots.Count() - mountSlots.Count()];
            SwapHotbarSlots(hotbarSlot, hotbarSlot2);
        }
        // If swapping from a mounting slot to inventory slot.
        else if (index1 >= inventorySlots.Count() && index2 < inventorySlots.Count())
        {
            var mountSlot = mountSlots[index1 - inventorySlots.Count()];
            var invSlot = inventorySlots[index2];
            SwapInventoryToMountSlot(invSlot, mountSlot);
        }
        // If swapping from an inventory slot to a mounting slot.
        else if (index1 < inventorySlots.Count() && index2 >= inventorySlots.Count())
        {
            var mountSlot = mountSlots[index2 - inventorySlots.Count()];
            var invSlot = inventorySlots[index1];
            SwapInventoryToMountSlot(invSlot, mountSlot);
        }
        else
            return;
        audioSource.PlayOneShot(swapSound);
        HideHoverTooltip();
    }

    /// <summary>
    /// Helper method used to swap the contents of two inventory slots.
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    private void SwapInventorySlots(InventorySlot slot1, InventorySlot slot2)
    {
        // Slot1 should never be empty.
        if (slot1.isEmpty)
            return;

        // If they both contain an item.
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
        // If slot2 is empty.
        else if (slot2.isEmpty)
        {
            slot1.GetItem().gameObject.transform.parent = slot2.GetInventoryItem().transform;
            slot2.SetItem(slot1.GetItem());
            slot2.SetQuantity(slot1.GetQuantity());
            slot1.ClearSlot();
        }
    }

    /// <summary>
    /// Helper method used to swap the contents of an inventory slot to a hotbar slot, or vice versa.
    /// </summary>
    /// <param name="invSlot"></param>
    /// <param name="hotbarSlot"></param>
    private void SwapInventoryToHotbar(InventorySlot invSlot, HotbarSlot hotbarSlot)
    {
        // If both slots contain an item.
        if (!hotbarSlot.isEmpty && !invSlot.isEmpty)
        {
            ShipComponent invComp = invSlot.GetItem() as ShipComponent;
            ShipComponent hotbarComp = hotbarSlot.GetItem() as ShipComponent;

            // Is each item a component?
            if (invComp != null && hotbarComp != null)
            {
                if (invComp.GetItemType() == ItemType.Turret)
                {
                    hotbarSlot.GetItem().gameObject.transform.parent = invSlot.GetInventoryItem().transform;
                    invSlot.GetItem().gameObject.transform.parent = hotbarSlot.GetInventoryItem().transform;
                    var temp = invSlot.GetItem();
                    invSlot.SetItem(hotbarSlot.GetItem());
                    hotbarSlot.SetItem(temp);
                }
            }
        }
        // If inventory slot is empty.
        else if (invSlot.isEmpty)
        {
            hotbarSlot.GetItem().gameObject.transform.parent = invSlot.GetInventoryItem().transform;
            invSlot.SetItem(hotbarSlot.GetItem());
            hotbarSlot.ClearSlot();
        }
        // If hotbar slot is empty.
        else if (hotbarSlot.isEmpty)
        {
            if (invSlot.GetItem() is WeaponComponent || invSlot.GetItem() is MiningComponent)
            {
                invSlot.GetItem().gameObject.transform.parent = hotbarSlot.GetInventoryItem().transform;
                hotbarSlot.SetItem(invSlot.GetItem());
                invSlot.ClearSlot();
            }
        }
        if (hotbarSlot.IsSelected())
            weaponController.UpdateTurret(hotbarSlot.GetItem() as ShipComponent);
    }

    /// <summary>
    /// Helper method used to swap the contents fo two hotbar slots.
    /// </summary>
    /// <param name="slot1"></param>
    /// <param name="slot2"></param>
    private void SwapHotbarSlots(HotbarSlot hotbarSlot, HotbarSlot hotbarSlot2)
    {
        // If both slots contain an item.
        if (!hotbarSlot.isEmpty && !hotbarSlot2.isEmpty)
        {
            ShipComponent invComp = hotbarSlot2.GetItem() as ShipComponent;
            ShipComponent hotbarComp = hotbarSlot.GetItem() as ShipComponent;

            hotbarSlot.GetItem().gameObject.transform.parent = hotbarSlot2.GetInventoryItem().transform;
            hotbarSlot2.GetItem().gameObject.transform.parent = hotbarSlot.GetInventoryItem().transform;
            var temp = hotbarSlot2.GetItem();
            hotbarSlot2.SetItem(hotbarSlot.GetItem());
            hotbarSlot.SetItem(temp);
        }
        else if (hotbarSlot2.isEmpty)
        {
            hotbarSlot.GetItem().gameObject.transform.parent = hotbarSlot2.GetInventoryItem().transform;
            hotbarSlot2.SetItem(hotbarSlot.GetItem());
            hotbarSlot.ClearSlot();
        }
        if (hotbarSlot.IsSelected())
            weaponController.UpdateTurret(hotbarSlot.GetItem() as ShipComponent);
        else if (hotbarSlot2.IsSelected())
            weaponController.UpdateTurret(hotbarSlot2.GetItem() as ShipComponent);
    }

    /// <summary>
    /// Helper method used to swap the contents of an inventory slot to a mount slot, or vice versa.
    /// </summary>
    /// <param name="invSlot"></param>
    /// <param name="mountSlot"></param>
    private void SwapInventoryToMountSlot(InventorySlot invSlot, MountSlot mountSlot)
    {
        // If they both contain an item.
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
                }
            }
        }
        // If inventory slot is empty.
        else if (invSlot.isEmpty)
        {
            mountSlot.GetItem().gameObject.transform.parent = invSlot.GetInventoryItem().transform;
            invSlot.SetItem(mountSlot.GetItem());
            if (mountSlot.GetItem() is StorageComponent)
            {
                RemoveSlots((mountSlot.GetItem() as StorageComponent).slotCount);
            }
            else if (mountSlot.GetItem() is ThrusterComponent)
                playerController.UpdateThrusterList();
            mountSlot.ClearSlot();
            
        }
        // If mount slot is empty.
        else if (mountSlot.isEmpty)
        {
            if (mountSlot.GetMount().IsComponentCompatible(invSlot.GetItem() as ShipComponent))
            {
                invSlot.GetItem().gameObject.transform.parent = mountSlot.GetInventoryItem().transform;
                mountSlot.SetItem(invSlot.GetItem());
                if (invSlot.GetItem() is StorageComponent)
                {
                    AddSlots((invSlot.GetItem() as StorageComponent).slotCount);
                }
                else if (invSlot.GetItem() is ThrusterComponent)
                    playerController.UpdateThrusterList();
                invSlot.ClearSlot();
            }
        }
    }

    /// <summary>
    /// Swap provided index with an empty hotbar slot, or the currently selected one if
    /// no hotbar slots are empty.
    /// </summary>
    public void QuickSwapWithHotbarSlot(int index)
    {
        var slot = GetEmptyHotbarSlot();
        if (slot == null)
        {
            slot = GetSelectedHotbarSlot();
        }

        SwapSlots(new int[] { index, slot.GetIndex() });
    }

    /// <summary>
    /// Swap provided index with an empty mount slot of it's type, or the first non-empty one if none exists.
    /// </summary>
    /// <param name="index"></param>
    public void QuickSwapWithMountSlot(int index)
    {
        var item = inventorySlots[index].GetItem();
        var slotIndex = 0;
        slotIndex = GetIndexOfEmptyOrFirstMountSlotOfType(item.type);

        if (slotIndex >= 0)
            SwapSlots(new int[] { index, slotIndex });
    }

    /// <summary>
    /// Swap provided index with an empty inventory slot.
    /// </summary>
    /// <param name="index"></param>
    public void QuickSwapWithInventorySlot(int index)
    {
        var slot = GetEmptySlot();
        if (slot != null)
        {
            SwapSlots(new int[] { index, slot.GetIndex() });
        }
    }

    /// <summary>
    /// Get the index of an empty mount slot of the provided type, or the first non-empty one.
    /// </summary>
    /// <param name="type"></param>
    public int GetIndexOfEmptyOrFirstMountSlotOfType(ItemType type)
    {
        foreach (var mountSlot in mountSlots.Where(e => e.GetMount().GetMountType() == type))
        {
            if (mountSlot.GetItem() == null)
                return mountSlot.GetIndex();
        }
        var first = mountSlots.Where(e => e.GetMount().GetMountType() == type).OrderBy(e => e.GetIndex()).FirstOrDefault();
        if (first != null)
            return first.GetIndex();
        return -1;
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
    /// Get an empty hotbar slot if it exists. Otherwise returns null.
    /// </summary>
    /// <returns></returns>
    public HotbarSlot GetEmptyHotbarSlot()
    {
        foreach (HotbarSlot slot in hotbarSlots.OrderBy(e => e.index))
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

    /// <summary>
    /// Allow hotbar slots to highlight when the user hovers their mouse over them.
    /// </summary>
    private void AllowHotbarHoverEffect()
    {
        foreach (var slot in hotbarSlots)
        {
            slot.canHighlight = true;
        }
    }

    /// <summary>
    /// Do not allow hotbar slots to highlight when the user hovers their mouse over them.
    /// </summary>
    private void ForbidHotbarHoverEffect()
    {
        foreach (var slot in hotbarSlots)
        {
            slot.canHighlight = false;
        }
    }
}
