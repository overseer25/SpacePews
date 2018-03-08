using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTooltip : MonoBehaviour {

    private InventorySlot inventorySlot;
    public DestroyButton destroyButton;
    public InfoButton infoButton;

    /// <summary>
    /// Sets the slot the tooltip is currently able to manipulate
    /// </summary>
    public void SetSlot(InventorySlot slot)
    {
        inventorySlot = slot;
        destroyButton.slot = inventorySlot;
        infoButton.slot = inventorySlot;
    }

    public InventorySlot GetSlot()
    {
        return inventorySlot;
    }
}
