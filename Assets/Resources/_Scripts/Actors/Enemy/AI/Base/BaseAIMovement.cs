using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base movement class for all AI. Contains basic methods that move the associated
/// rigidbody of the AI Actor in simple motion using translate and rotate. Contains
/// a speed variable that can be used when calling the methods to determine how 
/// far the body should move or rotate.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BaseAIMovement : MonoBehaviour
{
    // DEBUG
    public UnityEngine.UI.Text heading;
    // DEBUG
    public UnityEngine.UI.Text currentSpeed;
    public float Speed = 0;
    public float RotationSpeed = 0;
    public float AvoidanceDistance = 300f;

    protected Rigidbody2D rigidbody;
    protected Collider2D collider;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
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

    /// <summary>
    /// Function to determine which direction a body should rotate towards a target
    /// vector. True for clockwise, false for counterclockwise.
    /// </summary>
    /// <param name="currentVec">The current heading of the body.</param>
    /// <param name="targetVec">What vector point we are trying to look at.</param>
    /// <returns>Returns false for clockwise rotation, true for counterclockwise. If the unsigned angle between the two vectors is 180, it will pick clockwise or counterclockwise arbitrarily.</returns>
    public bool GetRotationDirection(Vector2 currentVec, Vector2 targetVec)
    {
        return Vector2.SignedAngle(currentVec, targetVec) < 0;
    }

    public Vector2 GetAvoidancePoint(Vector2 target, Vector2 currentDir, Vector2 goal)
    {
        RaycastHit2D hit = Physics2D.BoxCast(rigidbody.transform.position, this.transform.localScale * 0.4f, Vector2.Angle(Vector2.up, rigidbody.transform.up.normalized), currentDir, AvoidanceDistance); // TODO: Add layer mask
        if (hit.collider != null)
        {
            Debug.DrawLine(rigidbody.transform.position, hit.point, Color.red);
            Vector2 leftTestPoint = Vector2.Perpendicular(currentDir) * 10 + hit.point;
            Vector2 rightTestPoint = -Vector2.Perpendicular(currentDir) * 10 + hit.point;
            Vector2 leftDir = leftTestPoint - (Vector2)rigidbody.transform.position;
            Vector2 rightDir = rightTestPoint - (Vector2)rigidbody.transform.position;
            leftDir = leftDir.normalized;
            rightDir = rightDir.normalized;
            Debug.DrawLine(rigidbody.transform.position, leftTestPoint, Color.blue);
            Debug.DrawLine(rigidbody.transform.position, rightTestPoint, Color.green);
            Debug.DrawRay(rigidbody.transform.position, leftDir, Color.cyan);
            Debug.DrawRay(rigidbody.transform.position, rightDir, Color.yellow);
            Vector2 leftAttempt = GetAvoidancePoint(leftTestPoint, leftDir, goal);
            Vector2 rightAttempt = GetAvoidancePoint(rightTestPoint, rightDir, goal);
            return Vector2.Distance(leftAttempt, goal) < Vector2.Distance(rightAttempt, goal) ? leftAttempt : rightAttempt;
        }
        else
        {
            return target;
        }
    }

    // DEBUG
    protected virtual void Update()
    {
        heading.text = GetCurrentForwardDirection().ToString();
        currentSpeed.text = Speed.ToString();
    }
}
