using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponComponent : ShipComponent
{
    public float firerate;
    public float shotSpread;
    public float fireAnimPlayspeed;
    public Projectile projectile;
    public Sprite[] fireAnimation;
    private List<GameObject> shotSpawns;

    private static System.Random random;
    private AudioSource audioSource;

    // What time the last shot was fired.
    private float lastShot = 0.0f;
    private bool playingFireAnimation;

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
    private IEnumerator PlayFireAnimation()
    {
        playingFireAnimation = true;
        foreach(var frame in fireAnimation)
        {
            GetSpriteRenderer().sprite = frame;
            yield return new WaitForSeconds(fireAnimPlayspeed);
        }
        GetSpriteRenderer().sprite = sprites[index];

        playingFireAnimation = false;
        yield break;
    }

    /// <summary>
    /// Fire the projectile.
    /// </summary>
    /// <param name="time"> The current time that has passed. If the</param>
    public void Fire()
    {
        foreach(var shotSpawn in shotSpawns)
        {
            var projectile = ProjectilePool.current.GetPooledObject();
            if (projectile == null)
                continue;

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
        if(fireAnimation.Length > 0 && !playingFireAnimation)
            StartCoroutine(PlayFireAnimation());
    }

}
