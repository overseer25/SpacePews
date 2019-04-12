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
    private const float MAXDISTANCE = 5.0f;
    private bool mined = false;
    private float minedFollowSpeed = FOLLOWSPEED; // Speed at which a mined item follows the player.
    private float minedFollowAngle; // Angle of descrepancy so that the items don't come out in a straight line.
    private Vector2 startingPos;
    private GameObject targetPlayer;
    private static System.Random random;

    protected Coroutine animate;
    protected bool animating;

    protected virtual void Awake()
    {
        random = new System.Random();
        itemColor = ItemColors.colors[(int)itemTier];
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
            if (distanceToPlayer <= MAXDISTANCE)
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, FOLLOWSPEED * Time.deltaTime);
        }
        else
        {
            var distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // If the player's inventory is full, don't hover toward them.
            if ((!player.GetComponent<PlayerController>().inventory.ContainsEmptySlot() && !player.GetComponent<PlayerController>().inventory.ContainsItem(this))
                    || distanceToPlayer > MAXDISTANCE)
            {
                // If the player mining cannot collect the items, make it fair game to any nearby player.
                if (minedFollowSpeed != 0.0f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, minedFollowSpeed * Time.deltaTime);
                    minedFollowSpeed *= 0.9f * (1 - Time.deltaTime);
                }
                else
                    player = null;
            }
            else if(distanceToPlayer <= MAXDISTANCE)
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, FOLLOWSPEED * Time.deltaTime);
        }
    }

    public void SetWasMined(bool mined, GameObject target = null)
    {
        this.mined = mined;
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
    /// Deals with object collisions.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        var obj = collider.GetComponentInParent<Rigidbody2D>();

        if (obj == null)
            return;
        switch (obj.gameObject.tag)
        {
            case "Player":
                // If the player's inventory is full, don't add to their inventory.
                if(obj.GetComponent<PlayerController>().inventory.ContainsEmptySlot() || obj.GetComponent<PlayerController>().inventory.ContainsItem(this))
                {
                    obj.GetComponent<PlayerController>().inventory.AddItem(this);
                    gameObject.SetActive(false);
                }
                break;
            default:
                return;
        }
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
        mined = other.mined;
        targetPlayer = other.targetPlayer;
    }

    /// <summary>
    /// Initialize the item's position.
    /// </summary>
    public void Initialize(GameObject target, Item other = null)
    {
        // Copy the details of the incoming object, if one was provided.
        if (other != null)
            Copy(other);

        minedFollowSpeed = FOLLOWSPEED;
        minedFollowAngle = random.Next(-FOLLOWANGLEMAX, FOLLOWANGLEMAX);
        transform.position = target.transform.position;
        startingPos = transform.position;
        gameObject.SetActive(true);
    }
}
