using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// The minimum damage the projectile can do.
    /// </summary>
    public int minDamage;

    /// <summary>
    /// The maximum damage the projectile can do.
    /// </summary>
    public int maxDamage;

    /// <summary>
    /// The speed of the projectile.
    /// </summary>
    public float speed;

    /// <summary>
    /// What chance does it have to be a critical?
    /// </summary>
    public float critChance;

    /// <summary>
    /// The critical multiplier.
    /// </summary>
    public float critMultiplier;

    /// <summary>
    /// Does the projectile home in on enemies?
    /// </summary>
    public bool homing;

    /// <summary>
    /// At what distance does it begin to home in?
    /// </summary>
    public float homingDistance;

    /// <summary>
    /// How quickly does it snap to the target?
    /// </summary>
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

    public AudioClip fireSound;
    public ParticleEffect destroyEffect;
    public int lifetime;

	/// <summary>
	/// Is the projectile animated when traveling?
	/// </summary>
	public bool animated;

	/// <summary>
	/// Static sprite when not animated.
	/// </summary>
	public Sprite sprite;

    /// <summary>
    /// The sprites of the projectile if animated.
    /// </summary>
    public SpriteAnimation sprites;

    /// <summary>
    /// The sprite animation that plays when the projectile is fired. Does not loop.
    /// </summary>
    public SpriteAnimation fireSprites;

    internal Rigidbody2D rigidBody;
    internal SpriteRenderer spriteRenderer;
    internal bool collided = false; // Check to see if a collision sound should be played on deactivation.
    internal static System.Random random; // Static so all projectiles pull from same randomness and don't end up generating the same number with similar seeds.
    internal bool isCritical = false; // Is this shot a critical hit?

    
    private int damage;
	private Coroutine animate;

    protected virtual void Awake()
    {
        random = new System.Random();
        random.Next();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
		if (spriteRenderer == null)
			Debug.LogError("WARNING: Projectile " + this + " does not contain a sprite renderer. This will cause problems with rendering.");
    }

	/// <summary>
	/// Coroutine for the traveling animation.
	/// </summary>
	/// <returns></returns>
	private IEnumerator Animate()
	{
		while (gameObject.activeInHierarchy)
		{
			var sprite = sprites.GetNextFrame();
			spriteRenderer.sprite = sprite;
			yield return new WaitForSeconds(sprites.playSpeed);
		}
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
		if (splitting && splitAfterTime > 0.0f)
        {
            StartCoroutine(BeginSplit());
        }
    }

    /// <summary>
    /// Play the fire animation.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayFireAnimation()
    {
		Sprite frame = fireSprites.GetNextFrame();
        while (frame != null)
        {
			spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(fireSprites.playSpeed);
			frame = fireSprites.GetNextFrame();
		}

		if (animated)
			animate = StartCoroutine(Animate());
		else
			spriteRenderer.sprite = sprite;
    }

    /// <summary>
    /// After an amount of time has passed, split the projectile.
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
            var projectile = ProjectilePool.current.GetPooledObject() as Projectile;
            projectile.Copy(data.projectile);
            projectile.damage = random.Next(1, this.damage);
            projectile.speed = data.projectile.speed;
            projectile.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, data.angle);
            projectile.transform.position = transform.position + (projectile.transform.up);
            projectile.gameObject.SetActive(true);
        }
        if(destroyOnSplit)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Returns a random damage value based on the damage parameters of the projectile.
    /// Chance for a critical attack.
    /// </summary>
    /// <returns></returns>
    private int ComputeDamage()
    {
        var damage = random.Next(minDamage, maxDamage + 1);
        float critChanceVal = critChance / 100;
        var result = random.NextDouble();
        // If adding the multiplier to the current damage is larger than max damage and is a critical hit.
        if ((damage * critMultiplier > maxDamage) && result <= critChanceVal)
        {
            isCritical = true;
            damage = (int)Math.Ceiling(damage * critMultiplier);
        }
        else
        {
            isCritical = false;
            damage = random.Next(minDamage, maxDamage + 1);
        }

        return damage;
    }

    /// <summary>
    /// Copy data from one projectile to another.
    /// </summary>
    /// <param name="other"></param>
    public void Copy(Projectile other)
    {
        minDamage = other.minDamage;
        maxDamage = other.maxDamage;
        speed = other.speed;
        critChance = other.critChance;
        critMultiplier = other.critMultiplier;
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
		animated = other.animated;
		sprite = other.sprite;
		if(other.sprites != null)
			sprites = Instantiate(other.sprites);
		if(fireSprites != null)
			fireSprites = Instantiate(other.fireSprites);
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
        damage = ComputeDamage();
        waitForSplit = false;
        rigidBody.velocity = transform.up * speed;
        // Start the clock to disabling again.
        StartCoroutine(RemoveAfterSeconds(lifetime));

		if (fireSprites != null)
		{
			fireSprites.ResetAnimation();
			StartCoroutine(PlayFireAnimation());
		}
		else
		{
			if (animated)
				animate = StartCoroutine(Animate());
			else
				spriteRenderer.sprite = sprite;
		}
	}

    /// <summary>
    /// When the projectile collides with an object, play the explosion animation
    /// </summary>
    protected virtual void OnDisable()
    {
        if (collided)
        {
            // Play collision particle effect.
            ParticleManager.PlayParticle(destroyEffect, gameObject.transform.position, gameObject.transform.rotation);

            // Current damage is set on collisions with entities that can take damage.
			if(OptionsMenu.current.graphicsMenu.DamageNumbersEnabled())
			{
				var popUptext = PopUpTextPool.current.GetPooledObject() as PopUpText;
				if (popUptext == null)
					return;

				// Spawn popup text within a radius of 2 from the collision.
				if (!isCritical)
					popUptext.Initialize(gameObject, damage.ToString(), 8f, Color.white, radius: 1.0f, playDefaultSound: false);
				else
					popUptext.Initialize(gameObject, damage.ToString(), 8f, Color.yellow, radius: 1.0f, playDefaultSound: false);
			}
        }
        StopCoroutine(PlayFireAnimation());
		if(animate != null)
		{
			StopCoroutine(animate);
			animate = null;
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
            case "Tile":
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
