using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{

    public float maxSpeed;
    public float acceleration = 0.0f; // Acceleration to be applied
    public float lifetime;
    public int health;
    public int damage;
    public bool homing = false;
    public GameObject explosion;
    public AudioClip collisionSound;
    private Rigidbody2D rigidBody;


    private bool collided = false;
    private float speed = 0.0f;

    /// <summary>
    /// Startup stuff
    /// </summary>
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// When the missile collides with an object, play the explosion animation
    /// </summary>
    void OnDestroy()
    {
        if (collided)
        {
            Explosion exp = Instantiate(explosion, transform.position, transform.rotation).GetComponent<Explosion>();
            exp.Initialize(collisionSound);
        }
    }

    /// <summary>
    /// Deals with the missile colliding with different objects in the scene.
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                collided = true;
                Destroy(gameObject);
                break;
            case "Player":
                collided = true;
                Destroy(gameObject);
                break;
            case "PlayerProjectile":
            case "EnemyProjectile":
                health -= collision.gameObject.GetComponent<Projectile>().Damage;
                break;
            case "Missile":
                health -= collision.gameObject.GetComponent<Missile>().damage;
                break;
        }

        if(health <= 0)
        {
            collided = true;
            Destroy(gameObject);
        }
    }
}
