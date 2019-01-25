using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : SlotBase
{

    internal bool canHighlight = false; // Allow the slot to highlight when hovered over.
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
    public ShipComponent startingComponent;

    void Awake()
    {
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        numkeyDisplay = GetComponentInChildren<TextMeshProUGUI>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
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
            inventoryItem.Dehighlight();
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
        if (!(component is WeaponComponent) && !(component is MiningComponent))
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

        if (canHighlight && !IsEmpty() && !InventoryItem.dragging)
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
            // Shift right-clicking will swap slots.
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
            {
                SendMessageUpwards("QuickSwapWithInventorySlot", index);
            }

            if (!IsEmpty() && !InventoryItem.dragging)
            {
                Highlight();
                SendMessageUpwards("ShowHoverTooltip", index);
            }
            else if (IsEmpty())
            {
                Dehighlight();
                SendMessageUpwards("HideHoverTooltip");
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
            if (inventoryItem.gameObject.activeSelf && !InventoryItem.dragging)
            {
                if (!current)
                {
                    inventoryItem.Dehighlight();
                    if (exitSound != null)
                        audioSource.PlayOneShot(exitSound);
                }
                SendMessageUpwards("HideHoverTooltip");
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
