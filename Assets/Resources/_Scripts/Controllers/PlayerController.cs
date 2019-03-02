using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public Inventory inventory;
    public DeathScreen deathScreen;
    public PauseMenuScript pauseMenu;
    public GameObject itemTransferConfirmWindow;
    public GameObject respawnPoint;
    [Header("Effects/Sounds")]
    public ParticleEffect deathExplosion;
    public AudioClip deathSound;
    public ParticleEffect respawnEffect;
    public AudioClip respawnSound;

    private float acceleration;
    private Rigidbody2D rigidBody;
    private MovementController movementController;
    private ShipMountController mountController;
    private WeaponController weaponController;
    private List<Thruster> thrusters; // Contains the thrusters of the ship;
    private Vector2 moveInput;
    private Vector3 previousCameraPosition; // Used to create floaty camera effect.
    private AudioSource engine;
    private AudioClip engineAudio;

    private int prevHealth;
    private float healthToDisplay;
    private PlayerHealth healthUI;
    private bool dead = false;
    private bool rotatingLeft;
    private bool rotatingRight;
    private bool movingForward;

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
        engine = GetComponent<AudioSource>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();

        // Initial thrusters.
        thrusters = new List<Thruster>();
        foreach (var thrusterObj in mountController.GetThrusterMounts())
            thrusters.Add(thrusterObj.GetShipComponent().gameObject.GetComponentInChildren<Thruster>(true));
        UpdateThrusterList();

        if ((shipRenderer = GetComponentInChildren<SpriteRenderer>()) == null)
            Debug.LogError("Ship contains no Sprite Renderer :(");
        else
        {
            ship = shipRenderer.gameObject;
            _ship = ship.GetComponent<Ship>();
            //inventory.AddSlots(_ship.inventorySize);
            //foreach (var mount in mountController.GetStorageMounts())
            //{
            //    if (mount.startingComponent != null)
            //        inventory.AddSlots((mount.startingComponent as StorageComponent).slotCount);
            //}
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
    /// Movement stuff.
    /// </summary>
    private void FixedUpdate()
    {
        if (movingForward)
        {
            movementController.MoveForward();
            if(thrusters.Count > 0)
                PlayEngineSound();
        }
        else
        {
            movementController.Decelerate();
            if(engine.isPlaying)
                StopEngineSound();
        }

        if (rotatingRight)
            movementController.RotateRight();
        else if (rotatingLeft)
            movementController.RotateLeft();
        Debug.DrawLine(ship.transform.position, (Vector2)ship.transform.position + rigidBody.velocity, Color.green);
        Debug.DrawLine(ship.transform.position, (Vector2)ship.transform.position + ((Vector2)ship.transform.up * 5.0f), Color.white);

    }

    /// <summary>
    /// Deals with inputs.
    /// </summary>
    void Update()
    {
        // Only allow controls if the player is alive.
        if (!dead)
        {
            if ((!Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.W)) || (Input.GetKeyDown(KeyCode.W) && thrusters.FirstOrDefault() != null))
            {
                movingForward = true;
                if (!GetThrusterState())
                    SetThrusterState(true);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                movingForward = false;
                if (GetThrusterState())
                    SetThrusterState(false);
            }

            if (Input.GetKey(KeyCode.D))
            {
                rotatingRight = true;
                rotatingLeft = false;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                rotatingRight = false;
            }
            if (Input.GetKey(KeyCode.A))
            {
                rotatingRight = false;
                rotatingLeft = true;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                rotatingLeft = false;
            }

            // Suicide button.
            if (Input.GetKeyDown(KeyCode.Backspace) && !itemTransferConfirmWindow.activeInHierarchy)
                health = 0;

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
        float acc = 0;
        float dec = 0;
        float maxSpeed = 0;
        float rotSpeed = 0;
        if (mountController == null)
            return;
        thrusters = new List<Thruster>();
        foreach (var thrusterObj in mountController.GetThrusterMounts())
        {
            if(!thrusterObj.IsEmpty())
            {
                var thruster = thrusterObj.GetShipComponent().gameObject.GetComponentInChildren<Thruster>(true);
                thrusters.Add(thruster);
                var comp = ((ThrusterComponent)thrusterObj.GetShipComponent());
                acc += comp.acceleration;
                dec += comp.deceleration;
                maxSpeed += comp.maxSpeed;
                rotSpeed += comp.rotationSpeed;
            }

        }
        movementController.UpdateAcceleration(acc);
        movementController.UpdateMaxSpeed(maxSpeed);
        movementController.UpdateRotationSpeed(rotSpeed);
        movementController.UpdateDeceleration(dec);

        // Set the engine audio to the audio for the first thruster encountered in the list.
        if (thrusters.Count > 0)
        {
            engineAudio = (mountController.GetThrusterMounts().First().GetShipComponent() as ThrusterComponent).engine;
            engine.clip = engineAudio;
            engine.volume = 0.0f;
        }
        engine.Stop();
        SetThrusterState(false);
    }

    /// <summary>
    /// Set the state of all thrusters on the ship.
    /// </summary>
    /// <param name="state"></param>
    private void SetThrusterState(bool state)
    {
        if (thrusters.FirstOrDefault() == null)
            return;
        foreach (var thruster in thrusters)
        {
            thruster.gameObject.SetActive(state);
        }
    }

    /// <summary>
    /// Fade in the engine sound.
    /// </summary>
    private void PlayEngineSound()
    {
        if (!engine.isPlaying)
            engine.Play();
        engine.volume = (engine.volume < 1) ? engine.volume + movementController.GetAcceleration() : 1.0f;
    }

    /// <summary>
    /// Fade out the engine sound, and stop it once it reaches 0.
    /// </summary>
    private void StopEngineSound()
    {
        engine.volume = (engine.volume > 0) ? engine.volume - movementController.GetDeceleration() : 0.0f;
        if (engine.volume <= 0.0f)
            engine.Stop();
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
                Vector2 direction = Vector2.zero; // Direction of the collision, with the magnitude applied being the speed of the player.
                // If the player isn't moving, we need to move them away, as they would be able to clip into the immovable otherwise.
                if (rigidBody.velocity == Vector2.zero)
                {
                    direction = (gameObject.transform.position - collider.gameObject.transform.position) * 20;
                }
                // The difference here is the player has a velocity, so we will propel them away from the immovable using their velocity magnitude.
                else
                {
                    if (rigidBody.velocity.magnitude > 10)
                    {
                        health -= (int)System.Math.Floor(rigidBody.velocity.magnitude / 3);
                    }
                    direction = (gameObject.transform.position - collider.gameObject.transform.position).normalized * movementController.GetMaxSpeed() * 10f;
                }
                movementController.Stop();
                movementController.MoveDirection(direction);
                break;

            case "Enemy":
                health -= 5;
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
        if (health <= 0)
        {
            if (!dead)
            {
                dead = true;
                this.GetComponentInChildren<SpriteRenderer>().enabled = false;
                mountController.HideMounted();
                weaponController.UpdateDead(dead);
                inventory.UpdateDead(dead);
                movementController.UpdateDead(dead);
                pauseMenu.UpdateDead(dead);
                pauseMenu.ResumeGame();
                this.SendMessage("UpdateDead", true);
                healthUI.SetIsDead(true);
                healthUI.RedrawHealthSprites(0, 0);
                deathScreen.Display();
                engine.Stop();

                movingForward = false;
                rotatingLeft = false;
                rotatingRight = false;
                if (GetThrusterState())
                    SetThrusterState(false);

                //spawn explosion effect.
                ParticleManager.PlayParticle(deathExplosion, gameObject);

                StartCoroutine(Respawn());
            }
        }
        return health <= 0;
    }

    /// <summary>
    /// Gives the result if the player is dead or not.
    /// </summary>
    /// <returns>True if player is dead, false otherwise.</returns>
    public bool IsDead()
    {
        return dead;
    }

    /// <summary>
    /// Respawn the player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Respawn()
    {
        // Wait before beginning the respawn animation.
        yield return new WaitForSeconds(RESPAWN_WAIT_TIME);

        // Spawn respawn effect.
        ParticleManager.PlayParticle(respawnEffect, respawnPoint);

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
        pauseMenu.UpdateDead(dead);
        this.SendMessage("UpdateDead", false);
        healthUI.SetIsDead(false);
        healthUI.ResetHealth();

    }
}
