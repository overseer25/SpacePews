using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : ShipComponent
{
    [Header("Weapon Stats")]
    [SerializeField]
    private int minDamage;
    [SerializeField]
    private int maxDamage;
    [SerializeField]
    private float critChance;
    [SerializeField]
    private float critMultiplier;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float shotSpeed;
    [SerializeField]
    private float shotSpread;
    [SerializeField]
    private Projectile projectile;
    // If the component is a weapon, this is the gameobject where the projectile will spawn from.
    [SerializeField]
    private GameObject shotSpawn;

    private static System.Random random;
    private AudioSource audioSource;

    // What time the last shot was fired.
    private float lastShot = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        itemType = ItemType.Turret;
        audioSource = GetComponent<AudioSource>();
        random = new System.Random();
        random.Next();
    }

    /// <summary>
    /// Set the mounted status of the component.
    /// </summary>
    /// <param name="val"></param>
    public override void SetMounted(bool val)
    {
        base.SetMounted(val);
        // If the weapon component is being mounted, create the projectile pool.
        if(val)
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
    private int ComputeDamage()
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
    public void Fire()
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
    }

}
