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

    // Use this for initialization
    void Start()
    {
        
    }

    /// <summary>
    /// Returns a random damage value based on the damage parameters of the projectile.
    /// Chance for a critical attack.
    /// </summary>
    /// <returns></returns>
    public int GetDamage()
    {
        var result = 0;

        if(random.NextDouble() <= critChance)
        {
            result = (int)Math.Round(random.Next(minDamage, maxDamage) * critMultiplier);
        }
        else
        {
            result = random.Next(minDamage, maxDamage);
        }
        return result;
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
            case "Asteroid":
            case "Mineable":
            case "Mine":
                collided = true;
                gameObject.SetActive(false);
                break;

        }
    }

}
