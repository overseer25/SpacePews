﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public bool homing;
    [HideInInspector]
    public float homingDistance;
    [HideInInspector]
    public float homingTurnSpeed;

    /// <summary>
    /// Does the projectile split into other projectiles?
    /// </summary>
    public bool splitting;
    /// <summary>
    /// What projectiles does it split into?
    /// </summary>
    public ProjectileData[] splitProjectiles;
    /// <summary>
    /// Does it split on collision?
    /// </summary>
    public bool splitOnCollision;
    /// <summary>
    /// Does it destroy when split?
    /// </summary>
    public bool destroyOnSplit;
    /// <summary>
    /// If it does not split on collision, split after time (this only happens if <see cref="splitting"/> is set to true.
    /// </summary>
    public float splitAfterTime;

    /// <summary>
    /// Used by the split after time coroutine so that several coroutines cannot be spun off at the same time.
    /// </summary>
    private bool waitForSplit;


    [HideInInspector]
    public AudioClip fireSound;
    [HideInInspector]
    public ParticleEffect destroyEffect;
    [HideInInspector]
    public int lifetime;
    [HideInInspector]
    public float playspeed;

    /// <summary>
    /// The sprites of the projectile. Only animates if there is more than one sprite, otherwise is static.
    /// </summary>
    [SerializeField]
    public Sprite[] sprites;

    internal Rigidbody2D rigidBody;
    internal SpriteRenderer spriteRenderer;
    internal bool collided = false; // Check to see if a collision sound should be played on deactivation.
    internal static System.Random random; // Static so all projectiles pull from same randomness and don't end up generating the same number with similar seeds.
    internal bool isCritical = false; // Is this shot a critical hit?

    private int damage;
    private float speed;
    private float changeSprite;
    private int index;

    protected virtual void Start()
    {
        random = new System.Random();
        random.Next();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("WARNING: Projectile " + this + " does not contain a sprite renderer. This will cause problems with rendering.");
        else if (sprites.Length == 0)
            Debug.LogError("WARNING: Projectile " + this + " does not contain any sprites in the sprite list. This will cause problems with rendering.");
        else
            spriteRenderer.sprite = sprites[0];
    }

    protected virtual void Update()
    {
        if(homing)
        {
            var target = EnemyUtils.FindNearestEnemy(gameObject, homingDistance);
            if(target != null)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, target.transform.position - transform.position), homingTurnSpeed * Time.deltaTime);
                rigidBody.velocity = transform.up.normalized * rigidBody.velocity.magnitude;
            }
        }
        if(splitting && splitAfterTime > 0.0f)
        {
            StartCoroutine(BeginSplit());
        }
        if(sprites.Length > 1)
        {
            // Player sprite animation
            if (Time.time > changeSprite)
            {
                changeSprite = Time.time + playspeed;
                index++;
                if (index >= sprites.Length) { index = 0; } // Restart animation
                spriteRenderer.sprite = sprites[index];
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeginSplit()
    {
        if(!waitForSplit)
        {
            waitForSplit = true;
            yield return new WaitForSeconds(splitAfterTime);
            Split();
            waitForSplit = false;
        }
    }

    /// <summary>
    /// Split the projectile based on the splitting parameters.
    /// </summary>
    private void Split()
    {
        StopCoroutine(BeginSplit());
        foreach (var data in splitProjectiles)
        {
            var projectile = ProjectilePool.current.GetPooledObject();
            projectile.Copy(data.projectile);
            projectile.damage = random.Next(1, this.damage);
            projectile.speed = data.speed;
            projectile.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, data.angle);
            projectile.transform.position = transform.position + projectile.transform.up;
            projectile.gameObject.SetActive(true);
        }
        if(destroyOnSplit)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Give the projectile data from the weapon it is being fired from.
    /// </summary>
    public void Initialize(int damage, float speed, bool isCritical)
    {
        this.damage = damage;
        this.speed = speed;
        this.isCritical = isCritical;
    }

    /// <summary>
    /// Copy data from one projectile to another.
    /// </summary>
    /// <param name="other"></param>
    public void Copy(Projectile other)
    {
        homing = other.homing;
        homingDistance = other.homingDistance;
        homingTurnSpeed = other.homingTurnSpeed;
        splitting = other.splitting;
        splitProjectiles = other.splitProjectiles;
        splitOnCollision = other.splitOnCollision;
        destroyOnSplit = other.destroyOnSplit;
        splitAfterTime = other.splitAfterTime;
        fireSound = other.fireSound;
        destroyEffect = other.destroyEffect;
        lifetime = other.lifetime;
        playspeed = other.playspeed;
        sprites = other.sprites;
    }

    /// <summary>
    /// Get the fire sound of the projectile.
    /// </summary>
    /// <returns></returns>
    public AudioClip GetFireSound()
    {
        return fireSound;
    }

    /// <summary>
    /// Called when the projectile is active in the hierarchy.
    /// </summary>
    protected virtual void OnEnable()
    {
        waitForSplit = false;
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.up * speed;
        // Start the clock to disabling again.
        StartCoroutine(RemoveAfterSeconds(lifetime));
    }

    /// <summary>
    /// When the projectile collides with an object, play the explosion animation
    /// </summary>
    protected virtual void OnDisable()
    {
        if (collided)
        {
            // Play collision particle effect.
            ParticleManager.PlayParticle(destroyEffect, gameObject);

            // Current damage is set on collisions with entities that can take damage.
            var popUptext = PopUpTextPool.current.GetPooledObject();
            if(popUptext == null)
                return;

            // Spawn popup text within a radius of 2 from the collision.
            if (!isCritical)
                popUptext.GetComponent<PopUpText>().Initialize(gameObject, damage.ToString(), ItemTier.Tier1, radius: true);
            else
                popUptext.GetComponent<PopUpText>().Initialize(gameObject, "<style=\"CritHit\">" + damage.ToString() + "</style>", ItemTier.Tier1, radius: true);
        }

    }

    /// <summary>
    /// Disables the projectile after a set time has passed.
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    protected IEnumerator RemoveAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        collided = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles collisions.
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // If friendly fire, ignore
        if (collision.gameObject.tag == "Player" && gameObject.tag == "PlayerProjectile") { return; }
        if (collision.gameObject.tag == "Enemy" && gameObject.tag == "EnemyProjectile") { return; }

        switch (collision.gameObject.tag)
        {
            case "Player":
            case "Enemy":
                collision.GetComponentInChildren<Ship>().health -= damage;
                collided = true;
                gameObject.SetActive(false);
                if (splitting && splitOnCollision)
                    Split();
                break;
            case "Asteroid":
            case "Mineable":
            case "Mine":
                collided = true;
                gameObject.SetActive(false);
                if (splitting && splitOnCollision)
                    Split();
                break;

        }
    }

}
