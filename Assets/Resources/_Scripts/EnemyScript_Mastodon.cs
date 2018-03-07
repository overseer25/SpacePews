using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript_Mastodon : MonoBehaviour
{

    public int health; // health of the enemy
    public float speed; // Speed of the enemy
    public float stoppingDistance; // How close the enemy can get before stopping
    public float retreatDistance; // How close the enemy allows the player to get before retreating
    public float viewDistance; // How far the enemy can see

    private Transform player;
    public Transform turret1;
    public Transform turret2;
    public GameObject explosion;
    public GameObject shot;
    private GameObject thruster;
    private Rigidbody2D rigidBody;

    private float timeBetweenShots = 0.0f;
    private float startTimeBetweenShots = 1.0f;
    private float acceleration = 0.5f;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thruster = transform.Find("thruster").gameObject;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector2 direction = (player.position - transform.position).normalized;

        // Move toward the player if outside stopping distance
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = true;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // enemy tracks the player
            turret1.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // enemy tracks the player
            turret2.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // enemy tracks the player

            if (rigidBody.velocity.magnitude < speed)
            {
                rigidBody.AddForce(rigidBody.mass * (acceleration * direction));
            }
            else
            {
                rigidBody.velocity *= 0.95f;
            }
            Fire();
        }
        // Stop moving if within stopping distance and outside retreat distance
        else if ((Vector2.Distance(transform.position, player.position) < stoppingDistance) && Vector2.Distance(transform.position, player.position) > retreatDistance)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = false;
            rigidBody.velocity *= 0.95f;
            // Rotate the object according to the state of its distance from the target.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret1.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // turret tracks the player
            turret2.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // turret tracks the player
            Fire();
        }
        // Retreat if player gets too close
        else if (Vector2.Distance(transform.position, player.position) < retreatDistance)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = true;
            direction = (transform.position - player.position).normalized;
            // Rotate the object according to the state of its distance from the target.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // rotate away to flee
            turret1.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // turret tracks the player
            turret2.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // turret tracks the player

            if (rigidBody.velocity.magnitude < speed)
            {
                rigidBody.AddForce(rigidBody.mass * (acceleration * direction));
            }
            else
            {
                rigidBody.velocity *= 0.95f;
            }
            Fire();
        }
        // Stop moving and tracking if player is outside view distance
        else if (Vector2.Distance(transform.position, player.position) > viewDistance)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = false;
            transform.position = this.transform.position;
        }

        // If killed
        if (health <= 0)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            GetComponent<SpawnLoot>().Spawn(); // Drop loot
            Destroy(gameObject);
        }

        
    }

    void Fire()
    {
        // Shoot if ready, given the player isn't too close or too far away.
        if (timeBetweenShots <= 0 && Vector2.Distance(transform.position, player.position) < viewDistance)
        {

            Instantiate(shot, turret1.position, turret1.rotation);

            Instantiate(shot, turret2.position, turret2.rotation);

            timeBetweenShots = startTimeBetweenShots;
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }
}
