﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Base class for all Weapon Components.
/// </summary>
public abstract class WeaponComponentBase : ShipComponentBase
{
    public float firerate;
    public float shotSpread;
    public Projectile projectile;
    public SpriteAnimation fireAnimation;
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
    /// Fire the projectile.
    /// </summary>
    /// <param name="time"> The current time that has passed. If the</param>
    public abstract void Fire();

}
