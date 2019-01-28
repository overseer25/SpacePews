﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour
{
    [Header("Display")]
    public Sprite[] inventorySprites; // If more than one sprite, this will animate using the playspeed variable.
    public GameObject hoverText;

    [Header("Attributes")]
    [SerializeField]
    internal ItemType type;
    [SerializeField]
    internal ItemTier itemTier;
    [SerializeField]
    internal string itemName;
    [SerializeField]
    internal int quantity = 1;
    [SerializeField]
    internal int value;

    [SerializeField]
    [TextArea(10,10)]
    internal string description;

    [Header("Other")]
    [SerializeField]
    private AudioClip pickupSound;
    public bool stackable;
    public int stackSize;

    // For playing a sprite animation, if it exists.
    internal float playspeed = 0.5f;
    internal float changeSprite = 0.0f;
    internal int index = 0;
    internal Color itemColor;

    private const int FOLLOWSPEED = 50;
    private const int FOLLOWANGLEMAX = 10;
    private int followSpeed = FOLLOWSPEED;
    private const float MAXDISTANCE = 5.0f;
    private bool mined = false;
    private float minedFollowSpeed = FOLLOWSPEED; // Speed at which a mined item follows the player.
    private float minedFollowAngle; // Angle of descrepancy so that the items don't come out in a straight line.
    private Vector2 startingPos;
    private GameObject targetPlayer;
    private SpriteRenderer spriteRenderer;
    private static System.Random random;

    protected virtual void Awake()
    {
        random = new System.Random();
        itemColor = ItemColors.colors[(int)itemTier];
    }

    // Creates the object to play the collection sprite, and destroys the item.
    internal void DisplayHoverText()
    {
        var collected = PopUpTextPool.current.GetPooledObject();

        // if there is an available pop up text, display it.
        if(collected != null)
        {
            collected.GetComponent<PopUpText>().Initialize(PlayerUtils.GetClosestPlayer(gameObject), itemName, itemTier, pickupSound);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the item is hidden, it should not hover toward the player or animate.
        if(GetSpriteRenderer() != null && spriteRenderer.enabled)
        {
            if (inventorySprites.Length > 0)
            {
                // Player sprite animation
                if (Time.time > changeSprite)
                {
                    changeSprite = Time.time + playspeed;
                    index++;
                    if (index >= inventorySprites.Length) { index = 0; } // Restart animation
                    spriteRenderer.sprite = inventorySprites[index];
                }
            }
            HoverTowardPlayer(targetPlayer);
        }
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
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, followSpeed * Time.deltaTime);
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
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, followSpeed * Time.deltaTime);
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
    /// Get the sprite renderer of the item.
    /// </summary>
    /// <returns></returns>
    public SpriteRenderer GetSpriteRenderer()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        return spriteRenderer;
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
        return type;
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
                    DisplayHoverText();
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

        inventorySprites = other.inventorySprites;
        GetComponent<SpriteRenderer>().sprite = inventorySprites[0];
        type = other.type;
        itemTier = other.itemTier;
        itemName = other.itemName;
        value = other.value;
        description = other.description;
        stackable = other.stackable;
        stackSize = other.stackSize;
        playspeed = other.playspeed;
        changeSprite = other.changeSprite;
        index = other.index;
        itemColor = other.itemColor;
        pickupSound = other.pickupSound;
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
