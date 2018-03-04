using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript_Redwing : MonoBehaviour {

    public int health; // health of the enemy
    public float speed; // Speed of the enemy
    public float stoppingDistance; // How close the enemy can get before stopping
    public float retreatDistance; // How close the enemy allows the player to get before retreating
    public float viewDistance; // How far the enemy can see

    private Transform player;
    public Transform shotSpawn;
    public Transform shotSpawn2;
    public GameObject explosion;
    public GameObject shot;
    private GameObject thruster;
    private Rigidbody2D rigidBody;

    private float timeBetweenShots = 0.0f;
    private float startTimeBetweenShots = 1.0f;
    private float acceleration = 0.5f;
    private float maxSpeed = 2.0f;
    private bool switchSides = false; // swaps shotSpawns, as the redwing has two gun barrels.


    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thruster = transform.Find("thruster").gameObject;
        rigidBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector2 direction = (player.position - transform.position).normalized;

        // Move toward the player if outside stopping distance
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = true;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // enemy tracks the player

            if (rigidBody.velocity.magnitude < maxSpeed)
            {
                rigidBody.AddForce(rigidBody.mass * (acceleration * direction));
            }
            else
            {
                rigidBody.velocity *= 0.95f;
            }
        }
        // Stop moving if within stopping distance and outside retreat distance
        else if((Vector2.Distance(transform.position, player.position) < stoppingDistance) && Vector2.Distance(transform.position, player.position) > retreatDistance)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = false;
            rigidBody.velocity *= 0.95f;
            // Rotate the object according to the state of its distance from the target.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // enemy tracks the player
        }
        // Retreat if player gets too close
        else if(Vector2.Distance(transform.position, player.position) < retreatDistance)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = true;
            direction = (transform.position - player.position).normalized;
            // Rotate the object according to the state of its distance from the target.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // rotate away to flee

            if (rigidBody.velocity.magnitude < maxSpeed)
            {
                rigidBody.AddForce(rigidBody.mass * (acceleration * direction));
            }
            else
            {
                rigidBody.velocity *= 0.95f;
            }
        }
        // Stop moving and tracking if player is outside view distance
        else if(Vector2.Distance(transform.position, player.position) > viewDistance)
        {
            thruster.GetComponent<SpriteRenderer>().enabled = false;
            transform.position = this.transform.position;
        }

        // If killed
        if(health <= 0)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            GetComponent<SpawnLoot>().Spawn(); // Drop loot
            Destroy(gameObject);
        }

        // Shoot if ready, given the player isn't too close or too far away.
        if (timeBetweenShots <= 0 && Vector2.Distance(transform.position, player.position) < viewDistance)
        {
            if(!switchSides)
            {
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                switchSides = !switchSides;
            }
            else if (switchSides)
            {
                Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation);
                switchSides = !switchSides;
            }
            timeBetweenShots = startTimeBetweenShots;       
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }
}
