using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	// This is used for debugging and testing.
	public Item StartingItem;
	public int StartingQuantity;

	// Item data
	private int quantity;
	private bool swapping;
	private bool rightClickSwapping;
	private Item item;
	private int animFrameIndex;

	private Image image;
	// For mounting slots.
	private bool mounting;
	private MountSlot mountSlot;

	// Tracks and displays the quantity of this inventory item.
	private TextMeshProUGUI count;

	// If swapping slots, send off these positions.
	private int[] positions;
	internal static bool dragging = false;
	internal static bool rightClickDragging = false;
	// If the inventory item is not visible, it is not draggable.
	internal static bool draggable;
	private bool highlighted = false;

	protected Coroutine animate;
	protected bool animating;

	void Awake()
	{
		image = GetComponent<Image>();
		count = GetComponentInChildren<TextMeshProUGUI>();
		positions = new int[2];
		positions[0] = GetComponentInParent<SlotBase>().GetIndex();

		if (StartingItem != null)
		{
			if (!StartingItem.stackable || StartingQuantity > 0)
			{
				SetItem(StartingItem, StartingQuantity);
			}
		}
	}

	/// <summary>
	/// Set the item.
	/// </summary>
	/// <param name="incomingItem"></param>
	/// <param name="quantity"></param>
	public Item SetItem(Item incomingItem, int quantity, bool createNewCopy = true)
	{
		this.quantity = quantity;
		if (animating)
		{
			StopCoroutine(animate);
			animate = null;
			animating = false;
		}
		// If the current item for this inventory item is not empty, destroy the old item first.
		if (createNewCopy)
		{
			if (item != null)
			{
				Destroy(item.gameObject);
				item = Instantiate(incomingItem, transform.position, Quaternion.identity, transform) as Item;
			}
			else if (incomingItem != null)
				item = Instantiate(incomingItem, transform.position, Quaternion.identity, transform) as Item;
			else
			{
				Clear();
				return null;
			}
			// Hide the item so that it isn't displayed in the game world.
			item.gameObject.SetActive(false);
		}
		else
			item = incomingItem;

		image.color = (item != null) ? new Color(1.0f, 1.0f, 1.0f, 0.7f) : new Color(1.0f, 1.0f, 1.0f, 0.0f);
		if (count != null)
		{
			if (item != null && item.stackable)
				count.text = quantity.ToString();
			else
				count.text = "";
		}
		gameObject.SetActive(true);
		if (item.animated && !animating)
			animate = StartCoroutine(Animate());
		else
			image.sprite = item.itemSprite;

		return item;
	}

	private IEnumerator Animate()
	{
		while (item != null)
		{
			animating = true;
			image.sprite = item.idleAnimation.GetNextFrame();
			yield return new WaitForSeconds(item.idleAnimation.playSpeed);
		}
		animating = false;
	}

	/// <summary>
	/// Get the item.
	/// </summary>
	/// <returns></returns>
	public Item GetItem()
	{
		return item;
	}

	/// <summary>
	/// Set the quantity of the item represented by this inventory item. Typically called when the item represented
	/// already exists in the inventory, is stackable, and the player picked up more of this item or removed some.
	/// </summary>
	/// <param name="quantity"></param>
	public void SetQuantity(int quantity)
	{
		if (!IsStackable())
			return;

		this.quantity = quantity;
		count.text = this.quantity + "";
	}

	/// <summary>
	/// Add to the current quantity of this inventory item.
	/// </summary>
	/// <param name="quantity"></param>
	public void AddQuantity(int quantity)
	{
		SetQuantity(this.quantity + quantity);
	}

	/// <summary>
	/// Subtract from the current quantity of this inventory item.
	/// </summary>
	/// <param name="quantity"></param>
	public void SubtractQuantity(int quantity)
	{
		SetQuantity(this.quantity - quantity);
	}

	/// <summary>
	/// Get the quantity of the item represented by this inventory item.
	/// </summary>
	/// <returns></returns>
	public int GetQuantity()
	{
		return quantity;
	}

	/// <summary>
	/// Get the item type of the item represented by this inventory item.
	/// </summary>
	/// <returns></returns>
	public ItemType GetItemType()
	{
		return item.GetItemType();
	}

	/// <summary>
	/// Get whether the item represented by this inventory item is stackable.
	/// </summary>
	/// <returns></returns>
	public bool IsStackable()
	{
		return item.stackable;
	}

	/// <summary>
	/// Get the maximum stack size of the item. If the item is not stackable, the method will return 0.
	/// </summary>
	/// <returns></returns>
	public int GetStackSize()
	{
		return item.stackSize;
	}

	/// <summary>
	/// Called when the gameobject is disabled.
	/// </summary>
	void OnDisable()
	{
		highlighted = false;
	}

	/// <summary>
	/// Highlight the image.
	/// </summary>
	public void Highlight()
	{
		if (!highlighted && image != null)
		{
			image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			highlighted = true;
		}
	}

	/// <summary>
	/// Dehighlight the image.
	/// </summary>
	public void Dehighlight()
	{
		if (highlighted && image != null)
		{
			image.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
			highlighted = false;
		}
	}

	/// <summary>
	/// Clear the inventory item.
	/// </summary>
	public void Clear(bool destroyCopy = true)
	{
		if (animate != null)
		{
			StopCoroutine(animate);
			animate = null;
			animating = false;
		}
		image.sprite = null;
		quantity = 0;
		if (destroyCopy)
			Destroy(item.gameObject);
		item = null;
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Disable the tool tip when dragging.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (Input.GetKey(KeyCode.LeftShift))
			return;
		// Cannot right click drag and left click drag at the same time.
		if (eventData.button == PointerEventData.InputButton.Left && draggable && item != null && !rightClickDragging)
		{
			if (GetComponentInParent<SlotBase>() != null)
				positions[0] = GetComponentInParent<SlotBase>().GetIndex();
			dragging = true;
			InfoScreen.current.Hide();
		}
		else if (eventData.button == PointerEventData.InputButton.Right && draggable && item != null && !dragging)
		{
			if (GetComponentInParent<SlotBase>() != null)
				positions[0] = GetComponentInParent<SlotBase>().GetIndex();
			rightClickDragging = true;
			InfoScreen.current.Hide();
		}
	}

	/// <summary>
	/// Drag the item.
	/// </summary>
	public void OnDrag(PointerEventData eventData)
	{
		if (!rightClickDragging)
		{
			if (eventData.button == PointerEventData.InputButton.Left && draggable && item != null)
			{
				if (image == null) { return; }

				var newPos = Input.mousePosition;
				newPos.z = transform.position.z - Camera.main.transform.position.z;
				transform.position = Camera.main.ScreenToWorldPoint(newPos);
			}
		}
		else if (!dragging)
		{
			if (eventData.button == PointerEventData.InputButton.Right && draggable && item != null)
			{
				if (image == null) { return; }

				var newPos = Input.mousePosition;
				newPos.z = transform.position.z - Camera.main.transform.position.z;
				transform.position = Camera.main.ScreenToWorldPoint(newPos);
			}
		}
	}

	/// <summary>
	/// Return the item to it's original position, if no valid slot is present.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnEndDrag(PointerEventData eventData)
	{
		if (rightClickDragging && eventData != null && eventData.button != PointerEventData.InputButton.Right)
			return;
		else if (dragging && eventData.button != PointerEventData.InputButton.Left)
			return;
		if (image == null || !draggable || item == null) { return; }
		dragging = false;
		rightClickDragging = false;

		if (swapping || mounting)
		{
			SendMessageUpwards("SwapSlots", positions);
			swapping = false;
		}
		else if (rightClickSwapping)
		{
			// If the item is not stackable, then just swap as usual with the right-click drag.
			if (IsStackable())
				SendMessageUpwards("PromptUserForAmountToSwap", positions);
			else
				SendMessageUpwards("SwapSlots", positions);

			rightClickSwapping = false;
		}

		transform.position = transform.parent.gameObject.transform.position;

	}

	/// <summary>
	/// Deals with changing slots.
	/// </summary>
	/// <param name="collider"></param>
	void OnTriggerEnter2D(Collider2D collider)
	{
		switch (collider.gameObject.tag)
		{
			case ("InventorySlot"):
				positions[1] = collider.gameObject.GetComponentInParent<InventorySlot>().GetIndex();
				if (dragging)
					swapping = true;
				else if (rightClickDragging)
					rightClickSwapping = true;
				break;
			case ("MountSlot"):
				mountSlot = collider.gameObject.GetComponent<MountSlot>();
				positions[1] = mountSlot.GetIndex();
				mounting = true;
				break;
			case ("HotbarSlot"):
				var hotbarSlot = collider.gameObject.GetComponent<HotbarSlot>();
				positions[1] = hotbarSlot.GetIndex();
				if (dragging)
					swapping = true;
				else if (rightClickDragging)
					rightClickSwapping = true;
				break;
		}
	}

	/// <summary>
	/// Deals with changing slots.
	/// </summary>
	/// <param name="collider"></param>
	void OnTriggerStay2D(Collider2D collider)
	{
		switch (collider.gameObject.tag)
		{
			case ("InventorySlot"):
				positions[1] = collider.gameObject.GetComponentInParent<InventorySlot>().GetIndex();
				if (dragging)
					swapping = true;
				else if (rightClickDragging)
					rightClickSwapping = true;
				break;
			case ("MountSlot"):
				mountSlot = collider.gameObject.GetComponent<MountSlot>();
				positions[1] = mountSlot.GetIndex();
				mounting = true;
				break;
			case ("HotbarSlot"):
				positions[1] = collider.gameObject.GetComponent<HotbarSlot>().GetIndex();
				if (dragging)
					swapping = true;
				else if (rightClickDragging)
					rightClickSwapping = true;
				break;
		}
	}

	/// <summary>
	/// Deals with resetting newSlotPosition to null.
	/// </summary>
	/// <param name="collider"></param>
	void OnTriggerExit2D(Collider2D collider)
	{
		swapping = false;
		rightClickSwapping = false;
		mounting = false;
		mountSlot = null;
	}
}
