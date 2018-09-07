using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Tool Tip")]
    public GameObject tooltip;

    [Header("Sounds")]
    public AudioClip inventoryOpen;
    public AudioClip inventoryClose;
    public AudioClip swapSound;
    public AudioClip clearSlotSound;

    private InfoScreen infoScreen;
    private InventorySlot[] inventorySlots;
    private AudioSource audioSource;
    private bool isOpen = false;

    // All possible items in the game. This reference is required in order to maintain inventory sprites when objects are destroyed.
    private List<Item> itemList;


    // Use this for initialization
    void Start()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>().ToArray();

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
        infoScreen = tooltip.GetComponent<InfoScreen>();
    }

    /// <summary>
    /// Called to toggle display to inventory UI.
    /// </summary>
    public void Toggle()
    {
        if(!isOpen)
        {
            audioSource.clip = inventoryOpen;
            audioSource.Play();
            gameObject.GetComponent<Canvas>().enabled = true;
            isOpen = true;
        }
        else
        {
            audioSource.clip = inventoryClose;
            audioSource.Play();
            gameObject.GetComponent<Canvas>().enabled = false;
            isOpen = false;
        }
    }

    /// <summary>
    /// Populates the tooltip with the information of the item in the inventory slot at the given index
    /// and displays the tooltip.
    /// </summary>
    /// <param name="index"></param>
    public void ShowHoverTooltip(int index)
    {
        if (inventorySlots[index].isEmpty)
        {
            infoScreen.Hide();
        }
        else
        {
            var item = inventorySlots[index].GetItem();

            infoScreen.SetInfo(item, inventorySlots[index].GetQuantity());
            infoScreen.Show();
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
        inventorySlots[index].DeleteSlot();

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
        inventorySlots[index].ClearSlot();

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
                    slot.IncrementQuantity();
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

        // Swap the values of the two, if they both contain an item.
        if(!inventorySlots[index1].isEmpty && !inventorySlots[index2].isEmpty)
        {
            inventorySlots[index1].GetItem().gameObject.transform.parent = inventorySlots[index2].GetInventoryItem().transform;
            inventorySlots[index2].GetItem().gameObject.transform.parent = inventorySlots[index1].GetInventoryItem().transform;
            var temp = inventorySlots[index2].GetItem();

            inventorySlots[index2].SetItem(inventorySlots[index1].GetItem());
            inventorySlots[index1].SetItem(temp);

        }
        else if(inventorySlots[index2].isEmpty)
        {
            inventorySlots[index1].GetItem().gameObject.transform.parent = inventorySlots[index2].GetInventoryItem().transform;
            inventorySlots[index2].SetItem(inventorySlots[index1].GetItem());
            inventorySlots[index1].ClearSlot();
        }

        audioSource.clip = swapSound;
        audioSource.Play();
    }



    /// <summary>
    /// Checks the list of Inventory Slots to see if an empty one exists.
    /// </summary>
    /// <returns></returns>
    public bool ContainsEmptySlot()
    {
        // Since we can swap inventory slots around, we need to search based on their ordering in the object hierarchy.
        foreach (InventorySlot slot in inventorySlots.OrderBy(s => s.transform.GetSiblingIndex()))
        {
            if (slot.isEmpty) { return true; } // Empty slot found
        }

        return false;
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
                if (slot.GetItem().name.Contains(item.name) && slot.GetQuantity() < slot.GetItem().stackSize)
                    return true;
            }
        }
        return false; // Find element in item list equivalent to the parameter.
    }
}
