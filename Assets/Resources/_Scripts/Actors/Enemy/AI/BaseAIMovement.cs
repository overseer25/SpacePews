using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseAIMovement : MonoBehaviour
{
    private float speed = 0;

    private Vector2 direction = Vector2.up;

    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Function to always increase the speed of the ship.
    /// </summary>
    /// <param name="amount">Amount to increase the speed by. Amount is run through absolute value, and so will always be postive.</param>
    /// <returns></returns>
    public float IncreaseSpeed(float amount = 1.0f)
    {
        speed += Math.Abs(amount);
        return speed;
    }

    /// <summary>
    /// Function to always decrease the speed of the ship.
    /// </summary>
    /// <param name="amount">Amount to decrease the speed by. Amount is run through absolute value, and so will always be postive.</param>
    /// <returns></returns>
    public float DecreaseSpeed(float amount = 1.0f)
    {
        speed -= Math.Abs(amount);
        return speed;
    }

    public void MoveForwardOne()
    {
        rigidbody.transform.Translate(direction * Time.deltaTime);
    }

    public void MoveForward(float amount = 1.0f)
    {

    }
}
