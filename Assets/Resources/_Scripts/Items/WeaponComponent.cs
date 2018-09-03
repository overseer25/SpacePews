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

    private System.Random random;
    private AudioSource audioSource;

    // What time the last shot was fired.
    private float lastShot = 0.0f;

    void Start()
    {
        itemColor = ItemColors.colors[(int)itemTier];
        audioSource = GetComponent<AudioSource>();
        random = new System.Random();
        componentType = ComponentType.Weapon;
    }


    /// <summary>
    /// Returns a random damage value based on the damage parameters of the projectile.
    /// Chance for a critical attack.
    /// </summary>
    /// <returns></returns>
    private int ComputeDamage()
    {
        var damage = 0;
        if (random.NextDouble() <= critChance)
        {
            damage = (int)Math.Round(random.Next(minDamage, maxDamage) * critMultiplier);
        }
        else
        {
            damage = random.Next(minDamage, maxDamage);
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
    /// Fire the projectile.
    /// </summary>
    /// <param name="time"> The current time that has passed. If the</param>
    public void Fire()
    {
        random.Next();

        var projectile = ProjectilePool.current.GetPooledObject();
        if (projectile == null)
            return;

        var damage = ComputeDamage();
        projectile.GetComponent<Projectile>().Initialize(damage, shotSpeed);
        projectile.transform.position = shotSpawn.transform.position;
        projectile.transform.rotation = shotSpawn.transform.rotation;
        audioSource.clip = projectile.GetComponent<Projectile>().GetFireSound();
        audioSource.Play();
        projectile.SetActive(true);
    }

}
