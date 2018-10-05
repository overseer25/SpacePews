using UnityEngine;

/// <summary>
/// Model representing a ship.
/// </summary>
public class Ship : MonoBehaviour
{

    public string shipName;

    [Header("Stats")]
    public int health;

    public float rotationSpeed;

    public int inventorySize;

    [Header("Sounds")]
    public AudioSource engine;

}
