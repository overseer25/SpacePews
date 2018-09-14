﻿using UnityEngine;

/// <summary>
/// Model representing a ship.
/// </summary>
public class Ship : MonoBehaviour
{

    public string shipName;

    [Header("Stats")]
    public int health;

    public float maxSpeed;

    public float acceleration;

    public float deceleration;

    public float rotationSpeed;

    [Header("Sounds")]
    public AudioSource engine;

}
