using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{

    public GameObject projectile; // Contains the sprite for the projectile
    public GameObject missile; // Contains the sprite for the missile

    public float shotSpeed = 1.0f;
    public float fireRate = 0.5f;
    public float fireRateMissile = 1.0f;
    public bool isMiningLaser;

    // Variables used within the functions
    private float nextFire = 0.0f;
    private float nextFireMissile = 0.0f;

    private bool playingMiningLaser = false;
    private bool cursorSet; // To stop the update function from constantly refreshing the cursor texture;
    private bool dead = false;
    public bool menuOpen = false;
    private GameObject turret;
    public SpriteRenderer miningLaserParticles;
    public AudioSource miningLaserAudio;
    public Transform turretShotSpawn;
    public Transform missileShotSpawn;

    // Use this for initialization
    void Start()
    {
        turret = transform.Find("turret").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Don't shoot if a menu is open and don't shoot if dead
        if(!menuOpen && !dead)
        {
            if (!isMiningLaser)
            {
                if (playingMiningLaser)
                {
                    turret.GetComponent<LineRenderer>().enabled = false;
                    playingMiningLaser = false;
                    miningLaserAudio.Stop();
                    miningLaserParticles.enabled = false;
                }
                // Create projectiles
                FireProjectile();
            }
            else
            {
                // Create mining projectiles
                if (Input.GetMouseButton(0))
                {
                    turret.GetComponent<LineRenderer>().enabled = true;
                    if (!playingMiningLaser)
                    {
                        playingMiningLaser = true;
                        miningLaserAudio.Play();
                        miningLaserParticles.enabled = true;
                    }
                }
                // Stop the mining sound
                else
                {
                    turret.GetComponent<LineRenderer>().enabled = false;
                    if (playingMiningLaser)
                    {
                        playingMiningLaser = false;
                        miningLaserAudio.Stop();
                        miningLaserParticles.enabled = false;
                    }
                }
            }
        }
        
    }

    void FireProjectile()
    {
        if (Input.GetMouseButton(0) && (Time.time > nextFire))
        {

            Instantiate(projectile, turretShotSpawn.position, turretShotSpawn.rotation).GetComponent<Projectile>().Initialize(shotSpeed);
            nextFire = Time.time + fireRate;
        }

        if (Input.GetMouseButton(1) && (Time.time > nextFireMissile))
        {
            Instantiate(missile, missileShotSpawn.position, missileShotSpawn.rotation);
            nextFireMissile = Time.time + fireRateMissile;
        }
    }

    public void UpdateDead(bool isDead)
    {
        dead = isDead;
    }
}
