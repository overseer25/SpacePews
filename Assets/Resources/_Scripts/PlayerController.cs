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

    public AudioSource engine; // Engine sound
    public bool inertialDamp = true; // Are inertial dampeners on?
    public float maxSpeed = 3.0f; // Max speed of the player.

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
        Vector2 temp = moveInput;
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
            rigidBody.velocity = rigidBody.velocity * 0.95f;
        }
        else
        {
            rigidBody.AddForce(moveInput * acceleration);
            rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, maxSpeed);
        }

        /** Rotating the ship **/
        if (Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f); } // North
        else if (Input.GetKey("w") && Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f); } // Northwest
        else if (Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && Input.GetKey("d")) { transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f); } // Northeast
        else if (!Input.GetKey("w") && !Input.GetKey("a") && Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f); } // South
        else if (!Input.GetKey("w") && Input.GetKey("a") && Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Euler(0.0f, 0.0f, 135.0f); } // Southwest
        else if (!Input.GetKey("w") && !Input.GetKey("a") && Input.GetKey("s") && Input.GetKey("d")) { transform.rotation = Quaternion.Euler(0.0f, 0.0f, -135.0f); } // Southeast
        else if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && Input.GetKey("d")) { transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f); } // East
        else if (!Input.GetKey("w") && Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d")) { transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f); } // West

        /** Code below is used to rotate the turret with the mouse **/

        // Distance from camera to object.  We need this to get the proper calculation.
        float camDis = Camera.main.transform.position.y - turret.transform.position.y;
        // Get the mouse position in world space. Using camDis for the Z axis.
        Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDis));
        float AngleRad = Mathf.Atan2(mouse.y - turret.transform.position.y, mouse.x - turret.transform.position.x);
        float angle = (180 / Mathf.PI) * AngleRad;
        turret.transform.rotation = Quaternion.Euler(0, 0, angle - 90.0f); // turret follows mouse

    }

}
