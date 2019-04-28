using UnityEngine;

/// <summary>
/// Model representing a ship.
/// </summary>
public class Ship : MonoBehaviour
{

    public string shipName;

    [Header("Stats")]
    public int maxHealth;
    public int health;

    public int inventorySize;

    [Header("Turret")]
    public GameObject turret;

}
