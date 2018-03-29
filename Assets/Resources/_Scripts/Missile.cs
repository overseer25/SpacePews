using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public float maxSpeed;
    public float acceleration = 0.0f; // Acceleration to be applied
    public float lifetime;
    public int damage;
    public bool homing = false;
    public bool followMouse = false;
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

        // If liftime = 0, don't destroy after time.
        if(lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
    }
	
    /// <summary>
    /// Movement of the missile
    /// </summary>
    void FixedUpdate()
    {
        speed = (speed < maxSpeed) ? (speed + acceleration) : (speed); // increase acceleration until reaches max acceleration

        if (!homing && !followMouse)
        {
            rigidBody.velocity = transform.up * speed;
        }
        else if(followMouse)
        {
            Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // missile tracks the mouse
            if(rigidBody.velocity.magnitude < maxSpeed)
            {
                rigidBody.AddForce(speed * 5.0f * direction + transform.up);
            }
            else
            {
                rigidBody.AddForce(speed * direction + transform.up);
            }
            collided = true;
        }
    }

    /// <summary>
    /// When the missile collides with an object, play the explosion animation
    /// </summary>
    void OnDestroy()
    {
        if(collided)
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
        switch(collision.gameObject.tag)
        {
            case "Enemy":
                collision.gameObject.GetComponent<EnemyScript>().health -= damage;
                Destroy(gameObject);
                break;
            case "Asteroid":
            case "Immovable":
            case "Mineable":
            case "Mine":
                collided = true;
                Destroy(gameObject);
                break;
        }
    }
}
