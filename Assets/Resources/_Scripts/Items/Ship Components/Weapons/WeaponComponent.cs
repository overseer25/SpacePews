using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class WeaponComponent : ShipComponent
{
    public float firerate;
    public float shotSpread;
    public float fireAnimPlayspeed;
    public Projectile projectile;
    public Sprite[] fireAnimation;
    internal List<GameObject> shotSpawns;

    internal static System.Random random;
    internal AudioSource audioSource;

    // What time the last shot was fired.
    internal float lastShot = 0.0f;
    internal bool playingFireAnimation;

    protected override void Awake()
    {
        base.Awake();
        itemType = ItemType.Turret;
        audioSource = GetComponent<AudioSource>();
        random = new System.Random();
        random.Next();

        // Hide the sprites used in the editor for the shotspawns.
        shotSpawns = new List<GameObject>();
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            if(child.tag == "shotspawn")
            {
                shotSpawns.Add(child.gameObject);
                child.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    protected override void Update()
    {
        if(!playingFireAnimation)
            base.Update();
    }

    /// <summary>
    /// Check if the weapon can fire. If it can, fire it.
    /// </summary>
    public abstract void CheckFire();

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
        return firerate + lastShot;
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
        return (projectile.critChance) + "%";
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
    /// Play the weapons fire animation, if it exists.
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayFireAnimation()
    {
        playingFireAnimation = true;
        foreach(var frame in fireAnimation)
        {
            spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(fireAnimPlayspeed);
        }
        spriteRenderer.sprite = sprites[index];

        playingFireAnimation = false;
        yield break;
    }

    /// <summary>
    /// Fire the projectile.
    /// </summary>
    /// <param name="time"> The current time that has passed. If the</param>
    public abstract void Fire();

}
