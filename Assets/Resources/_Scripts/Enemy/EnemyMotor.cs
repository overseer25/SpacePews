using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMotor : MonoBehaviour
{
    private Vector2 velocity;
    private Rigidbody2D rigidBody;
    private float desiredRotation;
    private float acceleration;
    private float maxSpeed;

    // The ship variables.
    private SpriteRenderer shipRenderer;
    private GameObject ship;
    private Ship _ship;

    void Start()
    {
        velocity = Vector2.zero;
        rigidBody = GetComponentInChildren<Rigidbody2D>();
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
        if (velocity.magnitude <= 0)
        {
            velocity = Vector2.zero;
        }
        if (velocity != Vector2.zero)
        {
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            rigidBody.MovePosition((Vector2)ship.transform.position + velocity * Time.fixedDeltaTime);
        }
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);
    }

    /// <summary>
    /// Move the ship forward.
    /// </summary>
    public void MoveForward(float accelerationAmount, float newMaxSpeed)
    {
        acceleration = accelerationAmount;
        maxSpeed = newMaxSpeed;
        if (!_ship.engine.isPlaying)
        {
            _ship.engine.Play();
        }
        velocity += (Vector2)ship.transform.up * acceleration * 10.0f * Time.deltaTime;
    }

    /// <summary>
    /// Shifts the enemy ship to the left or the right some amount upon impact with player.
    /// </summary>
    /// <param name="amount">How big of a shift to make</param>
    /// <param name="dir">If dir is 1, shift right, if dir is -1 shift left</param>
    public void Shift(float amount, int dir)
    {
        if(dir == 1)
        {
            velocity += (Vector2)ship.transform.right * amount * Time.deltaTime;
        }
        else if(dir == -1)
        {
            velocity -= (Vector2)ship.transform.right * amount * Time.deltaTime;
        }
    }

    /// <summary>
    /// This method is called when a ship impacts with something else. The ship is
    /// set off an a drifting direction while spinning until it has recovered.
    /// </summary>
    /// <param name="impactAmount">How much a shift will happen.</param>
    /// <param name="impactDir">Which direction the shift will occur and the stunned spin will do.</param>
    public void ShipImpact(float impactAmount, int impactDir, Vector2 driftDir)
    {
        SetDriftDirection(driftDir);
    }

    private void SetDriftDirection(Vector2 dir)
    {
        velocity = velocity.magnitude * dir;
    }

    /// <summary>
    /// Rotates the ship to the left.
    /// </summary>
    public void RotateLeft(float rotationSpeedModifier = 1, bool adjustVelocity = true)
    {
        if (ship == null) { return; }

        desiredRotation += _ship.rotationSpeed * Time.deltaTime * rotationSpeedModifier;
        var rotationQuaternion = Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, desiredRotation);
        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, rotationQuaternion, _ship.rotationSpeed * Time.deltaTime);
        if (adjustVelocity)
        {
            velocity = ship.transform.up * velocity.magnitude;
        }
    }

    /// <summary>
    /// Rotates the ship to the right.
    /// </summary>
    public void RotateRight(float rotationSpeedModifier = 1, bool adjustVelocity = true)
    {
        if (ship == null) { return; }

        desiredRotation -= _ship.rotationSpeed * Time.deltaTime * rotationSpeedModifier;
        var rotationQuaternion = Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, desiredRotation);
        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, rotationQuaternion, _ship.rotationSpeed * Time.deltaTime);
        if (adjustVelocity)
        {
            velocity = ship.transform.up * velocity.magnitude;
        }
    }

    /// <summary>
    /// Slow down the ship when no button is being pressed.
    /// </summary>
    public void Decelerate(float decelerationAmount)
    {
        if (_ship.engine.isPlaying)
        {
            _ship.engine.Stop();
        }
        if (velocity.magnitude > 0)
        {
            velocity -= velocity * decelerationAmount * Time.deltaTime;
        }
    }
}
