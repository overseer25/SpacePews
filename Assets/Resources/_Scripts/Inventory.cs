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

    private InfoScreen infoScreen;
    private InventorySlot[] slots;
    private AudioSource audio;
    private bool isOpen = false;

    // All possible items in the game. This reference is required in order to maintain inventory sprites when objects are destroyed.
    private List<Item> itemList;


    // Use this for initialization
    void Start()
    {
        slots = GetComponentsInChildren<InventorySlot>();

        var i = 0;
        foreach (var slot in slots)
            slot.SetIndex(i++);

        itemList = new List<Item>();
        foreach (var obj in Resources.LoadAll("_Prefabs/Items"))
        {
            GameObject itemObj = obj as GameObject;
            itemList.Add(itemObj.GetComponent<Item>());
        }
        audio = GetComponent<AudioSource>();
        infoScreen = tooltip.GetComponent<InfoScreen>();
    }

    /// <summary>
    /// Called to toggle display to inventory UI.
    /// </summary>
    public void Toggle()
    {
        if(!isOpen)
        {
            audio.clip = inventoryOpen;
            audio.Play();
            gameObject.GetComponent<Canvas>().enabled = true;
            isOpen = true;
        }
        else
        {
            audio.clip = inventoryClose;
            audio.Play();
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
        if (slots[index].isEmpty)
        {
            infoScreen.Hide();
        }
        else
        {
            var item = slots[index].GetItem();

            infoScreen.SetInfo(item);
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
    /// TODO: Maybe switch to Dictionary?
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        Item temp = itemList.Find(x => (item.name.Contains(x.name))); // Find element in item list with name equivalent to the parameter.
        foreach (InventorySlot slot in slots)
        {
            if (!slot.isEmpty && temp.name.Contains(slot.GetItem().name))
            {
                slot.IncrementQuantity();
                return;
            }
        }
        // If the item doesn't yet exist in the list, add it to an empty slot.
        foreach (InventorySlot slot in slots.OrderBy(s => s.transform.GetSiblingIndex()))
        {
            if (slot.isEmpty)
            {
                slot.SetItem(temp); // Add to the slot
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

        // Index of the first slot.
        var ind1 = slots[index1].transform.GetSiblingIndex();

        // Swap the slots. Make sure to reorder their slots to remain consistent.
        slots[index1].transform.SetSiblingIndex(slots[index2].transform.GetSiblingIndex());
        slots[index2].transform.SetSiblingIndex(ind1);
    }

    /// <summary>
    /// Checks the list of Inventory Slots to see if an empty one exists.
    /// </summary>
    /// <returns></returns>
    public bool ContainsEmptySlot()
    {
        // Since we can swap inventory slots around, we need to search based on their ordering in the object hierarchy.
        foreach (InventorySlot slot in slots.OrderBy(s => s.transform.GetSiblingIndex()))
        {
            if (slot.isEmpty) { return true; } // Empty slot found
        }

        return false;
    }

    /// <summary>
    /// Checks to see if an inventory slot contains the given item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ContainsItem(Item item)
    {
        // Find element in item list equivalent to the parameter.
        foreach (InventorySlot slot in slots)
        {
            if (slot.GetItem() != null)
            {
                if (item.name.Contains(slot.GetItem().name))
                    return true;
            }
        }
        return false; // Find element in item list equivalent to the parameter.
    }
}
