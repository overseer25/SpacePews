using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Converts inputs given by the player into actions and movements of the player character.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Constants
    private const float RESPAWN_WAIT_TIME = 5.0f;
    private const float RESPAWN_ANIMATION_TIME = 0.5f;

    [Header("State")]
    [SerializeField]
    private int healthChunk = 20;
    public int maxHealth = 200; // Max health the player can currently have.
    public int currency = 5; // Amount of currency the player currently has.
    public bool inertialDamp = true; // Are inertial dampeners on?
    public Inventory inventory;
    public DeathScreen deathScreen;
    public PauseMenuScript pauseMenu;
    public GameObject respawnPoint;
    [Header("Effects/Sounds")]
    public GameObject deathExplosion;
    public AudioClip deathSound;
    public GameObject respawnEffect;
    public AudioClip respawnSound;

    private float acceleration;
    private Rigidbody2D rigidBody;
    private MovementController movementController;
    private ShipMountController mountController;
    private WeaponController weaponController;
    private GameObject turret;
    private List<Thruster> thrusters; // Contains the thrusters of the ship;
    private Vector2 moveInput;
    private Vector3 previousCameraPosition; // Used to create floaty camera effect.
    private AudioSource source;

    private int prevHealth;
    private float healthToDisplay;
    private PlayerHealth healthUI;
    private bool dead = false;

    private int health; // The amount of health the player currently has.

    // The ship variables.
    private SpriteRenderer shipRenderer;
    private GameObject ship;
    private Ship _ship;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        health = maxHealth;
        movementController = gameObject.GetComponent<MovementController>();
        mountController = gameObject.GetComponent<ShipMountController>();
        weaponController = gameObject.GetComponent<WeaponController>();
        healthToDisplay = maxHealth / healthChunk;
        healthUI = GetComponent<PlayerHealth>();
        healthUI.SetupHealthSprite((int)healthToDisplay);
        prevHealth = health;
        source = GetComponent<AudioSource>();

        // Initial thrusters.
        thrusters = new List<Thruster>();
        foreach (var thrusterObj in mountController.GetThrusterMounts())
            thrusters.Add(thrusterObj.GetShipComponent().gameObject.GetComponentInChildren<Thruster>(true));

        if ((shipRenderer = GetComponentInChildren<SpriteRenderer>()) == null)
            Debug.LogError("Ship contains no Sprite Renderer :(");
        else
        {
            ship = shipRenderer.gameObject;
            _ship = ship.GetComponent<Ship>();
            inventory.AddSlots(_ship.inventorySize);
            foreach(var mount in mountController.GetStorageMounts())
            {
                if (mount.startingComponent != null)
                    inventory.AddSlots((mount.startingComponent as StorageComponent).slotCount);
            }
        }

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
        // Only allow controls if the player is alive.
        if(!dead)
        {
            if (Input.GetKey(KeyCode.W) && thrusters.FirstOrDefault() != null)
            {
                if (!GetThrusterState())
                    SetThrusterState(true);
                movementController.MoveForward();
            }
            else
            {
                if (GetThrusterState())
                    SetThrusterState(false);
                movementController.Decelerate();
            }
            if (Input.GetKey(KeyCode.D))
            {
                movementController.RotateRight();
            }
            if (Input.GetKey(KeyCode.A))
            {
                movementController.RotateLeft();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                gameObject.GetComponent<WeaponController>().menuOpen = !gameObject.GetComponent<WeaponController>().menuOpen;
                inventory.Toggle();
                inventory.infoScreen.Hide();
            }
        }
        else
        {
            if (GetThrusterState())
                SetThrusterState(false);
            movementController.Stop();
        }
    }

    /// <summary>
    /// Update the list of thrusters.
    /// </summary>
    public void UpdateThrusterList()
    {
        if (mountController == null)
            return;
        thrusters = new List<Thruster>();
        foreach (var thrusterObj in mountController.GetThrusterMounts())
        {
            var thruster = thrusterObj.GetShipComponent().gameObject.GetComponentInChildren<Thruster>(true);
            thrusters.Add(thruster);
        }
    }

    /// <summary>
    /// Set the state of all thrusters on the ship.
    /// </summary>
    /// <param name="state"></param>
    private void SetThrusterState(bool state)
    {
        if (thrusters.FirstOrDefault() == null)
            return;
        foreach(var thruster in thrusters)
        {
            thruster.gameObject.SetActive(state);
        }
    }

    /// <summary>
    /// Get the state of the thrusters on the ship.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private bool GetThrusterState()
    {
        if (thrusters.FirstOrDefault() == null)
            return false;
        return thrusters.FirstOrDefault().gameObject.activeSelf;
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

            case "Enemy":
                health -= 20;
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
            if(!dead)
            {
                dead = true;
                this.GetComponentInChildren<SpriteRenderer>().enabled = false;
                mountController.HideMounted();
                weaponController.UpdateDead(dead);
                inventory.UpdateDead(dead);
                movementController.UpdateDead(dead);
                pauseMenu.ResumeGame();
                if (inventory.isOpen)
                    inventory.CloseInventory();
                this.SendMessage("UpdateDead", true);
                healthUI.SetIsDead(true);
                healthUI.RedrawHealthSprites(0, 0);
                deathScreen.Display();
                source.PlayOneShot(deathSound);
                Instantiate(deathExplosion, transform.position, Quaternion.identity);

                StartCoroutine(Respawn());
            }
        }
        return health <= 0;
    }

    /// <summary>
    /// Respawn the player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Respawn()
    {
        // Wait before beginning the respawn animation.
        yield return new WaitForSeconds(RESPAWN_WAIT_TIME);

        Instantiate(respawnEffect, respawnPoint.transform.position, respawnPoint.transform.rotation);
        source.PlayOneShot(respawnSound);
        transform.position = respawnPoint.transform.position;
        ship.transform.rotation = respawnPoint.transform.rotation;
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        deathScreen.Hide();

        // Wait before returning control to the player.
        yield return new WaitForSeconds(RESPAWN_ANIMATION_TIME);

        dead = false;
        health = maxHealth;
        this.GetComponentInChildren<SpriteRenderer>().enabled = true;
        mountController.ShowMounted();
        weaponController.UpdateDead(dead);
        inventory.UpdateDead(dead);
        movementController.UpdateDead(dead);
        this.SendMessage("UpdateDead", false);
        healthUI.SetIsDead(false);
        healthUI.ResetHealth();

    }
}
