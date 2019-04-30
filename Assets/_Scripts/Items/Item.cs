using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour
{
	public SpriteAnimation idleAnimation;
	public Sprite itemSprite;
    public GameObject hoverText;

    public ItemType itemType;
    public ItemTier itemTier;
    public string itemName;
    public int quantity = 1;
    public int value;
    [TextArea(1, 5)]
    public string description;
    public bool stackable;
    public int stackSize;
    public Color itemColor;
    public SpriteRenderer spriteRenderer;
	public bool animated;

    private const int FOLLOWSPEED = 50;
    private const int FOLLOWANGLEMAX = 10;
    private float distance = 5.0f;
    private GameObject targetPlayer;
    private Vector2 movementVec;

    protected Coroutine animate;
    protected bool animating;

    protected virtual void Awake()
    {
        itemColor = ItemColors.colors[(int)itemTier];
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameObject.tag = "Item";
	}

	private void OnEnable()
	{
		if (idleAnimation != null && animate == null)
		{
			animate = StartCoroutine(Animate());
		}
		else if (itemSprite != null)
		{
			spriteRenderer.sprite = itemSprite;
		}
		else
			Debug.LogError("Item " + this + " has no sprites!");
	}

    // Update is called once per frame
    protected virtual void Update()
    {
        // If the item is hidden, it should not hover toward the player or animate.
        if(spriteRenderer != null && spriteRenderer.enabled)
        {
            if(movementVec != Vector2.zero)
            {
                transform.position += (Vector3)movementVec;
                movementVec *= 0.9f;
            }
            HoverTowardPlayer(targetPlayer);
        }
    }

    /// <summary>
    /// Animate if the item has more than one sprite.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Animate()
    {
        while(gameObject.activeInHierarchy)
        {
			var sprite = idleAnimation.GetNextFrame();
			spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(idleAnimation.playSpeed);
        }
        animating = false;
    }

    /// <summary>
    /// The logic to move toward the player.
    /// </summary>
    internal virtual void HoverTowardPlayer(GameObject player = null)
    {
        if(player == null)
        {
            player = PlayerUtils.GetClosestPlayer(gameObject);

            // If the player's inventory is full, don't hover toward them.
            if (!player.GetComponent<PlayerController>().inventory.ContainsEmptySlot() && !player.GetComponent<PlayerController>().inventory.ContainsItem(this))
                return;

            var distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= distance)
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, FOLLOWSPEED * Time.deltaTime);
        }
        else
        {
            var distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if(distanceToPlayer <= distance)
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, FOLLOWSPEED * Time.deltaTime);
        }
    }

    /// <summary>
    /// Activates the item. Can provide a movement vector where the item will float and decrease speed in that direction.
    /// </summary>
    /// <param name="movementVector"></param>
    public void Activate(Vector2? movementVector = null)
    {
        if (movementVector != null)
            movementVec = (Vector2)movementVector;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivate the item. Relinquishes the item back to the item pool.
    /// </summary>
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetWasMined(bool mined, GameObject target = null)
    {
        if(target != null)
        {
            targetPlayer = target;
        }
    }

    /// <summary>
    /// get the quantity of the item.
    /// </summary>
    /// <returns></returns>
    public int GetQuantity()
    {
        return quantity;
    }

    /// <summary>
    /// Get the item type.
    /// </summary>
    /// <returns></returns>
    public ItemType GetItemType()
    {
        return itemType;
    }

    /// <summary>
    /// Get the name of the item.
    /// </summary>
    /// <returns></returns>
    public string GetItemName()
    {
        return itemName;
    }

	/// <summary>
	/// Get name colored with item's tier color.
	/// </summary>
	/// <returns></returns>
	public string GetPrettyName()
	{
		var hex = itemColor.ToHex();
		return "<color=" + hex + ">" + itemName + "</color>";
	}

    /// <summary>
    /// Is the item stackable?
    /// </summary>
    /// <returns></returns>
    public bool IsStackable()
    {
        return stackable;
    }

    /// <summary>
    /// Get the max stack size of the item.
    /// </summary>
    /// <returns></returns>
    public int GetStackSize()
    {
        return stackSize;
    }

    /// <summary>
    /// Activate the hover text.
    /// </summary>
    void OnMouseOver()
    {
        if(hoverText != null)
        {
            hoverText.SetActive(true);
            hoverText.GetComponent<TextMeshPro>().color = itemColor;
        }
    }
    
    /// <summary>
    /// Disable the hover text.
    /// </summary>
    void OnMouseExit()
    {
        if(hoverText != null)
            hoverText.SetActive(false);
    }

    /// <summary>
    /// Copy the display values and attributes of one item to this one.
    /// </summary>
    public void Copy(Item other)
    {
		animated = other.animated;
		idleAnimation = other.idleAnimation;
		itemSprite = other.itemSprite;
		itemType = other.itemType;
        itemTier = other.itemTier;
        itemName = other.itemName;
        value = other.value;
        description = other.description;
        stackable = other.stackable;
        stackSize = other.stackSize;
        itemColor = ItemColors.colors[(int)other.itemTier];
        distance = other.distance;
        targetPlayer = other.targetPlayer;
    }

    /// <summary>
    /// Initialize the item with some data.
    /// </summary>
    /// <param name="position">The target position.</param>
    /// <param name="distance">The item will hover toward the player if the player is within this distance.</param>
    /// <param name="other">If this is not null, this item will copy the data of the other.</param>
    public void Initialize(Vector2 position, float distance = 5.0f, Item other = null)
    {
        // Copy the details of the incoming object, if one was provided.
        if (other != null)
            Copy(other);

        this.distance = distance;
        transform.position = position;
        gameObject.SetActive(true);
    }
}
