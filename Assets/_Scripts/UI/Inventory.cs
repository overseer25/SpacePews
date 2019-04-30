using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inventory management system for the player. Manages inventory slots, hotbar slots, and mount slots.
/// </summary>
public class Inventory : MonoBehaviour
{
	[Header("Item Transfer Confirm Window")]
	public GameObject itemTransferPanel;
	public Button itemTransferConfirmButton;
	public InputField itemTransferNumber;
	private int[] indicesToTransfer;

    [SerializeField]
    private GameObject hotbarSlot;
    [SerializeField]
    private GameObject mountSlot;

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
	[SerializeField]
	private AudioClip collectItemSound;
    [SerializeField]
    private AudioClip collectMoneySound;

	private const int INVENTORY_SIZE = 20;
    private const long MAX_CURRENCY = 999999999999;
    // Must be greater than 0 and less than 10.
    private const int HOTBAR_COUNT = 5;

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
    private TextMeshProUGUI currencyCount;
	[SerializeField]
	private TMPro.TextMeshProUGUI selectedTextDisplay; // Displays the name of the currently selected hotbar item.

	private List<InventorySlot> inventorySlots;
	private List<MountSlot> mountSlots;
	private List<HotbarSlot> hotbarSlots;

	// All possible items in the game. This reference is required in order to maintain inventory sprites when objects are destroyed.
	private List<Item> itemList;
	private int selectedHotbarSlotIndex;
	private bool dead = false;
    private long currency;

	// Use this for initialization
	void Awake()
	{
        currencyCount.text = currency.ToString();
		itemList = new List<Item>();
		var index = 0;
		foreach (var obj in Resources.LoadAll("Items"))
		{
			var item = (obj as GameObject);
            if(item.GetComponent<Item>() != null)
			    itemList.Add(item.GetComponent<Item>());
		}
		audioSource = GetComponent<AudioSource>();

		inventorySlots = GetComponentsInChildren<InventorySlot>().ToList();
		foreach (var slot in inventorySlots)
			slot.SetIndex(index++);

		mountSlots = new List<MountSlot>();
		foreach (var mount in mountController.GetAllMounts())
		{
			var slot = Instantiate(mountSlot, mount.transform.position, mount.transform.rotation, mountUI.transform) as GameObject;
			slot.GetComponent<MountSlot>().Initialize(mount, mount.GetMountType(), mount.GetMountTier(), mount.GetMountClass(), index++);
			mountSlots.Add(slot.GetComponent<MountSlot>());
		}

		hotbarSlots = new List<HotbarSlot>();
		for (int i = 1; i <= HOTBAR_COUNT; i++)
		{
			var slot = Instantiate(hotbarSlot, hotbarUI.transform) as GameObject;
			slot.GetComponent<HotbarSlot>().Initialize(index++, i);
			hotbarSlots.Add(slot.GetComponent<HotbarSlot>());
		}
		hotbarSlots[0].Select();

		CloseInventory(false);
	}

