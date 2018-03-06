using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {


    private Rigidbody2D rigidBody;
    public float speed;
    public float lifetime = 1.0f;
    public GameObject explosion;

    private bool bounce = false;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.up * speed;

        Destroy(gameObject, lifetime);
	}

    public void Initialize(float shotSpeed, bool bounce)
    {
        speed = shotSpeed;
        this.bounce = bounce;

        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.up * speed;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.tag == "Player" && gameObject.tag == "PlayerProjectile") { return; }
        if(collision.gameObject.tag == "Enemy" && gameObject.tag == "EnemyProjectile") { return; }

        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Asteroid" || collision.gameObject.tag == "Mineable")
        {
            if(!bounce)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 15.0f, ~(1 << 5));
                rigidBody.velocity = hit.normal * speed;

            }
            
        }
    }

}
