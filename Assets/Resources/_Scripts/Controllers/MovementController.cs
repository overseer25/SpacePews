using System.Linq;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    private Rigidbody2D rigidBody;
    private float desiredRotation;
    private ShipMountController mountController;
    private float acceleration;
    private float deceleration;
    private float maxSpeed;
    private float rotationSpeed;
    private bool dead = false;

    // The ship variables.
    private SpriteRenderer shipRenderer;
    private GameObject ship;
    private Ship _ship;

    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        mountController = gameObject.GetComponent<ShipMountController>();
        if ((shipRenderer = GetComponentInChildren<SpriteRenderer>()) == null)
            Debug.LogError("Ship contains no Sprite Renderer D:");
        else
        {
            ship = shipRenderer.gameObject;
            _ship = ship.GetComponent<Ship>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position,
                                        new Vector3(ship.transform.position.x, ship.transform.position.y, Camera.main.transform.position.z), rigidBody.velocity.magnitude * Time.deltaTime);
    }

    /// <summary>
    /// Move the ship forward.
    /// </summary>
    public void MoveForward()
    {
        rigidBody.AddForce(ship.transform.up * acceleration * Time.deltaTime, ForceMode2D.Impulse);
        rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, maxSpeed);
    }

    /// <summary>
    /// Rotates the ship to the left.
    /// </summary>
    public void RotateLeft()
    {
        if (ship == null) { return; }
        var currentRotation = ship.transform.rotation.eulerAngles.z;
        desiredRotation += rotationSpeed * Time.deltaTime;
        var rotationQuaternion = Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, desiredRotation);
        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, rotationQuaternion, rotationSpeed * Time.deltaTime);
        rigidBody.velocity = rigidBody.velocity.Rotate(desiredRotation - currentRotation);
    }

    /// <summary>
    /// Rotates the ship to the right.
    /// </summary>
    public void RotateRight()
    {
        if (ship == null) { return; }
        var currentRotation = ship.transform.rotation.eulerAngles.z;
        desiredRotation -= rotationSpeed * Time.deltaTime;
        var rotationQuaternion = Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, desiredRotation);
        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, rotationQuaternion, rotationSpeed * Time.deltaTime);
        rigidBody.velocity = rigidBody.velocity.Rotate(desiredRotation - currentRotation);
    }

    /// <summary>
    /// Slow down the ship when no button is being pressed.
    /// </summary>
    public void Decelerate()
    {
        if (rigidBody.velocity.magnitude > 0)
            rigidBody.velocity *= (1 - deceleration);
    }

    /// <summary>
    /// Add force in the provided direction.
    /// </summary>
    /// <param name="direction"></param>
    public void MoveDirection(Vector2 direction)
    {
        rigidBody.AddForce(direction);
    }

    /// <summary>
    /// Stop the movement completely.
    /// </summary>
    public void Stop()
    {
        rigidBody.velocity = Vector2.zero;
    }

    /// <summary>
    /// Update the acceleration of the player ship.
    /// </summary>
    /// <param name="acceleration"></param>
    public void UpdateAcceleration(float acceleration)
    {
        this.acceleration = acceleration;
    }

    /// <summary>
    /// Get the acceleration of the movement controller.
    /// </summary>
    /// <returns></returns>
    public float GetAcceleration()
    {
        return acceleration;
    }

    /// <summary>
    /// Update the deceleration of the player ship.
    /// </summary>
    /// <param name="deceleration"></param>
    public void UpdateDeceleration(float deceleration)
    {
        this.deceleration = deceleration;
    }

    /// <summary>
    /// Get the deceleration of the movement controller.
    /// </summary>
    /// <returns></returns>
    public float GetDeceleration()
    {
        return deceleration;
    }

    /// <summary>
    /// Update the max speed of the player ship.
    /// </summary>
    /// <param name="maxSpeed"></param>
    public void UpdateMaxSpeed(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }

    /// <summary>
    /// Get the max speed of the movement controller.
    /// </summary>
    /// <returns></returns>
    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    /// <summary>
    /// Update the rotation speed of the player ship.
    /// </summary>
    /// <param name="rotSpeed"></param>
    public void UpdateRotationSpeed(float rotSpeed)
    {
        rotationSpeed = rotSpeed;
    }

    /// <summary>
    /// Get the rotation speed of the movement controller.
    /// </summary>
    /// <returns></returns>
    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }

    /// <summary>
    /// Update the death state of the player.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        dead = isDead;
        desiredRotation = 0;
    }
}
