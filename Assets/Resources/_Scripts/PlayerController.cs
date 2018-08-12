using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Converts inputs given by the player into actions and movements of the player character.
/// </summary>
public class PlayerController : MonoBehaviour {

    private float acceleration;
    private Rigidbody2D rigidBody;
    private GameObject turret;
    private GameObject thruster; // Contains the thruster sprite that plays when moving.
    private Vector2 moveInput;
    private bool playingEngine = false;
    private Vector3 previousCameraPosition; // Used to create floaty camera effect.
    public int health = 5; // The amount of health the player currently has.
    public int maxHealth = 5; // Max health the player can currently have.
    public int currency = 5; // Amount of currency the player currently has.
    public float turnSpeed;

    public AudioSource engine; // Engine sound
    public bool inertialDamp = true; // Are inertial dampeners on?
    public float maxSpeed = 3.0f; // Max speed of the player.
    public float afterburnerMod = 1.0f; // Multiplier that increases speed when "Boost" key is pressed.

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {

        rigidBody = GetComponent<Rigidbody2D>();
        turret = transform.Find("turret").gameObject;
        thruster = transform.Find("thruster").gameObject;
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z); // Center on the player character.

    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {

        // Gets the movement vector given by WASD.
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // If moving
        if (moveInput != Vector2.zero)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = true;
            if(acceleration < maxSpeed)
                acceleration += 0.01f;
            if(!playingEngine)
            {
                engine.Play(); // Play engine sound while moving
                playingEngine = true;
            }
            engine.volume = 1.0f; // Max ship volume when accelerating
            if (engine.pitch < 1.0f) { engine.pitch += 0.1f; } // Increase pitch to signify accelerating

        }
        // Otherwise, if player isn't pressing WASD.
        else
        {
            thruster.GetComponent<SpriteRenderer>().enabled = false; // Disable thruster graphic

            // Decelerate
            if(acceleration > 0)
                acceleration -= 0.003f;

            engine.volume -= 0.005f; // Quiet the engine down as ship slows
            if (engine.pitch > 0.0f) { engine.pitch -= 0.005f; } // Reduce pitch when slowing down to represent engine slowing
            if(engine.volume <= 0)
            {
                engine.Stop(); // Stop the engine sound when not moving
                playingEngine = false;
            }
        }

        // Toggle inertial dampeners
        if (Input.GetKeyDown("f"))
        {
            inertialDamp = !inertialDamp;
        }

    }

    void FixedUpdate()
    {
        // Floaty camera movement
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z), rigidBody.velocity.magnitude * Time.deltaTime);

        /** Move the ship **/
        if (moveInput == Vector2.zero && inertialDamp)
        {
            rigidBody.velocity *= 0.95f;
        }
        else
        {
            // If boosting
            if(Input.GetButton("Boost"))
            {
                rigidBody.AddForce(moveInput * acceleration * afterburnerMod);
                rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, maxSpeed * afterburnerMod);
            }
            else
            {
                // Return to original max speed after using afterburner
                if(rigidBody.velocity.magnitude > maxSpeed)
                {
                    rigidBody.velocity *= 0.99f;
                }
                // Otherwise, move as normal.
                else
                {
                    rigidBody.AddForce(moveInput * acceleration);
                    rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, maxSpeed);
                }
                
            }
        }

        /** Rotating the ship **/
        if (Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, 0.0f), turnSpeed * Time.deltaTime); } // North
        else if (Input.GetKey("w") && Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, 45.0f), turnSpeed * Time.deltaTime); } // Northwest
        else if (Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && Input.GetKey("d")) { transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, -45.0f), turnSpeed * Time.deltaTime); } // Northeast
        else if (!Input.GetKey("w") && !Input.GetKey("a") && Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, 180.0f), turnSpeed * Time.deltaTime); } // South
        else if (!Input.GetKey("w") && Input.GetKey("a") && Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, 135.0f), turnSpeed * Time.deltaTime); } // Southwest
        else if (!Input.GetKey("w") && !Input.GetKey("a") && Input.GetKey("s") && Input.GetKey("d")) { transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, -135.0f), turnSpeed * Time.deltaTime); } // Southeast
        else if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && Input.GetKey("d")) { transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, -90.0f), turnSpeed * Time.deltaTime); } // East
        else if (!Input.GetKey("w") && Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, 90.0f), turnSpeed * Time.deltaTime); } // West


    }

    /// <summary>
    /// Deals with all collisions with the player character
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {

        switch(collider.gameObject.tag)
        {
            case "Item":
                Item item = collider.gameObject.GetComponent<Item>();
                Inventory inventory = GameObject.Find("InventoryHUD").GetComponent<Inventory>();
                Debug.Log("IsEmpty: " + inventory.IsEmptySlot() + ", Contains: " + inventory.ContainsItem(item));
                // Only add the item to the player's inventory list if there is an empty slot for it, or a slot contains the item already.
                if (inventory.IsEmptySlot() || inventory.ContainsItem(item))
                {
                    GameObject.Find("InventoryHUD").GetComponent<Inventory>().AddItem(item); // Add the item to the player inventory.
                    collider.gameObject.GetComponent<Item>().CreateCollectItemSprite();
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
                        gameObject.GetComponent<PlayerController>().health--;
                    }
                    else if (rigidBody.velocity.magnitude >= 5 && rigidBody.velocity.magnitude < 10)
                    {

                        gameObject.GetComponent<PlayerController>().health -= 2;
                    }
                    else if (rigidBody.velocity.magnitude >= 10)
                    {
                        gameObject.GetComponent<PlayerController>().health -= 3;
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
                health -= collider.gameObject.GetComponent<Projectile>().damage;
                break;
        
        }

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

}
