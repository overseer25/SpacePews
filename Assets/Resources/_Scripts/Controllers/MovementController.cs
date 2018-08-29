using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {

    [Header("Movement")]
    public float speed = 1.0f;
    public float maxSpeed = 5.0f;
    public float accelerationRate = 2.5f;
    public float decelerationRate = 0.005f;
    public float rotationSpeed = 50.0f;
    [Header("Camera")]
    public Camera playerCamera;

    private Vector3 velocity;
    private Rigidbody2D rigidBody;
    private float desiredRotation;

    // The ship variables.
    private SpriteRenderer shipRenderer;
    private GameObject ship;

    void Start()
    {
        velocity = Vector3.zero;
        rigidBody = GetComponent<Rigidbody2D>();
        if ((shipRenderer = GetComponentInChildren<SpriteRenderer>()) == null)
            Debug.LogError("Ship contains no Sprite Renderer :(");
        else
        {
            ship = shipRenderer.gameObject;
        }

        
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        playerCamera.transform.position = Vector3.Slerp(playerCamera.transform.position, 
                                        new Vector3(ship.transform.position.x, ship.transform.position.y, playerCamera.transform.position.z), rigidBody.velocity.magnitude * Time.deltaTime);
	}

    /// <summary>
    /// Move the ship forward.
    /// </summary>
    public void MoveForward()
    {
        rigidBody.AddForce(ship.transform.up * accelerationRate * speed * Time.deltaTime);
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);
    }
    /// <summary>
    /// Rotates the ship to the left.
    /// </summary>
    public void RotateLeft()
    {
        if(ship == null) { return; }

        desiredRotation += rotationSpeed * Time.deltaTime;
        var rotationQuaternion = Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, desiredRotation);
        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, rotationQuaternion, rotationSpeed * Time.deltaTime);
        rigidBody.velocity = ship.transform.up * rigidBody.velocity.magnitude;
    }

    /// <summary>
    /// Rotates the ship to the right.
    /// </summary>
    public void RotateRight()
    {
        if (ship == null) { return; }

        desiredRotation -= rotationSpeed * Time.deltaTime;
        var rotationQuaternion = Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, desiredRotation);
        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, rotationQuaternion, rotationSpeed * Time.deltaTime);
        rigidBody.velocity = ship.transform.up * rigidBody.velocity.magnitude;
    }

    /// <summary>
    /// Slow down the ship when no button is being pressed.
    /// </summary>
    public void Decelerate()
    {
        if(rigidBody.velocity.magnitude > 0)
            rigidBody.velocity *= (1 - decelerationRate);
    }
}
