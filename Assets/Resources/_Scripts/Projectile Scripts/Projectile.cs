using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    /// <summary>
    /// Dictates the shot speed of the projectile.
    /// </summary>
    private float speed = 0.0f;

    /// <summary>
    /// The current computed damage of the projectile, determined on collision.
    /// </summary>
    private int damage = 0;

    /// <summary>
    /// Sound that the projectile makes when colliding with something.
    /// </summary>
    [Header("Sound")]
    [SerializeField]
    private AudioClip fireSound;

    /// <summary>
    /// The lifetime of the projectile.
    /// </summary>
    [Header("Other")]
    [SerializeField]
    private ParticleEffect destroyEffect;
    [SerializeField]
    private int lifetime;
    [SerializeField]
    private int homingDistance;
    [SerializeField]
    private float rotationSpeed;

    private Rigidbody2D rigidBody;
    private bool collided = false; // Check to see if a collision sound should be played on deactivation.
    private static System.Random random; // Static so all projectiles pull from same randomness and don't end up generating the same number with similar seeds.
    private bool isCritical = false; // Is this shot a critical hit?

    private void Update()
    {
        if(homingDistance > 0)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject closestEnemy = null;
            var distance = Mathf.Infinity;
            foreach(var enemy in enemies)
            {
                Vector2 distVector = enemy.transform.position - transform.position;
                if(distVector.magnitude <= homingDistance && distVector.magnitude < distance)
                {
                    distance = distVector.magnitude;
                    closestEnemy = enemy;
                }
            }
            if(closestEnemy != null)
            {
                var pos = closestEnemy.transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, pos - transform.position), rotationSpeed * Time.deltaTime);

                rigidBody.velocity = transform.up * rigidBody.velocity.magnitude;
            }
        }
    }

    void Start()
    {
        random = new System.Random();
        random.Next();
    }

    /// <summary>
    /// Give the projectile data from the weapon it is being fired from.
    /// </summary>
    public void Initialize(int damage, float speed, bool isCritical)
    {
        this.damage = damage;
        this.speed = speed;
        this.isCritical = isCritical;
    }

    /// <summary>
    /// Get the fire sound of the projectile.
    /// </summary>
    /// <returns></returns>
    public AudioClip GetFireSound()
    {
        return fireSound;
    }

    /// <summary>
    /// Called when the projectile is active in the hierarchy.
    /// </summary>
    void OnEnable()
    {
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
            // Play collision particle effect.
            ParticleManager.PlayParticle(destroyEffect, gameObject);

            // Current damage is set on collisions with entities that can take damage.
            var popUptext = PopUpTextPool.current.GetPooledObject();
            if(popUptext == null)
                return;

            // Spawn popup text within a radius of 2 from the collision.
            if (!isCritical)
                popUptext.GetComponent<PopUpText>().Initialize(gameObject, damage.ToString(), ItemTier.Tier1, radius: true);
            else
                popUptext.GetComponent<PopUpText>().Initialize(gameObject, "<style=\"CritHit\">" + damage.ToString() + "</style>", ItemTier.Tier1, radius: true);
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
                collision.GetComponentInChildren<Ship>().health -= damage;
                collided = true;
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
