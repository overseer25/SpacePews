﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : SlotBase
{

    // The num key associated with this hotbar slot.
    private int numkey;
    private TextMeshProUGUI numkeyDisplay;

    // Colors
    private readonly Color SELECTCOLOR = new Color(0.2f, 0.7f, 1.0f);
    private readonly Color SELECTCOLOR_HIGHLIGHT = new Color(0.6f, 0.8f, 1.0f);
    private readonly Color DEFAULTCOLOR = new Color(1.0f, 1.0f, 1.0f, 0.4f);
    private readonly Color DEFAULTCOLOR_HIGHLIGHT = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    internal bool current; // Is this hotbar slot the selected one?
    [Header("Starting Component")]
    public ShipComponentBase startingComponent;

    protected override void Awake()
    {
        base.Awake();
        numkeyDisplay = GetComponentInChildren<TextMeshProUGUI>();
        image.color = DEFAULTCOLOR;
        if (startingComponent != null)
        {
            SetItem(startingComponent);
        }
    }

    /// <summary>
    /// Highlights the slot to represent that it is selected.
    /// </summary>
    public void Select()
    {
        current = true;
        image.color = SELECTCOLOR;
        if (inventoryItem.GetItem() != null)
            inventoryItem.Highlight();
    }

    /// <summary>
    /// Removes the highlight to represent the slot is no longer selected.
    /// </summary>
    public void Deselect()
    {
        current = false;
        image.color = DEFAULTCOLOR;
        if (inventoryItem.GetItem() != null)
            inventoryItem.Dehighlight();
    }

    /// <summary>
    /// Is the hotbar slot selected?
    /// </summary>
    public bool IsSelected()
    {
        return current;
    }

    /// <summary>
    /// Display the ship component. Called when the slot is selected.
    /// </summary>
    public void DisplayComponent()
    {
        inventoryItem.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the ship component. Called when the slot is deselected.
    /// </summary>
    public void HideComponent()
    {
        inventoryItem.gameObject.SetActive(false);
    }

    /// <summary>
    /// Highlight the slot.
    /// </summary>
    public override void Highlight()
    {
        if (image != null)
        {
            if(current)
                image.color = SELECTCOLOR_HIGHLIGHT;
            else
                image.color = DEFAULTCOLOR_HIGHLIGHT;

            if (inventoryItem.GetItem() != null)
                inventoryItem.Highlight();
        }
    }

    /// <summary>
    /// Dehighlight the slot.
    /// </summary>
    public override void Dehighlight()
    {
        if (image != null)
        {
            if (current)
                image.color = SELECTCOLOR;
            else
                image.color = DEFAULTCOLOR;

            if (inventoryItem.GetItem() != null)
                inventoryItem.Dehighlight();
        }
    }

    /// <summary>
    /// Sets the turret component
    /// </summary>
    /// <param name="component"></param>
    public override void SetItem(Item component)
    {
        // If it is not a weapon or mining component, do not set the component.
        if (!(component is WeaponComponentBase) && !(component is MiningComponent))
            return;

        inventoryItem.SetItem(component, 0);
    }

    /// <summary>
    /// "Empty" the slot.
    /// </summary>
    public override void ClearSlot()
    {
        inventoryItem.Clear();
        Dehighlight();
    }

    /// <summary>
    /// Plays hover sound.
    /// </summary>
    void OnMouseEnter()
    {

        if (canHighlight && !IsEmpty() && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
        {
            if (enterSound != null)
            {
                audioSource.PlayOneShot(enterSound);
            }
        }
    }

    // Highlight the image when hovering over it
    void OnMouseOver()
    {
        // If the slot can be highlighted.
        if (canHighlight)
        {
            // Right-clicking will swap slots.
            if (Input.GetMouseButtonDown(1))
            {
                SendMessageUpwards("QuickSwapWithInventorySlot", index);
            }

            if (!IsEmpty() && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
            {
                Highlight();
				InfoScreen.current.SetInfo(GetItem());
				InfoScreen.current.Show();
			}
            else if (IsEmpty())
            {
                Dehighlight();
				InfoScreen.current.Hide();
            }
        }
        else
            Dehighlight();
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        Dehighlight();

        if (canHighlight)
        {
            if (inventoryItem.gameObject.activeSelf && !InventoryItem.dragging && !InventoryItem.rightClickDragging)
            {
                if (!current)
                {
                    inventoryItem.Dehighlight();
                    if (exitSound != null)
                        audioSource.PlayOneShot(exitSound);
                }
				InfoScreen.current.Hide();
            }
        }
    }

    /// <summary>
    /// Initialize this hotbar slot with some default data.
    /// </summary>
    public void Initialize(int index, int numkey)
    {
        this.index = index;
        this.numkey = numkey;
        numkeyDisplay.text = numkey.ToString();
    }
}