	/// <summary>
	/// Mainly controls updating the hotbar.
	/// </summary>
	void LateUpdate()
	{
		// Flash the selected inventory item when the transfer panel is open.
		if (itemTransferPanel.activeInHierarchy)
		{
			StartCoroutine(inventorySlots[indicesToTransfer[0]].Flash());
			StartCoroutine(inventorySlots[indicesToTransfer[1]].Flash());
		}

		if (!dead && !isPaused && !itemTransferPanel.activeInHierarchy)
		{
			// If scrolling up the hotbar.
			if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				// Wrap to the beginning if at the end of the hotbar.
				if (selectedHotbarSlotIndex + 1 >= HOTBAR_COUNT)
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
			for (int i = 1; i <= HOTBAR_COUNT; i++)
			{
				if (Input.GetKeyDown(i.ToString()))
					SwitchHotbarSlot(i - 1);
			}
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
		weaponController.UpdateTurret(item as ShipComponentBase);
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
			selectedTextDisplay.text = hotbarSlots[selectedHotbarSlotIndex].GetItem().GetItemName();
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
		ToggleSlotHoverEffect(true);
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
		ToggleSlotHoverEffect(false);
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
	/// Clears a slot in the inventory, given the index of the slot.
	/// </summary>
	public void ClearSlot(int index)
	{
		if (index < inventorySlots.Count())
			inventorySlots[index].ClearSlot();

		// Play the delete sound.
		audioSource.Stop();
		audioSource.clip = clearSlotSound;
		audioSource.Play();
		InfoScreen.current.Hide();
	}

	/// <summary>
	/// Adds an item to the inventory.
	/// </summary>
	/// <param name="item"></param>
	public void AddItem(Item item)
	{
		if (item == null || !item.gameObject.activeInHierarchy)
			return;

		Item prefab = itemList.Find(x => (x.GetItemName() == item.GetItemName())); // Find element in item list with name equivalent to the parameter.

		// If the item exists in the inventory and is stackable, update the quantity.
		foreach (InventorySlot slot in inventorySlots)
		{
			if (!slot.IsEmpty() && slot.GetItem().GetItemName().Equals(item.GetItemName()))
			{
				if (slot.GetItem().IsStackable() && slot.GetQuantity() < slot.GetItem().GetStackSize())
				{
					slot.GetInventoryItem().AddQuantity(item.GetQuantity());
					OutputWindow.current.DisplayText("Collected <color=" + item.itemColor.ToHex() + ">" + item.itemName + "</color>");
					audioSource.PlayOneShot(collectItemSound);
					return;
				}
			}
		}
		// If the item doesn't yet exist in the list, add it to an empty slot.
		foreach (InventorySlot slot in inventorySlots.OrderBy(s => s.transform.GetSiblingIndex()))
		{
			if (slot.IsEmpty())
			{
				slot.SetItem(prefab, item.GetQuantity());
				OutputWindow.current.DisplayText("Collected <color=" + item.itemColor.ToHex() + ">" + item.itemName + "</color>");
				audioSource.PlayOneShot(collectItemSound);
				return;
			}
		}
	}

    /// <summary>
    /// Add currency to the player.
    /// </summary>
    /// <param name="amount">The amount to add.</param>
    /// <returns>False if adding currency exceeds the max currency amount, true otherwise.</returns>
    public bool AddCurrency(long amount)
    {
        if (currency + amount > MAX_CURRENCY)
            return false;

        currency += amount;
        currencyCount.text = currency.ToString();
        OutputWindow.current.DisplayText("Collected <color=\"white\">" + amount + " Credits</color>");
        audioSource.PlayOneShot(collectMoneySound);
        return true;
    }

    /// <summary>
    /// remove currency from player.
    /// </summary>
    /// <param name="amount">The amount to remove.</param>
    /// <returns>False if removing currency caused the player's currency to dip below 0, true otherwise.</returns>
    public bool RemoveCurrency(long amount)
    {
        if (currency - amount < 0)
            return false;

        currency -= amount;
        currencyCount.text = currency.ToString();
        return true;
    }

    /// <summary>
    /// Check if adding the given amount to the currency of the player
    /// would cause a boundary overflow. Use negative values for subtraction.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool CheckCurrency(long amount)
    {
        long result = currency + amount;
        return (result > MAX_CURRENCY || result < 0);
    }

    /// <summary>
    /// Get the amount currency the player possesses.
    /// </summary>
    /// <returns>currency of player.</returns>
    public long GetCurrency()
    {
        return currency;
    }

    /// <summary>
    /// The right-click functionality of dragging an item from one slot to another. If the destination slot
    /// is empty, or contains the same item as the origin slot, then the user should be prompted to specify
    /// how much of the incoming item's stack they wish to transfer.
    /// </summary>
    /// <param name="indices"></param>
    public void PromptUserForAmountToSwap(int[] indices)
	{
		var index1 = indices[0];
		var index2 = indices[1];
		if (index1 < inventorySlots.Count() && index2 < inventorySlots.Count())
		{
			var slot1 = inventorySlots[index1];
			var slot2 = inventorySlots[index2];
			if (!slot2.IsEmpty())
			{
				// If the slots do not contain the same item, just swap them as normal.
				if (!slot1.ContainsSameItem(slot2))
					SwapSlots(indices);
				else
				{
					indicesToTransfer = indices;
					ShowPromptWindow();
				}
			}
			else
			{
				indicesToTransfer = indices;
				ShowPromptWindow();
			}
		}
	}

	/// <summary>
	/// The event that occurs when the user changes the value inside the item transfer
	/// prompt window.
	/// </summary>
	/// <returns></returns>
	public void OnValueChangeItemTransferInput()
	{
		int input = 0;
		InventorySlot slot = inventorySlots[indicesToTransfer[0]];
		if (!Int32.TryParse(itemTransferNumber.text, out input))
			itemTransferNumber.text = "";
		else if (input > slot.GetInventoryItem().GetQuantity())
			itemTransferNumber.text = slot.GetInventoryItem().GetQuantity().ToString();
	}

	/// <summary>
	/// Called when the user is finished editting the input form on the item transfer window.
	/// </summary>
	public void OnEndEditItemTransferInput()
	{
		if (Input.GetKeyDown(InputManager.current.controls.submit))
		{
			TransferItemButtonClick();
		}
	}

	private void ShowPromptWindow()
	{
		InventoryItem.draggable = false;
		itemTransferPanel.SetActive(true);
		ToggleSlotHoverEffect(false);
		itemTransferNumber.Select();
	}

	/// <summary>
	/// Hides the window for selecting amount of item to transfer, and resets the input box.
	/// </summary>
	private void HidePromptWindow()
	{
		InventoryItem.draggable = true;
		itemTransferPanel.SetActive(false);
		itemTransferNumber.text = "0";
		ToggleSlotHoverEffect(true);
		StopCoroutine(inventorySlots[indicesToTransfer[0]].Flash());
		StopCoroutine(inventorySlots[indicesToTransfer[1]].Flash());
		inventorySlots[indicesToTransfer[0]].Dehighlight();
	}

	/// <summary>
	/// Cancel button click event for the transfer item window.
	/// </summary>
	public void TransferItemCancelClick()
	{
		audioSource.PlayOneShot(clearSlotSound);
		HidePromptWindow();
	}

	/// <summary>
	/// The action to be done when this button is clicked.
	/// </summary>
	public void TransferItemButtonClick()
	{
		var index1 = indicesToTransfer[0];
		var index2 = indicesToTransfer[1];
		if (index1 < inventorySlots.Count() && index2 < inventorySlots.Count())
		{
			var slot1 = inventorySlots[index1];
			var slot2 = inventorySlots[index2];

			// Slot one should not be empty.
			if (slot1.IsEmpty())
			{
				return;
			}
			// If slot two is empty
			else if (slot2.IsEmpty())
			{
				var slot1Item = itemList.Find(x => (x.GetItemName().Equals(slot1.GetItem().GetItemName())));
				var slot1Quantity = slot1.GetQuantity();
				int numberToSwap;
				bool canParseInput = Int32.TryParse(itemTransferNumber.text, out numberToSwap);
				bool numberToSwapIsInvalid = numberToSwap <= 0 || numberToSwap > slot1Quantity;

				if (!canParseInput || numberToSwapIsInvalid)
				{
					HidePromptWindow();
					return;
				}

				slot2.SetItem(slot1Item, numberToSwap);
				if (numberToSwap == slot1Quantity)
					slot1.ClearSlot();
				else
					slot1.SetQuantity(slot1Quantity - numberToSwap);
			}
			// If both are non-empty, ensure they have the same object, and attempt to merge them.
			else
			{
				var slot1Item = itemList.Find(x => (x.GetItemName().Equals(slot1.GetItem().GetItemName())));
				var slot1Quantity = slot1.GetQuantity();

				var slot2Item = itemList.Find(x => (x.GetItemName().Equals(slot2.GetItem().GetItemName())));
				var slot2Quantity = slot2.GetQuantity();

				// Are they the same item, and if so, are they stackable?
				if (slot1.ContainsSameItem(slot2) && slot1Item.IsStackable())
				{
					int numberToSwap;
					// Edge case checks
					bool canParseInput = Int32.TryParse(itemTransferNumber.text, out numberToSwap);
					bool numberToSwapIsInvalid = numberToSwap <= 0 || numberToSwap > slot1Quantity;
					bool slotContainsMaxStack = slot2Quantity == slot2Item.stackSize;

					if (!canParseInput || numberToSwapIsInvalid || slotContainsMaxStack)
					{
						HidePromptWindow();
						return;
					}

					if (numberToSwap + slot2Quantity > slot1Item.stackSize)
					{
						var difference = numberToSwap + slot2Quantity - slot1Item.stackSize;
						slot2.SetQuantity(slot1Item.stackSize);
						slot1.SetQuantity(difference);
					}
					else if (numberToSwap != slot1Quantity)
					{
						slot2.SetQuantity(numberToSwap + slot2Quantity);
						slot1.SetQuantity(slot1Quantity - numberToSwap);
					}
					else
					{
						slot2.SetQuantity(numberToSwap + slot2Quantity);
						slot1.ClearSlot();
					}
				}
			}
		}
		HidePromptWindow();
		audioSource.PlayOneShot(swapSound);
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
		// If swapping from mount slot to mountslot.
		else if (index1 >= inventorySlots.Count() && index2 >= inventorySlots.Count()
			&& index1 < inventorySlots.Count() + mountSlots.Count() && index2 < inventorySlots.Count() + mountSlots.Count())
		{
			var mountSlot1 = mountSlots[index1 - inventorySlots.Count()];
			var mountSlot2 = mountSlots[index2 - inventorySlots.Count()];
			SwapMountSlots(mountSlot1, mountSlot2);
		}
		else
			return;
	}

	/// <summary>
	/// Helper method used to swap the contents of two inventory slots.
	/// </summary>
	/// <param name="e1"></param>
	/// <param name="e2"></param>
	private void SwapInventorySlots(InventorySlot slot1, InventorySlot slot2)
	{
		// Slot1 should never be empty.
		if (slot1.IsEmpty())
			return;

		// If they both contain an item.
		if (!slot1.IsEmpty() && !slot2.IsEmpty())
		{
			var slot1Item = itemList.Find(x => (x.GetItemName().Equals(slot1.GetItem().GetItemName())));
			var slot1Quantity = slot1.GetQuantity();

			var slot2Item = itemList.Find(x => (x.GetItemName().Equals(slot2.GetItem().GetItemName())));
			var slot2Quantity = slot2.GetQuantity();

			// If the items are the same, and they are stackable, then merge them into slot2, and return the remainder (if any) to slot1.
			if (slot1Item == slot2Item && slot1Item.IsStackable() && (slot1Quantity != slot1Item.stackSize && slot2Quantity != slot2Item.stackSize))
			{
				int remainder = (slot1.GetQuantity() + slot2.GetQuantity() > slot1Item.stackSize) ? slot1.GetQuantity() + slot2.GetQuantity() - slot1Item.stackSize : 0;
				if (remainder > 0)
				{
					slot2.SetQuantity(slot1Item.stackSize);
					slot1.SetQuantity(remainder);
				}
				else
				{
					slot2.SetQuantity(slot1Quantity + slot2Quantity);
					slot1.ClearSlot();
				}
			}
			else
			{
				slot2.SetItem(slot1Item, slot1Quantity);
				slot1.SetItem(slot2Item, slot2Quantity);
			}
		}
		// If slot2 is empty.
		else if (slot2.IsEmpty() && !slot1.IsEmpty())
		{
			var slot1Item = itemList.Find(x => (x.GetItemName().Equals(slot1.GetItem().GetItemName())));
			slot2.SetItem(slot1Item, slot1.GetQuantity());
			slot1.ClearSlot();
		}
		else
			return;

		audioSource.PlayOneShot(swapSound);
	}

	/// <summary>
	/// Helper method used to swap the contents of an inventory slot to a hotbar slot, or vice versa.
	/// </summary>
	/// <param name="invSlot"></param>
	/// <param name="hotbarSlot"></param>
	private void SwapInventoryToHotbar(InventorySlot invSlot, HotbarSlot hotbarSlot)
	{
		// If both slots contain an item.
		if (!hotbarSlot.IsEmpty() && !invSlot.IsEmpty())
		{
			var invComp = itemList.Find(x => (x.GetItemName().Equals(invSlot.GetItem().GetItemName())));
			var hotbarComp = itemList.Find(x => (x.GetItemName().Equals(hotbarSlot.GetItem().GetItemName())));

			// Is each item a component?
			if (invComp != null && hotbarComp != null && invComp.GetItemType() == ItemType.Turret)
			{
				invSlot.SetItem(hotbarComp);
				hotbarSlot.SetItem(invComp);
				weaponController.UpdateTurret(hotbarSlot.GetItem() as ShipComponentBase);
			}
			else
			{
				OutputWindow.current.DisplayText(invSlot.GetItem().GetPrettyName() + "<color=\"white\"> is not compatible with the hotbar</color>");
				return;
			}
		}
		// If inventory slot is empty.
		else if (invSlot.IsEmpty() && !hotbarSlot.IsEmpty())
		{
			var hotbarComp = itemList.Find(x => (x.GetItemName().Equals(hotbarSlot.GetItem().GetItemName())));
			invSlot.SetItem(hotbarComp);
			hotbarSlot.ClearSlot();
			weaponController.UpdateTurret(null);
		}
		// If hotbar slot is empty.
		else if (hotbarSlot.IsEmpty() && !invSlot.IsEmpty() && invSlot.GetItem().GetItemType() == ItemType.Turret)
		{
			var invComp = itemList.Find(x => (x.GetItemName().Equals(invSlot.GetItem().GetItemName())));
			hotbarSlot.SetItem(invComp);
			invSlot.ClearSlot();
			weaponController.UpdateTurret(hotbarSlot.GetItem() as ShipComponentBase);
		}
		else
		{
			OutputWindow.current.DisplayText(invSlot.GetItem().GetPrettyName() + "<color=\"white\"> is not compatible with the hotbar</color>");
			return;
		}


		audioSource.PlayOneShot(swapSound);
	}

	/// <summary>
	/// Helper method used to swap the contents fo two hotbar slots.
	/// </summary>
	/// <param name="slot1"></param>
	/// <param name="slot2"></param>
	private void SwapHotbarSlots(HotbarSlot hotbarSlot, HotbarSlot hotbarSlot2)
	{
		// If both slots contain an item.
		if (!hotbarSlot.IsEmpty() && !hotbarSlot2.IsEmpty())
		{
			var hotbarItem1 = itemList.Find(x => (x.GetItemName().Equals(hotbarSlot.GetItem().GetItemName())));
			var hotbarItem2 = itemList.Find(x => (x.GetItemName().Equals(hotbarSlot2.GetItem().GetItemName())));

			hotbarSlot2.SetItem(hotbarItem1);
			hotbarSlot.SetItem(hotbarItem2);
		}
		else if (hotbarSlot2.IsEmpty() && !hotbarSlot.IsEmpty())
		{
			var hotbarItem1 = itemList.Find(x => (x.GetItemName().Equals(hotbarSlot.GetItem().GetItemName())));

			hotbarSlot2.SetItem(hotbarItem1);
			hotbarSlot.ClearSlot();
		}
		else
			return;

		if (hotbarSlot.IsSelected())
			weaponController.UpdateTurret(hotbarSlot.GetItem() as ShipComponentBase);
		else if (hotbarSlot2.IsSelected())
			weaponController.UpdateTurret(hotbarSlot2.GetItem() as ShipComponentBase);

		audioSource.PlayOneShot(swapSound);
	}

	/// <summary>
	/// Helper method used to swap the contents of an inventory slot to a mount slot, or vice versa.
	/// </summary>
	/// <param name="invSlot"></param>
	/// <param name="mountSlot"></param>
	private void SwapInventoryToMountSlot(InventorySlot invSlot, MountSlot mountSlot)
	{
		var invComp = invSlot.GetItem() as ShipComponentBase;
		var mountComp = mountSlot.GetItem() as ShipComponentBase;

		// If they both contain an item.
		if (!mountSlot.IsEmpty() && !invSlot.IsEmpty())
		{
			// Only swap if the components are the same type. The tier of the incoming can be greater.
			if (mountSlot.GetMount().IsComponentCompatible(invComp))
			{
				if (invComp is ActiveAbilityComponent && !(mountComp is ActiveAbilityComponent) && playerController.GetAbility() != null)
				{
					OutputWindow.current.DisplayText("<color=\"white\">Ship already has an active ability</color>");
					return;
				}
				var invItem = itemList.Find(x => (x.GetItemName().Equals(invSlot.GetItem().GetItemName()))) as ShipComponentBase;
				var mountItem = itemList.Find(x => (x.GetItemName().Equals(mountSlot.GetItem().GetItemName()))) as ShipComponentBase;
				invSlot.SetItem(mountItem);
				mountSlot.SetItem(invItem);
			}
			else
			{
				OutputWindow.current.DisplayText(invSlot.GetItem().GetPrettyName() + "<color=\"white\"> is not compatible with this mount</color>");
				return;
			}
		}
		// If inventory slot is empty.
		else if (invSlot.IsEmpty() && !mountSlot.IsEmpty())
		{
			var mountItem = itemList.Find(x => (x.GetItemName().Equals(mountSlot.GetItem().GetItemName()))) as ShipComponentBase;
			if (mountComp.GetItemType() == ItemType.Thruster)
			{
				OutputWindow.current.DisplayText("<color=\"white\">Cannot remove thruster from ship.</color>");
				return;
			}

			mountSlot.ClearSlot();
			invSlot.SetItem(mountItem);
		}
		// If mount slot is empty.
		else if (mountSlot.IsEmpty() && !invSlot.IsEmpty())
		{
			// A player can only have one active ability at any given time, so make sure they can't add a second one.
			if (invComp is ActiveAbilityComponent && playerController.GetAbility() != null)
			{
				OutputWindow.current.DisplayText("<color=\"white\">Ship already has an active ability</color>");
				return;
			}

			if (mountSlot.GetMount().IsComponentCompatible(invComp))
			{
				var invItem = itemList.Find(x => (x.GetItemName().Equals(invSlot.GetItem().GetItemName()))) as ShipComponentBase;
				mountSlot.SetItem(invItem);
				if (invComp is ThrusterComponent)
					playerController.UpdateThrusterList();

				invSlot.ClearSlot();
			}
			else
			{
				OutputWindow.current.DisplayText(invSlot.GetItem().GetPrettyName() + "<color=\"white\"> is not compatible with this mount</color>");
				return;
			}

		}
		else
			return;

		audioSource.PlayOneShot(swapSound);
	}

	/// <summary>
	/// Swap two mount slots.
	/// </summary>
	/// <param name="mountSlot1"></param>
	/// <param name="mountSlot2"></param>
	private void SwapMountSlots(MountSlot mountSlot1, MountSlot mountSlot2)
	{
		var mountComp1 = mountSlot1.GetItem() as ShipComponentBase;
		var mountComp2 = mountSlot2.GetItem() as ShipComponentBase;

		// If both mount slots contain an item, and are compatible, swap them.
		if (!mountSlot1.IsEmpty() && !mountSlot2.IsEmpty() && mountSlot1.GetMount().IsComponentCompatible(mountComp2)
														  && mountSlot2.GetMount().IsComponentCompatible(mountComp1))
		{
			mountSlot1.SetItem(itemList.Find(x => (x.GetItemName().Equals(mountComp2.GetItemName()))) as ShipComponentBase);
			mountSlot2.SetItem(itemList.Find(x => (x.GetItemName().Equals(mountComp1.GetItemName()))) as ShipComponentBase);
		}
		// If swapping to an empty mount slot.
		else if (!mountSlot1.IsEmpty() && mountSlot2.IsEmpty() && mountSlot2.GetMount().IsComponentCompatible(mountComp1))
		{
			var comp = itemList.Find(x => (x.GetItemName().Equals(mountComp1.GetItemName())));
			mountSlot1.ClearSlot();
			mountSlot2.SetItem(comp as ShipComponentBase);
		}
		else
		{
			OutputWindow.current.DisplayText("<color=\"white\">Mounts are incompatible</color>");
			return;

		}

		audioSource.PlayOneShot(swapSound);
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
		if (item is ActiveAbilityComponent)
		{
			slotIndex = GetIndexOfActiveAbilityMountSlot(item.itemTier);
		}
		else
			slotIndex = GetIndexOfEmptyOrFirstMountSlotOfType(item.GetItemType(), item.itemTier);

		if (slotIndex == -2)
		{
			OutputWindow.current.DisplayText("<color=\"white\">Ship already has an active ability, and they cannot be swapped.</color>");
			return;
		}
		else if (slotIndex == -1)
			slotIndex = GetIndexOfEmptyOrFirstMountSlotOfType(item.GetItemType(), item.itemTier);

		if (slotIndex >= 0)
			SwapSlots(new int[] { index, slotIndex });
		else
			OutputWindow.current.DisplayText("Not compatible mount found.");
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
	public int GetIndexOfEmptyOrFirstMountSlotOfType(ItemType type, ItemTier tier)
	{
		foreach (var mountSlot in mountSlots.Where(e => e.GetMount().GetMountType() == type && e.GetMount().GetMountTier() >= tier))
		{
			if (mountSlot.GetItem() == null)
				return mountSlot.GetIndex();
		}
		var first = mountSlots.Where(e => e.GetMount().GetMountType() == type && e.GetMount().GetMountTier() >= tier).OrderBy(e => e.GetIndex()).FirstOrDefault();
		if (first != null)
			return first.GetIndex();
		return -1;
	}

	/// <summary>
	/// Get the index of a mount slot containing an active ability component. This method is used when the incoming item is an active ability.
	/// A player can only have one active ability at a time.
	/// </summary>
	/// <param name="tier"></param>
	/// <returns>Index of mount slot to swap, or -1 if no mount slot has an active ability, or -2 if a mount slot has an active ability, 
	/// but cannot be swapped due to the tier.</returns>
	public int GetIndexOfActiveAbilityMountSlot(ItemTier tier)
	{
		MountSlot result = null;
		List<MountSlot> possibleMounts = mountSlots.Where(e => e.GetMount().GetShipComponent() != null && e.GetMount().GetShipComponent() is ActiveAbilityComponent).ToList();
		if (possibleMounts != null)
			result = possibleMounts.Where(e => e.GetMount().GetMountTier() >= tier).FirstOrDefault();

		if (result == null)
		{
			if (possibleMounts.Count() == 0)
			{
				return -1;
			}
			return -2;
		}
		return result.GetIndex();
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
			if (slot.IsEmpty()) { return true; } // Empty slot found
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
			if (slot.IsEmpty()) { return slot; } // Empty slot found
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
			if (slot.IsEmpty()) { return slot; } // Empty slot found
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
        if (item == null)
            return false;

		// Find element in item list equivalent to the parameter.
		foreach (InventorySlot slot in inventorySlots)
		{
			if (slot.GetItem() != null)
			{
				if (slot.GetItem().GetItemName().Equals(item.GetItemName()) && slot.GetQuantity() < slot.GetItem().stackSize)
					return true;
			}
		}
		return false; // Find element in item list equivalent to the parameter.
	}

	/// <summary>
	/// Set whether all slots can hightlight when the user hovers their mouse over them.
	/// </summary>
	private void ToggleSlotHoverEffect(bool val)
	{
		ToggleInventorySlotHoverEffect(val);
		ToggleHotbarSlotHoverEffect(val);
		ToggleMountSlotHoverEffect(val);
	}

	/// <summary>
	/// Set whether mount slots can highlight or not when hovered over.
	/// </summary>
	/// <param name="val"></param>
	private void ToggleInventorySlotHoverEffect(bool val)
	{
		foreach (var slot in inventorySlots)
		{
			slot.canHighlight = val;
		}
	}

	/// <summary>
	/// Set whether mount slots can highlight or not when hovered over.
	/// </summary>
	/// <param name="val"></param>
	private void ToggleHotbarSlotHoverEffect(bool val)
	{
		foreach (var slot in hotbarSlots)
		{
			slot.canHighlight = val;
		}
	}

	/// <summary>
	/// Set whether mount slots can highlight or not when hovered over.
	/// </summary>
	/// <param name="val"></param>
	private void ToggleMountSlotHoverEffect(bool val)
	{
		foreach (var slot in mountSlots)
		{
			slot.canHighlight = val;
		}
	}
}
