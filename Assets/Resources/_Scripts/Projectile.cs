using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {


    private Rigidbody2D rigidBody;
    public float speed;
    public float lifetime = 1.0f;
    public GameObject explosion;
    public int damage = 1;
    public AudioClip collisionSound;

    private bool collided = false; // Check to see if a collision sound should be played on destroy.

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.up * speed;

        Destroy(gameObject, lifetime);
	}

    public void Initialize(float shotSpeed)
    {
        speed = shotSpeed;

        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.up * speed;

    }

    /// <summary>
    /// When the projectile collides with an object, play the explosion animation
    /// </summary>
    void OnDestroy()
    {
        if(collided)
        {
            Explosion exp = Instantiate(explosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
            exp.Initialize(collisionSound);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // If friendly fire, ignore
        if(collision.gameObject.tag == "Player" && gameObject.tag == "PlayerProjectile") { return; }
        if(collision.gameObject.tag == "Enemy" && gameObject.tag == "EnemyProjectile") { return; }

        switch(collision.gameObject.tag)
        {
            case "Player":
            case "Enemy":
            case "Asteroid":
            case "Mineable":
            case "Mine":
                collided = true;
                Destroy(gameObject);
                break;

        }
    }

}
