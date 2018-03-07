using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurretController : MonoBehaviour
{

    public GameObject projectile; // Contains the sprite for the projectile

    // Variables used to determine fire patterns/properties
    public bool isMiningLaser = false;
    public bool doubleShot = false;
    public bool tripleShot = false;
    public bool quadShot = false;
    public bool crossShot = false;
    public bool omniShot = false;
    public bool bounceShot = false;

    public float shotSpeed = 1.0f;
    public float fireRate = 0.5f;

    public AudioSource projectileSoundSource;
    public AudioClip[] projectileSounds; // Holds possible sounds for different shot types.

    // Variables used within the functions
    private float nextFire = 0.0f;
    private float fireRateModifier = 0.0f; // increases or decreases firerate depending on fire mode.
                                           // Negative values increase firerate, positive values decrease.
    private bool playingMiningLaser = false;
    private bool cursorSet; // To stop the update function from constantly refreshing the cursor texture;
    public bool menuOpen = false;
    private GameObject turret;
    public SpriteRenderer miningLaserParticles;
    public AudioSource miningLaserAudio;
    public Transform shotSpawn;

    // Use this for initialization
    void Start()
    {
        turret = transform.Find("turret").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Don't shoot if a menu is open
        if(!menuOpen)
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

    /// <summary>
    /// Resets the projectile's upgrades to false, so that the player fires the default shot again.
    /// </summary>
    public void ResetToDefault()
    {
        isMiningLaser = false;
        doubleShot = false;
        tripleShot = false;
        quadShot = false;
        crossShot = false;
        omniShot = false;
        bounceShot = false;
    }

    void FireProjectile()
    {
        if (Input.GetMouseButton(0) && (Time.time > nextFire))
        {
            Projectile proj;

            // If both, do Quintuple shot synergy
            if (doubleShot && tripleShot)
            {
                fireRateModifier = 1.0f;
                projectileSoundSource.clip = projectileSounds[4];
                Quaternion leftRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, 5.0f);
                Quaternion rightRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -5.0f);
                Quaternion midLeftRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f);
                Quaternion midRightRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -1.0f);

                proj = Instantiate(projectile, shotSpawn.position, leftRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.2f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, rightRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.2f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, midLeftRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.1f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, midRightRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.1f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation).GetComponent<Projectile>();
                proj.Initialize(shotSpeed, bounceShot);

                projectileSoundSource.Play();
            }
            // Fire double shot
            else if (doubleShot)
            {
                fireRateModifier = 0.2f;
                projectileSoundSource.clip = projectileSounds[1];

                proj = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.1f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.1f);
                proj.Initialize(shotSpeed, bounceShot);

                projectileSoundSource.Play();
            }
            // Fire triple shot
            else if (tripleShot)
            {
                fireRateModifier = 0.5f;
                projectileSoundSource.clip = projectileSounds[2];
                Quaternion leftRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, 5.0f);
                Quaternion rightRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -5.0f);

                proj = Instantiate(projectile, shotSpawn.position, leftRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.1f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation).GetComponent<Projectile>();
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, rightRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.1f);
                proj.Initialize(shotSpeed, bounceShot);

                projectileSoundSource.Play();
            }
            // Fire triple shot
            else if (quadShot)
            {
                fireRateModifier = 0.8f;
                projectileSoundSource.clip = projectileSounds[3];
                Quaternion leftRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, 5.0f);
                Quaternion rightRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -5.0f);
                Quaternion midLeftRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f);
                Quaternion midRightRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -1.0f);

                proj = Instantiate(projectile, shotSpawn.position, leftRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.2f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, rightRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.2f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, midLeftRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.1f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, midRightRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.1f);
                proj.Initialize(shotSpeed, bounceShot);

                projectileSoundSource.Play();
            }
            // Cross shot
            else if (crossShot)
            {
                fireRateModifier = 0.8f;
                projectileSoundSource.clip = projectileSounds[0];
                Quaternion rightRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -90.0f);
                Quaternion leftRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, 90.0f);
                Quaternion downRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -180.0f);

                proj = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation).GetComponent<Projectile>();
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, downRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.down * -1.0f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, rightRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.5f + Vector3.up * 0.5f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, leftRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.5f + Vector3.up * 0.5f);
                proj.Initialize(shotSpeed, bounceShot);

                projectileSoundSource.Play();
            }
            else if (omniShot)
            {
                fireRateModifier = 0.8f;
                projectileSoundSource.clip = projectileSounds[0];
                Quaternion neRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -45.0f);
                Quaternion eRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -90.0f);
                Quaternion seRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -135.0f);
                Quaternion sRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -180.0f);
                Quaternion swRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -225.0f);
                Quaternion wRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -270.0f);
                Quaternion nwRotation = shotSpawn.rotation * Quaternion.Euler(0.0f, 0.0f, -315.0f);

                proj = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation).GetComponent<Projectile>();
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, neRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.5f + Vector3.up * 0.5f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, eRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.right * 0.5f + Vector3.up * 0.8f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, seRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.down * -1.0f + Vector3.right * 0.5f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, sRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.down * -1.0f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, swRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.down * -1.0f + Vector3.left * 0.5f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, wRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.5f + Vector3.up * 0.8f);
                proj.Initialize(shotSpeed, bounceShot);

                proj = Instantiate(projectile, shotSpawn.position, nwRotation).GetComponent<Projectile>();
                proj.transform.Translate(Vector3.left * 0.5f + Vector3.up * 0.5f);
                proj.Initialize(shotSpeed, bounceShot);

                projectileSoundSource.Play();
            }
            // Fire normal shot
            else
            {
                fireRateModifier = 0.0f;
                projectileSoundSource.clip = projectileSounds[0];
                Instantiate(projectile, shotSpawn.position, shotSpawn.rotation).GetComponent<Projectile>().Initialize(shotSpeed, bounceShot);
                projectileSoundSource.Play();
            }

            nextFire = Time.time + (fireRate + fireRateModifier);
        }


    }
}
