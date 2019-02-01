using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : ShipComponent
{
    [Header("Weapon Stats")]
    [SerializeField]
    private float fireRate;
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
        return projectile.minDamage + "-" + projectile.maxDamage;
    }

    /// <summary>
    /// Returns a string representation of the critical chance.
    /// </summary>
    /// <returns></returns>
    public string GetCriticalChanceString()
    {
        return MathUtils.ConvertToPercent(projectile.critChance); ;
    }

    /// <summary>
    /// Returns a string representation of the critical damage multiplier.
    /// </summary>
    /// <returns></returns>
    public string GetCriticalMultiplierString()
    {
        return projectile.critMultiplier + "x";
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

        projectile.Copy(this.projectile);
        projectile.gameObject.transform.position = shotSpawn.transform.position;
        Quaternion rotation = shotSpawn.transform.transform.rotation;
        var angle = UnityEngine.Random.Range(-shotSpread, shotSpread);
        rotation *= Quaternion.Euler(0, 0, angle);
        projectile.gameObject.transform.rotation = rotation;
        audioSource.clip = projectile.GetComponent<Projectile>().GetFireSound();
        audioSource.Play();
        projectile.gameObject.SetActive(true);
    }

}
