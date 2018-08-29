using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTooltip : MonoBehaviour {

    private InventorySlot inventorySlot;

    /// <summary>
    /// Sets the slot the tooltip is currently able to manipulate
    /// </summary>
    public void SetSlot(InventorySlot slot)
    {
        inventorySlot = slot;
    }

    public InventorySlot GetSlot()
    {
        return inventorySlot;
    }
}
