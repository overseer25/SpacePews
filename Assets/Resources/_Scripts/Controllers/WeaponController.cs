﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    public GameObject projectileObject; // Contains the sprite for the projectile
    public GameObject missile; // Contains the sprite for the missile

    
    public bool isMiningLaser;

    private bool playingMiningLaser = false;
    // To stop the update function from constantly refreshing the cursor texture;
    private bool cursorSet; 
    private bool dead = false;

    // Variables used within the functions
    private float nextFire = 0.0f;
    private float fireRateMissile = 1.0f;
    private float nextFireMissile = 0.0f;
    public GameObject turret;
    private Projectile projectile;

    public bool menuOpen = false;
    public SpriteRenderer miningLaserParticles;

    [Header("Audio")]
    public AudioSource audioPlayer;
    public AudioClip miningLaserAudio;

    // Use this for initialization
    void Start()
    {
        projectile = projectileObject.GetComponent<Projectile>();
    }

    // Update is called once per frame
    void Update()
    {
        // Don't shoot if a menu is open and don't shoot if dead
        if(!menuOpen && !dead && !PauseMenuScript.IsPaused)
        {
            if (!isMiningLaser)
            {
                if (playingMiningLaser)
                {
                    turret.GetComponent<LineRenderer>().enabled = false;
                    playingMiningLaser = false;
                    audioPlayer.Stop();
                    miningLaserParticles.enabled = false;
                }
                // Create projectiles
                audioPlayer.clip = projectile.fireSound;
                audioPlayer.loop = false;
                FireProjectile();
            }
            else
            {
                audioPlayer.clip = miningLaserAudio;
                // Create mining projectiles
                if (Input.GetMouseButton(0))
                {
                    turret.GetComponent<LineRenderer>().enabled = true;
                    if (!playingMiningLaser)
                    {
                        playingMiningLaser = true;
                        audioPlayer.loop = true;
                        audioPlayer.Play();
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
                        audioPlayer.Stop();
                        miningLaserParticles.enabled = false;
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// Fires projectiles from the player's turret.
    /// </summary>
    void FireProjectile()
    {
        if (Input.GetMouseButton(0) && (Time.time > nextFire))
        {
            // Get an unused projectile, if it exists.
            GameObject proj = ProjectilePool.current.GetPooledObject();
            if(proj == null)
            {
                return;
            }

            // Prepare and activate the projectile.
            proj.transform.position = turret.transform.position;
            proj.transform.rotation = turret.transform.rotation;
            proj.SetActive(true);
            nextFire = Time.time + (1/projectile.fireRate);
            audioPlayer.Play();
        }
    }

    /// <summary>
    /// Update the controller on the state of the player.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        dead = isDead;
    }
}