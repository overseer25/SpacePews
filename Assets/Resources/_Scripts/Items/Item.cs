using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour
{
    [Header("Display")]
    public Sprite[] spriteAnim; // For animation
    public Sprite sprite; // For no animation
    public GameObject hoverText;
    public ItemTier itemTier = ItemTier.Tier1;

    [Header("Attributes")]
    public new string name;
    public int quantity = 0;
    public int value;
    public string description;
    public string type;

    [Header("Other")]
    [SerializeField]
    private AudioClip pickupSound;
    public bool stackable;
    public int stackSize;

    // For playing a sprite animation, if it exists.
    internal float playspeed = 0.5f;
    internal float changeSprite = 0.0f;
    internal int index = 0;

    private int followSpeed = 5;
    private const float MAXDISTANCE = 1.0f;
    private bool mined = false;
    internal Color itemColor;

    private Transform playerPos;

    private void Start()
    {
        itemColor = ItemColors.colors[(int)itemTier];
    }

    // Creates the object to play the collection sprite, and destroys the item.
    internal void DisplayHoverText()
    {
        var collected = PopUpTextPool.current.GetPooledObject();

        // if there is an available pop up text, display it.
        if(collected != null)
        {
            collected.GetComponent<PopUpText>().Initialize(PlayerUtils.GetClosestPlayer(gameObject), name, itemTier, pickupSound);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteAnim.Length > 0)
        {
            // Player sprite animation
            if (Time.time > changeSprite)
            {
                changeSprite = Time.time + playspeed;
                index++;
                if (index >= spriteAnim.Length) { index = 0; } // Restart animation
                GetComponentInChildren<SpriteRenderer>().sprite = spriteAnim[index];
            }
        }
        if (mined)
        {
            HoverTowardsTargetPlayer();
        }
        else
        {
            HoverTowardPlayer();
        }
    }

    /// <summary>
    /// The logic to move toward the player.
    /// </summary>
    internal virtual void HoverTowardPlayer()
    {
        var closestPlayer = PlayerUtils.GetClosestPlayer(gameObject);

        // If the player's inventory is full, don't hover toward them.
        if (!closestPlayer.GetComponent<PlayerController>().inventory.ContainsEmptySlot())
        {
            return;
        }

        var distanceToPlayer = Vector2.Distance(transform.position, closestPlayer.transform.position);
        if (distanceToPlayer <= MAXDISTANCE)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestPlayer.transform.position, followSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Cause the item that was just mined to fly towards the player who obtained
    /// this resource.
    /// </summary>
    /// <param name="playerPos">The reference to the player and where they are.</param>
    private void HoverTowardsTargetPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPos.position, followSpeed * Time.deltaTime);
    }

    public void SetWasMined(bool mined, Transform target = null)
    {
        this.mined = mined;
        if(target != null)
        {
            playerPos = target;
        }
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
}
