using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object in the game world that can be shot and destroyed.
/// </summary>
public class DestructibleObject : Lootable
{
    public int health;
    public SpriteRenderer healthBar;

    private Coroutine hideHealthBar;

    private void Start()
    {
        healthBar.enabled = false;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.size = new Vector2(health / 100f, healthBar.size.y);
        if (hideHealthBar != null)
            StopCoroutine(hideHealthBar);
        hideHealthBar = StartCoroutine(HideHealthBar());
    }

    private IEnumerator HideHealthBar()
    {
        yield return new WaitForSeconds(5.0f);
        healthBar.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "PlayerProjectile":
                var projectile = collision.gameObject.GetComponent<Projectile>();
                health -= projectile.GetDamage();
                if (health <= 0)
                {
                    PlayDestroyEffect();
                    SpawnLoot();
                    Destroy(gameObject);
                }
                healthBar.enabled = true;
                UpdateHealthBar();
                break;
        }
    }
}
