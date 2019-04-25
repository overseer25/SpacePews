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
    private PlayerController pController;

    // The ship variables.
    private SpriteRenderer shipRenderer;
    private GameObject ship;

    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        mountController = GetComponent<ShipMountController>();
        pController = GetComponent<PlayerController>();
        if ((shipRenderer = GetComponentInChildren<SpriteRenderer>()) == null)
            Debug.LogError("Ship contains no Sprite Renderer D:");
        else
        {
            ship = shipRenderer.gameObject;
        }
    }

    /// <summary>
    /// Move the ship forward.
    /// </summary>
    public void MoveForward()
    {
        rigidBody.AddForce(ship.transform.up * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);

		if (rigidBody.velocity.magnitude > maxSpeed)
			Decelerate();
    }

    /// <summary>
    /// Rotates the ship to the left.
    /// </summary>
    public void RotateLeft()
    {
        if (ship == null) { return; }
		var currentRotation = rigidBody.rotation;
		desiredRotation = Mathf.Lerp(currentRotation, currentRotation + rotationSpeed, Time.deltaTime);
		rigidBody.MoveRotation(desiredRotation);
		rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, rigidBody.velocity.Rotate(desiredRotation - currentRotation), Time.deltaTime * rotationSpeed);
		rigidBody.angularVelocity = 0;
    }

    /// <summary>
    /// Rotates the ship to the right.
    /// </summary>
    public void RotateRight()
    {
        if (ship == null) { return; }
        var currentRotation = rigidBody.rotation;
		desiredRotation = Mathf.Lerp(currentRotation, currentRotation - rotationSpeed, Time.deltaTime);
		rigidBody.MoveRotation(desiredRotation);
		rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, rigidBody.velocity.Rotate(desiredRotation - currentRotation), Time.deltaTime * rotationSpeed);
		rigidBody.angularVelocity = 0;
	}

    /// <summary>
    /// Slow down the ship when no button is being pressed.
    /// </summary>
    public void Decelerate()
    {
        if (rigidBody.velocity.magnitude > 0)
            rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, Vector2.zero, Time.fixedDeltaTime * deceleration);
    }

    /// <summary>
    /// Add force in the provided direction.
    /// </summary>
    /// <param name="direction">The direction to propel the ship.</param>
	/// <param name="clampSpeed">Whether or not to clamp the speed to the max speed of the ship.</param>
    public void MoveDirection(Vector2 direction, bool clampSpeed = false)
    {
        rigidBody.velocity += direction;

        if (clampSpeed && rigidBody.velocity.magnitude > maxSpeed)
            Decelerate();
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
	/// Get the magnitude of the ship's movement vector.
	/// </summary>
	/// <returns></returns>
	public float GetVelocity()
	{
		return rigidBody.velocity.magnitude;
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
	/// Get the ship the movement controller moves.
	/// </summary>
	/// <returns></returns>
	public GameObject GetShip()
	{
		return ship;
	}

    /// <summary>
    /// Update the death state of the player.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        dead = isDead;
        desiredRotation = 0;
		if (dead)
			Stop();
    }
}
