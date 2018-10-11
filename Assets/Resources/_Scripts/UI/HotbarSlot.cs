using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : InteractableElement
{
    [HideInInspector]
    public bool isEmpty = true; // All slots start out empty.

    // The num key associated with this hotbar slot.
    private int numkey;
    private TextMeshProUGUI numkeyDisplay;
    private InventoryItem inventoryItem; // The item in the slot.

    void Awake()
    {
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        numkeyDisplay = GetComponentInChildren<TextMeshProUGUI>();
        inventoryItem = GetComponentInChildren<InventoryItem>();
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
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
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        isEmpty = true;
    }

    /// <summary>
    /// Plays hover sound.
    /// </summary>
    void OnMouseEnter()
    {
        if (!isEmpty && !InventoryItem.dragging)
        {
            if (enterSound != null)
            {
                audioSource.clip = enterSound;
                audioSource.Play();
            }
        }
    }

    // Highlight the image when hovering over it
    void OnMouseOver()
    {
        if (!isEmpty && !InventoryItem.dragging)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            inventoryItem.Highlight();
            SendMessageUpwards("ShowHoverTooltip", index);
        }
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        if (!isEmpty)
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);

        if (inventoryItem.gameObject.activeSelf && !InventoryItem.dragging)
        {
            inventoryItem.Dehighlight();
            SendMessageUpwards("HideHoverTooltip");
            if (exitSound != null)
            {
                audioSource.clip = exitSound;
                audioSource.Play();
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
