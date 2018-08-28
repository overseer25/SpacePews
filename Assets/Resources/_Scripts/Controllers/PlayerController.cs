using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Converts inputs given by the player into actions and movements of the player character.
/// </summary>
public class PlayerController : MonoBehaviour
{

    private float acceleration;
    private Rigidbody2D rigidBody;
    private MovementController movementController;
    private GameObject turret;
    private GameObject thruster; // Contains the thruster sprite that plays when moving.
    private Vector2 moveInput;
    private bool playingEngine = false;
    private Vector3 previousCameraPosition; // Used to create floaty camera effect.
    [SerializeField]
    private int healthChunk = 20;
    private int prevHealth;
    private float healthToDisplay;
    private PlayerHealth healthUI;
    private bool dead = false;

    private int health = 200; // The amount of health the player currently has.
    public int maxHealth = 200; // Max health the player can currently have.
    public int currency = 5; // Amount of currency the player currently has.

    public AudioSource engine; // Engine sound
    public bool inertialDamp = true; // Are inertial dampeners on?

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        movementController = GetComponentInParent<MovementController>();
        healthToDisplay = maxHealth / healthChunk;
        healthUI = this.GetComponent<PlayerHealth>();
        healthUI.SetupHealthSprite((int)healthToDisplay);
        prevHealth = health;
    }

    ///// <summary>
    ///// Update is called once per frame
    ///// </summary>
    //void Update()
    //{
    //    // Gets the movement vector given by WASD.
    //    moveInput = !dead ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) : Vector2.zero;
    //    // If moving
    //    if (moveInput != Vector2.zero)
    //    {
    //        thruster.GetComponent<SpriteRenderer>().enabled = true;
    //        if (acceleration < maxSpeed)
    //            acceleration += 0.01f;
    //        if (!playingEngine)
    //        {
    //            engine.Play(); // Play engine sound while moving
    //            playingEngine = true;
    //        }
    //        engine.volume = 1.0f; // Max ship volume when accelerating
    //        if (engine.pitch < 1.0f) { engine.pitch += 0.1f; } // Increase pitch to signify accelerating

    //    }
    //    // Otherwise, if player isn't pressing WASD.
    //    else
    //    {
    //        thruster.GetComponent<SpriteRenderer>().enabled = false; // Disable thruster graphic

    //        // Decelerate
    //        if (acceleration > 0)
    //            acceleration -= 0.003f;

    //        engine.volume -= 0.005f; // Quiet the engine down as ship slows
    //        if (engine.pitch > 0.0f) { engine.pitch -= 0.005f; } // Reduce pitch when slowing down to represent engine slowing
    //        if (engine.volume <= 0)
    //        {
    //            engine.Stop(); // Stop the engine sound when not moving
    //            playingEngine = false;
    //        }
    //    }

    //    // Toggle inertial dampeners
    //    if (Input.GetKeyDown("f") && !dead)
    //    {
    //        inertialDamp = !inertialDamp;
    //    }

    //}

    /// <summary>
    /// Gets the current health value of the player.
    /// </summary>
    /// <returns></returns>
    public int GetHealth()
    {
        return health;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            movementController.MoveForward();
        }
        else
        {
            movementController.Decelerate();
        }
        if(Input.GetKey(KeyCode.D))
        {
            movementController.RotateRight();
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementController.RotateLeft();
        }

    }

    private void LateUpdate()
    {
        if (!CheckIfDead() && prevHealth != health)
        {
            prevHealth = health;
            DisplayHealth();
        }
    }

    /// <summary>
    /// Deals with all collisions with the player character
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {

        switch (collider.gameObject.tag)
        {
            case "Item":
                Item item = collider.gameObject.GetComponent<Item>();
                Inventory inventory = GameObject.Find("InventoryHUD").GetComponent<Inventory>();
                // Only add the item to the player's inventory list if there is an empty slot for it, or a slot contains the item already.
                if (inventory.IsEmptySlot() || inventory.ContainsItem(item))
                {
                    GameObject.Find("InventoryHUD").GetComponent<Inventory>().AddItem(item); // Add the item to the player inventory.
                    //collider.gameObject.GetComponent<Item>().CreateCollectItemSprite();
                }
                break;
            case "Immovable":
            case "Asteroid":
            case "Mineable":
                Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
                Vector2 direction = Vector2.zero; // Direction of the collision, with the magnitude applied being the speed of the player.
                // If the player isn't moving, we need to move them away, as they would be able to clip into the immovable otherwise.
                if (rigidBody.velocity == Vector2.zero)
                {
                    direction = (gameObject.transform.position - collider.gameObject.transform.position) * 20;
                }
                // The difference here is the player has a velocity, so we will propel them away from the immovable using their velocity magnitude.
                else
                {
                    // Different damage values depending on speed of collision.
                    if (rigidBody.velocity.magnitude >= 3 && rigidBody.velocity.magnitude < 5)
                    {
                        health--;
                    }
                    else if (rigidBody.velocity.magnitude >= 5 && rigidBody.velocity.magnitude < 10)
                    {

                        health -= 2;
                    }
                    else if (rigidBody.velocity.magnitude >= 10)
                    {
                        health -= 3;
                    }
                    direction = (gameObject.transform.position - collider.gameObject.transform.position).normalized * (rigidBody.velocity.magnitude * 100);
                }
                rigidBody.AddForce(direction); // Apply the force
                break;
            case "Mine":
                rigidBody = gameObject.GetComponent<Rigidbody2D>();
                direction = Vector2.zero; // Direction of the collision, with the magnitude applied being the speed of the player.
                // If the player isn't moving, we need to move them away, as they would be able to clip into the immovable otherwise.
                if (rigidBody.velocity == Vector2.zero)
                {
                    direction = (gameObject.transform.position - collider.gameObject.transform.position) * 20;
                }
                // The difference here is the player has a velocity, so we will propel them away from the immovable using their velocity magnitude.
                else
                {
                    direction = (gameObject.transform.position - collider.gameObject.transform.position).normalized * (rigidBody.velocity.magnitude * 100);
                }
                rigidBody.AddForce(direction); // Apply the force
                break;
            case "EnemyProjectile":
                health -= collider.gameObject.GetComponent<Projectile>().Damage;
                break;

        }       
    }

    /// <summary>
    /// Calculates number of hearts to draw, and transparency of last one if necessary. Sends to
    /// UI for updating.
    /// </summary>
    private void DisplayHealth()
    {
        healthToDisplay = (float)health / healthChunk;
        int fullDisplayHealth = health / healthChunk;
        float remainingHealth = healthToDisplay - fullDisplayHealth;
        healthUI.RedrawHealthSprites(fullDisplayHealth, remainingHealth);
    }

    /// <summary>
    /// Deals with all collisions exiting the player
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerExit2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            // Does nothing yet.
        }
    }

    /// <summary>
    /// Check to see if the player is dead, set some variables if he is and render them dead.
    /// </summary>
    /// <returns>Returns true if health is less than or equal to 0</returns>
    public bool CheckIfDead()
    {
        if(health <= 0)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            dead = true;
            this.SendMessage("UpdateDead", true);
            healthUI.SetIsDead(true);
            healthUI.RedrawHealthSprites(0, 0);
        }
        return health <= 0;
    }
}
