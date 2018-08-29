using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public GameObject tooltip;

    private InventorySlot[] slots;
    // All possible items in the game. This reference is required in order to maintain inventory sprites when objects are destroyed.
    private List<Item> itemList; 


	// Use this for initialization
	void Start ()
    {
        slots = GetComponentsInChildren<InventorySlot>();

        var i = 0;
        foreach (var slot in slots)
            slot.SetIndex(i++);

        itemList = new List<Item>();
        foreach(var obj in Resources.LoadAll("_Prefabs/Items"))
        {
            GameObject itemObj = obj as GameObject;
            itemList.Add(itemObj.GetComponent<Item>());
        }
    }

    /// <summary>
    /// Populates the tooltip with the information of the item in the inventory slot at the given index
    /// and displays the tooltip.
    /// </summary>
    /// <param name="index"></param>
    public void HoverTooltip(int index)
    {
        if(slots[index].isEmpty)
        {
            tooltip.SetActive(false);
        }
        else
        {
            tooltip.SetActive(true);
            var item = slots[index].GetItem();
            tooltip.GetComponent<InfoScreen>().SetInfo(item.name, item.type, item.quantity.ToString(), item.value.ToString(), item.description);
        }
    }
	
    /// <summary>
    /// TODO: Maybe switch to Dictionary?
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        Item temp = itemList.Find(x => (item.name.Contains(x.name))); // Find element in item list with name equivalent to the parameter.

        foreach(InventorySlot slot in slots)
        {
            if(!slot.isEmpty && temp.name.Contains(slot.GetItem().name))
            {
                slot.quantity++;
                slot.UpdateSlot();
                return;
            }
        }
        // If the item doesn't yet exist in the list, add it to an empty slot.
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty)
            {
                slot.SetItem(temp); // Add to the slot
                slot.quantity++;
                slot.UpdateSlot();
                return;
            }
        }
    }

    /// <summary>
    /// Checks the list of Inventory Slots to see if an empty one exists.
    /// </summary>
    /// <returns></returns>
    public bool IsEmptySlot()
    {
        foreach(InventorySlot slot in slots)
        {
            if(slot.isEmpty) { return true; } // Empty slot found
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
        foreach(InventorySlot slot in slots)
        {
            if(slot.GetItem() != null)
            {
                if (item.name.Contains(slot.GetItem().name))
                {
                    return true;
                }
            }

        }
        return false; // Find element in item list equivalent to the parameter.
    }
}
