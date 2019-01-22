using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : ShipComponent
{
    [Header("Weapon Stats")]
    [SerializeField]
    internal int minDamage;
    [SerializeField]
    internal int maxDamage;
    [SerializeField]
    internal float critChance;
    [SerializeField]
    internal float critMultiplier;
    [SerializeField]
    internal float fireRate;
    [SerializeField]
    internal float shotSpeed;
    [SerializeField]
    internal float shotSpread;
    [SerializeField]
    internal Projectile projectile;
    // If the component is a weapon, this is the gameobject where the projectile will spawn from.
    [SerializeField]
    internal GameObject shotSpawn;
    [SerializeField]
    internal Sprite[] fireAnimation;
    [SerializeField]
    internal float fireAnimationPlaySpeed;

    internal static System.Random random;
    internal AudioSource audioSource;
    internal SpriteRenderer spriteRenderer;

    // What time the last shot was fired.
    internal float lastShot = 0.0f;

    void Awake()
    {
        itemColor = ItemColors.colors[(int)itemTier];
        audioSource = GetComponent<AudioSource>();
        random = new System.Random();
        random.Next();
        itemType = ItemType.Turret;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if(spriteRenderer == null)
        {
            Debug.LogError("WARNING: Weapon Component (" + this + ") does not contain a sprite renderer.");
        }
    }

    /// <summary>
    /// Set the mounted status of the component.
    /// </summary>
    /// <param name="val"></param>
    public override void SetMounted(bool val)
    {
        mounted = val;
        // If the weapon component is being mounted, create the projectile pool.
        if(val == true)
        {
            GetComponent<ProjectilePool>().CreatePool();
        }
        // Else, destroy the projectile pool.
        else
        {
            GetComponent<ProjectilePool>().DestroyPool();
        }
    }

    /// <summary>
    /// Returns a random damage value based on the damage parameters of the projectile.
    /// Chance for a critical attack.
    /// </summary>
    /// <returns></returns>
    internal int ComputeDamage()
    {
        var damage = random.Next(minDamage, maxDamage + 1);

        // If adding the multiplier to the current damage is larger than max damage and is a critical hit.
        if ((damage * critMultiplier > maxDamage) && random.NextDouble() <= critChance)
        {
            damage = (int)(damage * critMultiplier);
        }
        else
        {
            damage = random.Next(minDamage, maxDamage+1);
        }

        return damage;
    }

    /// <summary>
    /// Play the animation for firing, if it exists.
    /// </summary>
    internal IEnumerator PlayFireAnimation()
    {
        if (fireAnimation == null || fireAnimation.Length <= 1)
            yield break;

        for(int i = 1; i < fireAnimation.Length; i++)
        {
            spriteRenderer.sprite = fireAnimation[i];
            yield return new WaitForSeconds(fireAnimationPlaySpeed);
        }

        spriteRenderer.sprite = fireAnimation[0];
        yield break;
    }

    /// <summary>
    /// Gets the fire rate of the weapon.
    /// </summary>
    /// <returns></returns>
    public float GetNextShotTime()
    {
        return fireRate + lastShot;
    }

    /// <summary>
    /// Sets the last time a shot was fired.
    /// </summary>
    public void SetLastShot(float time)
    {
        lastShot = time;
    }

    /// <summary>
    /// Returns a string representation of the damage range of this weapon component.
    /// </summary>
    /// <returns></returns>
    public string GetDamageString()
    {
        return minDamage + "-" + maxDamage;
    }

    /// <summary>
    /// Returns a string representation of the critical chance.
    /// </summary>
    /// <returns></returns>
    public string GetCriticalChanceString()
    {
        return MathUtils.ConvertToPercent(critChance); ;
    }

    /// <summary>
    /// Returns a string representation of the critical damage multiplier.
    /// </summary>
    /// <returns></returns>
    public string GetCriticalMultiplierString()
    {
        return critMultiplier + "x";
    }

    /// <summary>
    /// Fire the projectile.
    /// </summary>
    /// <param name="time"> The current time that has passed. If the</param>
    public virtual void Fire()
    {
        var projectile = GetComponent<ProjectilePool>().GetPooledObject();
        if (projectile == null)
            return;

        var damage = ComputeDamage();
        projectile.GetComponent<Projectile>().Initialize(damage, shotSpeed, (damage > maxDamage) ? true : false);
        projectile.transform.position = shotSpawn.transform.position;
        Quaternion rotation = shotSpawn.transform.transform.rotation;
        var angle = UnityEngine.Random.Range(-shotSpread, shotSpread);
        rotation *= Quaternion.Euler(0, 0, angle);
        projectile.transform.rotation = rotation;
        audioSource.clip = projectile.GetComponent<Projectile>().GetFireSound();
        audioSource.Play();
        projectile.SetActive(true);

        StartCoroutine(PlayFireAnimation());
    }

}
