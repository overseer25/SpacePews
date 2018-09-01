using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Converts inputs given by the player into actions and movements of the player character.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("State")]
    public Inventory inventory;

    private float acceleration;
    private Rigidbody2D rigidBody;
    private MovementController movementController;
    private GameObject turret;
    private Thruster[] thrusters; // Contains the thrusters of the ship;
    private Vector2 moveInput;
    private bool playingEngine = false;
    private Vector3 previousCameraPosition; // Used to create floaty camera effect.
    [SerializeField]
    private int healthChunk = 20;
    private int prevHealth;
    private float healthToDisplay;
    private PlayerHealth healthUI;
    private bool dead = false;

    private int health; // The amount of health the player currently has.
    public int maxHealth = 200; // Max health the player can currently have.
    public int currency = 5; // Amount of currency the player currently has.

    public AudioSource engine; // Engine sound
    public bool inertialDamp = true; // Are inertial dampeners on?

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        health = maxHealth;
        movementController = gameObject.GetComponent<MovementController>();
        healthToDisplay = maxHealth / healthChunk;
        healthUI = this.GetComponent<PlayerHealth>();
        healthUI.SetupHealthSprite((int)healthToDisplay);
        prevHealth = health;

        thrusters = GetComponentsInChildren<Thruster>();
    }

    /// <summary>
    /// Gets the current health value of the player.
    /// </summary>
    /// <returns></returns>
    public int GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Deals with inputs.
    /// </summary>
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            SetThrusterState(true);
            movementController.MoveForward();
            if(!engine.isPlaying)
                engine.Play();
        }
        else
        {
            SetThrusterState(false);
            movementController.Decelerate();
            engine.Stop();
        }
        if(Input.GetKey(KeyCode.D))
        {
            movementController.RotateRight();
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementController.RotateLeft();
        }

        if (Input.GetKeyDown("i"))
        {
            gameObject.GetComponent<WeaponController>().menuOpen = !gameObject.GetComponent<WeaponController>().menuOpen;
            inventory.Toggle();
        }

    }

    /// <summary>
    /// Set the state of all thrusters on the ship.
    /// </summary>
    /// <param name="state"></param>
    private void SetThrusterState(bool state)
    {
        foreach (var thruster in thrusters)
            thruster.gameObject.SetActive(state);
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
                // Only add the item to the player's inventory list if there is an empty slot for it, or a slot contains the item already.
                inventory.AddItem(item); // Add the item to the player inventory.
                //collider.gameObject.GetComponent<Item>().CreateCollectItemSprite();
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
    /// Check to see if the player is dead, set some variables if he is and render them dead.
    /// </summary>
    /// <returns>Returns true if health is less than or equal to 0</returns>
    public bool CheckIfDead()
    {
        if(health <= 0)
        {
            this.GetComponentInChildren<SpriteRenderer>().enabled = false;
            dead = true;
            this.SendMessage("UpdateDead", true);
            healthUI.SetIsDead(true);
            healthUI.RedrawHealthSprites(0, 0);
        }
        return health <= 0;
    }
}
