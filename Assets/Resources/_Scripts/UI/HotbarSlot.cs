using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : InteractableElement
{
    [HideInInspector]
    public bool isEmpty = true; // All slots start out empty.
    internal bool canHighlight = false; // Allow the slot to highlight when hovered over.
    // The num key associated with this hotbar slot.
    private int numkey;
    private TextMeshProUGUI numkeyDisplay;
    private InventoryItem inventoryItem; // The item in the slot.

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
        if (inventoryItem.gameObject.activeSelf)
            inventoryItem.Highlight();
    }

    /// <summary>
    /// Removes the highlight to represent the slot is no longer selected.
    /// </summary>
    public void Deselect()
    {
        current = false;
        image.color = DEFAULTCOLOR;
        if (inventoryItem.gameObject.activeSelf)
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
    /// Sets the turret component
    /// </summary>
    /// <param name="component"></param>
    public void SetItem(Item component)
    {
        // If it is not a weapon or mining component, do not set the component.
        if (!(component is WeaponComponent) && !(component is MiningComponent))
            return;

        inventoryItem.SetItem(component, 0);
        isEmpty = false;
    }

    /// <summary>
    /// Get the ship component of the hotbar slot.
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return inventoryItem.item;
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
    /// Deletes the item gameobject on the slot.
    /// </summary>
    public void DeleteSlot()
    {
        inventoryItem.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        inventoryItem.SetQuantity(0);
        Destroy(inventoryItem.item.gameObject);
        inventoryItem.gameObject.SetActive(false);
        isEmpty = true;
    }

    /// <summary>
    /// "Empty" the slot.
    /// </summary>
    public void ClearSlot()
    {
        inventoryItem.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        inventoryItem.SetQuantity(0);
        inventoryItem.SetItem(null, 0);
        inventoryItem.gameObject.SetActive(false);
        if (current)
            image.color = SELECTCOLOR;
        else
            image.color = DEFAULTCOLOR;
        isEmpty = true;
    }

    /// <summary>
    /// Plays hover sound.
    /// </summary>
    void OnMouseEnter()
    {

        if (canHighlight && !isEmpty && !InventoryItem.dragging)
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

            if (!isEmpty && !InventoryItem.dragging)
            {
                if (!current)
                    image.color = DEFAULTCOLOR_HIGHLIGHT;
                else
                    image.color = SELECTCOLOR_HIGHLIGHT;
                inventoryItem.Highlight();
                SendMessageUpwards("ShowHoverTooltip", index);
            }
        }
        else if (current)
        {
            image.color = SELECTCOLOR;
        }
        else if (!current)
        {
            image.color = SELECTCOLOR;
            Deselect();
            inventoryItem.Dehighlight();
        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {

        if (current)
            image.color = SELECTCOLOR;
        else if (!isEmpty)
            image.color = DEFAULTCOLOR;

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
