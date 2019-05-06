using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseAIMovement : MonoBehaviour
{
    public UnityEngine.UI.Text heading;
    public float Speed { get; set; } = 0;

    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Function to always increase the speed of the ship.
    /// Returns the new speed.
    /// </summary>
    /// <param name="amount">Amount to increase the speed by. Amount is run through absolute value, and so will always be postive.</param>
    /// <returns>New speed after the increase occurs.</returns>
    public float IncreaseSpeed(float amount = 1.0f)
    {
        Speed += Math.Abs(amount);
        return Speed;
    }

    /// <summary>
    /// Function to always decrease the speed of the ship.
    /// Returns the new speed.
    /// </summary>
    /// <param name="amount">Amount to decrease the speed by. Amount is run through absolute value, and so will always be postive.</param>
    /// <returns>Speed after the decrease occurs.</returns>
    public float DecreaseSpeed(float amount = 1.0f)
    {
        Speed -= Math.Abs(amount);
        return Speed;
    }

    /// <summary>
    /// Move body one unit forward in current direction, as affected by deltatime.
    /// </summary>
    public void MoveForwardOne()
    {
        rigidbody.transform.Translate(rigidbody.transform.up * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Move body forward in the current direction, multiplied by the amount given. 
    /// Also affected by deltatime.
    /// </summary>
    /// <param name="amount">Multiplied to the direction. So amount = 2 is twice the distance forward.</param>
    public void MoveForward(float amount = 1)
    {
        amount = Math.Abs(amount);
        Debug.DrawRay(rigidbody.transform.position, rigidbody.transform.up.normalized, Color.red, 2);
        rigidbody.transform.Translate(rigidbody.transform.up.normalized * amount * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Move body one unit right relative to the current direction, as affected by deltatime.
    /// </summary>
    public void MoveRightOne()
    {
        rigidbody.transform.Translate(rigidbody.transform.right * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Move body forward in the current direction, multiplied by the amount given. 
    /// Also affected by deltatime.
    /// </summary>
    /// <param name="amount">Multiplied to the direction. So amount = 2 is twice the distance forward.</param>
    public void MoveRight(float amount = 1)
    {
        amount = Math.Abs(amount);
        Debug.DrawRay(rigidbody.transform.position, rigidbody.transform.right.normalized, Color.green, 2);
        rigidbody.transform.Translate(rigidbody.transform.right * amount * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Move body one unit right relative to the current direction, as affected by deltatime.
    /// </summary>
    public void MoveLeftOne()
    {
        rigidbody.transform.Translate(-rigidbody.transform.right * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Move body forward in the current direction, multiplied by the amount given. 
    /// Also affected by deltatime.
    /// </summary>
    /// <param name="amount">Multiplied to the direction. So amount = 2 is twice the distance forward.</param>
    public void MoveLeft(float amount = 1)
    {
        amount = Math.Abs(amount);
        Debug.DrawRay(rigidbody.transform.position, -rigidbody.transform.right.normalized, Color.blue, 2);
        rigidbody.transform.Translate(-rigidbody.transform.right * amount * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Move body one unit right relative to the current direction, as affected by deltatime.
    /// </summary>
    public void MoveBackwardOne()
    {
        rigidbody.transform.Translate(-rigidbody.transform.up * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Move body forward in the current direction, multiplied by the amount given. 
    /// Also affected by deltatime.
    /// </summary>
    /// <param name="amount">Multiplied to the direction. So amount = 2 is twice the distance forward.</param>
    public void MoveBackward(float amount = 1)
    {
        amount = Math.Abs(amount);
        Debug.DrawRay(rigidbody.transform.position, -rigidbody.transform.up.normalized, Color.yellow, 2);
        rigidbody.transform.Translate(-rigidbody.transform.up * amount * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Returns the Vector2 of the direction the ship is currently pointing.
    /// </summary>
    /// <returns>Ships forward facing vector.</returns>
    public Vector2 GetCurrentForwardDirection()
    {
        return rigidbody.transform.up.normalized;
    }

    /// <summary>
    /// Rotates the body clockwise (or right) by some amount, affected by deltatime.
    /// </summary>
    /// <param name="amount">How much to rotate the body by.</param>
    public void RotateClockwise(float amount = 1)
    {
        rigidbody.transform.Rotate(0, 0, -Math.Abs(amount) * Time.deltaTime);
    }

    /// <summary>
    /// Rotates the body counter clockwise (or left) by some amount, affected by deltatime.
    /// </summary>
    /// <param name="amount">How much to rotate the body by.</param>
    public void RotateCounterClockwise(float amount = 1)
    {
        rigidbody.transform.Rotate(0, 0, Math.Abs(amount) * Time.deltaTime);
    }

    private void Update()
    {
        heading.text = GetCurrentForwardDirection().ToString();
    }
}
