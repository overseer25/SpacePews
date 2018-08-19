using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    /// <summary>
    /// Dictates the shot speed of the projectile.
    /// </summary>
    [Header("Movement")]
    public float speed;

    /// <summary>
    /// Used if the projectile gets faster as it exists.
    /// </summary>
    public float acceleration;

    /// <summary>
    /// Minimum damage.
    /// </summary>
    [Header("Damage")]
    public int minDamage;

    /// <summary>
    /// Maximum damage.
    /// </summary>
    public int maxDamage;

    /// <summary>
    /// The current computed damage of the projectile, determined on collision.
    /// </summary>
    public int Damage { get; internal set; }

    /// <summary>
    /// Critical chance damage;
    /// </summary>
    [Header("Critical")]
    public float critMultiplier;

    /// <summary>
    /// Critical chance percentage.
    /// </summary>
    public float critChance;

    /// <summary>
    /// Firing sound.
    /// </summary>
    [Header("Audio")]
    public AudioClip fireSound;

    /// <summary>
    /// Sound that the projectile makes when colliding with something.
    /// </summary>
    public AudioClip collisionSound;

    /// <summary>
    /// The lifetime of the projectile.
    /// </summary>
    [Header("Other")]
    public int lifetime;

    /// <summary>
    /// How quickly the projectile can be fired.
    /// </summary>
    public float fireRate;

    // Explosion sprite.
    public GameObject explosion;

    private Rigidbody2D rigidBody;
    private bool collided = false; // Check to see if a collision sound should be played on deactivation.
    private static System.Random random; // Static so all projectiles pull from same randomness and don't end up generating the same number with similar seeds

    /// <summary>
    /// Returns a random damage value based on the damage parameters of the projectile.
    /// Chance for a critical attack.
    /// </summary>
    /// <returns></returns>
    private void ComputeDamage()
    {
        if(random.NextDouble() <= critChance)
        {
            Damage = (int)Math.Round(random.Next(minDamage, maxDamage) * critMultiplier);
        }
        else
        {
            Damage = random.Next(minDamage, maxDamage);
        }
    }

    /// <summary>
    /// Called when the projectile is active in the hierarchy.
    /// </summary>
    void OnEnable()
    {
        random = new System.Random();
        random.Next();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.up * speed;
        Damage = 0;

        // Start the clock to disabling again.
        StartCoroutine(RemoveAfterSeconds(lifetime));
    }

    /// <summary>
    /// When the projectile collides with an object, play the explosion animation
    /// </summary>
    void OnDisable()
    {
        if (collided)
        {
            Explosion exp = Instantiate(explosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
            exp.Initialize(collisionSound);

            // Current damage is set on collisions with entities that can take damage.
            if(Damage != 0)
            {
                var popUptext = PopUpTextPool.current.GetPooledObject();
                popUptext.GetComponent<PopUpText>().Initialize(gameObject, Damage.ToString(), ItemColorSelector.Tier1);
            }
        }

    }

    /// <summary>
    /// Disables the projectile after a set time has passed.
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator RemoveAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        collided = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles collisions.
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // If friendly fire, ignore
        if (collision.gameObject.tag == "Player" && gameObject.tag == "PlayerProjectile") { return; }
        if (collision.gameObject.tag == "Enemy" && gameObject.tag == "EnemyProjectile") { return; }

        switch (collision.gameObject.tag)
        {
            case "Player":
            case "Enemy":
                collided = true;
                ComputeDamage();
                gameObject.SetActive(false);
                break;
            case "Asteroid":
            case "Mineable":
            case "Mine":
                collided = true;
                gameObject.SetActive(false);
                break;

        }
    }

}
