using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    public int health; // health of the enemy
    public float maxSpeed; // Speed of the enemy
    public float stoppingDistance; // How close the enemy can get before stopping
    public float retreatDistance; // How close the enemy allows the player to get before retreating
    public float viewDistance; // How far the enemy can see
    public float viewDistanceTurrets; // How far the enemy turrets can see.
    public float fireRate;
    public bool shipTracksPlayerOnHalt; // Does the ship turn with the player when it stops moving.

    public GameObject explosion;
    public GameObject shot;
    public Transform[] shotSpawns; // For fixed weapons.
    public GameObject[] turrets; // For gimbal turrets. I think in the future these should have a TURRET script attached that handles fireRate, projectiles, etc.
    public GameObject[] thrusters; // All thruster ports on the ship.

    private Rigidbody2D rigidBody;
    private Transform player;

    private float timeBetweenShots = 0.0f;
    private float acceleration = 0.5f;


    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rigidBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector2 direction = (player.position - transform.position).normalized;

        // Move toward the player if outside stopping distance
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            foreach (GameObject thruster in thrusters) { thruster.GetComponent<SpriteRenderer>().enabled = true; } // Activate all thrusters

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // enemy tracks the player

            foreach (GameObject turret in turrets) { turret.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); } // rotate turrets toward player.

            if (rigidBody.velocity.magnitude < maxSpeed)
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
        else if((Vector2.Distance(transform.position, player.position) < stoppingDistance) && Vector2.Distance(transform.position, player.position) > retreatDistance)
        {
            foreach (GameObject thruster in thrusters) { thruster.GetComponent<SpriteRenderer>().enabled = false; } // Deactivate all thrusters

            rigidBody.velocity *= 0.95f;
            // Rotate the object according to the state of its distance from the target.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if(shipTracksPlayerOnHalt)
            {
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // enemy tracks the player
            }

            foreach (GameObject turret in turrets) { turret.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); } // rotate turrets toward player.

            Fire();
        }
        // Retreat if player gets too close
        else if(Vector2.Distance(transform.position, player.position) < retreatDistance)
        {
            foreach (GameObject thruster in thrusters) { thruster.GetComponent<SpriteRenderer>().enabled = true; } // Activate all thrusters

            direction = (transform.position - player.position).normalized;
            // Rotate the object according to the state of its distance from the target.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // rotate away to flee

            foreach (GameObject turret in turrets) { turret.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward); } // rotate turrets toward player.

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
            foreach (GameObject thruster in thrusters) { thruster.GetComponent<SpriteRenderer>().enabled = false; } // Deactivate all thrusters

            transform.position = this.transform.position;
        }

        // If killed
        if(health <= 0)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            GetComponent<SpawnLoot>().Spawn(); // Drop loot
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Fire a shot
    /// </summary>
    void Fire()
    {
        // Shoot guns if ready, given the player isn't too close or too far away.
        if (timeBetweenShots <= 0 && Vector2.Distance(transform.position, player.position) < viewDistance && shotSpawns != null)
        {
            foreach (Transform shotSpawn in shotSpawns)
            {
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            }
        }
        // Shoot turrets if ready, given the player isn't too close or too far away.
        if (timeBetweenShots <= 0 && Vector2.Distance(transform.position, player.position) < viewDistanceTurrets && turrets != null)
        {
            foreach (GameObject turret in turrets)
            {
                Instantiate(shot, turret.transform.position, turret.transform.rotation);
            }
        }

        if(timeBetweenShots <= 0)
        {
            timeBetweenShots = fireRate;
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Deals with collisions for the enemy.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        switch(collider.gameObject.tag)
        {
            case "PlayerProjectile":
                health -= collider.gameObject.GetComponent<Projectile>().GetDamage();
                break;
            case "Immovable":
            case "Asteroid":
            case "Mineable":
                Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
                Vector2 direction = Vector2.zero; // Direction of the collision, with the magnitude applied being the speed of the player.
                // If the player isn't moving, we need to move them away, as they would be able to clip into the immovable otherwise.
                if (rigidBody.velocity == Vector2.zero)
                {
                    direction = (gameObject.transform.position - collider.gameObject.transform.position) * 20;
                }
                // The difference here is the player has a velocity, so we will propel them away from the immovable using their velocity magnitude.
                else
                {
                    // Different damage values depending on speed of collision.
                    if (rigidBody.velocity.magnitude >= 3 && rigidBody.velocity.magnitude < 5)
                    {
                        gameObject.GetComponent<PlayerController>().health--;
                    }
                    else if (rigidBody.velocity.magnitude >= 5 && rigidBody.velocity.magnitude < 10)
                    {

                        gameObject.GetComponent<PlayerController>().health -= 2;
                    }
                    else if (rigidBody.velocity.magnitude >= 10)
                    {
                        gameObject.GetComponent<PlayerController>().health -= 3;
                    }
                    direction = (gameObject.transform.position - collider.gameObject.transform.position).normalized * (rigidBody.velocity.magnitude * 100);
                }
                rigidBody.AddForce(direction); // Apply the force
                break;
        }
    }
}
