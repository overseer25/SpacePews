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
    private Projectile projectile;
    // If the component is a weapon, this is the gameobject where the projectile will spawn from.
    [SerializeField]
    private GameObject shotSpawn;

    private static System.Random random;
    private AudioSource audioSource;

    // What time the last shot was fired.
    private float lastShot = 0.0f;

    void Awake()
    {
        itemColor = ItemColors.colors[(int)itemTier];
        audioSource = GetComponent<AudioSource>();
        random = new System.Random();
        random.Next();
        itemType = ItemType.Weapon;
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
        var projectile = ProjectilePool.current.GetPooledObject();
        if (projectile == null)
            return;

        var damage = ComputeDamage();
        projectile.GetComponent<Projectile>().Initialize(damage, shotSpeed, (damage > maxDamage) ? true : false);
        projectile.transform.position = shotSpawn.transform.position;
        projectile.transform.rotation = shotSpawn.transform.rotation;
        audioSource.clip = projectile.GetComponent<Projectile>().GetFireSound();
        audioSource.Play();
        projectile.SetActive(true);
    }

}
