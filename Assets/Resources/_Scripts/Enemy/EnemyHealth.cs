using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public GameObject healthBar;
    private bool dead = false;
    private float length = 0;
    private float max;
    private float current;
    private Ship ship;

    private void Start()
    {
        ship = GetComponentInChildren<Ship>();
        current = ship.health;
        max = ship.maxHealth;
    }

    private void Update()
    {
        current = ship.health;
        AdjustHealthBar(current);
    }

    /// <summary>
    /// Adjust the size of the health bar, based on the health.
    /// </summary>
    /// <param name="health"></param>
    private void AdjustHealthBar(float health)
    {
        current = ((health < 0) ? 0 : health);

        if(max != 0)
            length = current / max;
        else
            Debug.LogError("DivideByZero Error: " + this);

        healthBar.transform.localScale = new Vector2(length, 1.0f);
    }

    /// <summary>
    /// Called by other scripts to inform the UI that the player has died.
    /// </summary>
    /// <param name="isDead">Is the player dead?</param>
    public void SetIsDead(bool isDead)
    {
        dead = isDead;
    }
}
