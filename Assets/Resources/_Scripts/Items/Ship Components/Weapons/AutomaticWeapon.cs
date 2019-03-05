using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticWeapon : WeaponComponent
{
    /// <summary>
    /// Fire the weapon if it is ready to fire the next round.
    /// </summary>
    public override void CheckFire()
    {
        if (Time.time > GetNextShotTime())
        {
            Fire();
            SetLastShot(Time.time);
        }
    }

    /// <summary>
    /// Fire the weapon.
    /// </summary>
    public override void Fire()
    {
        foreach (var shotSpawn in shotSpawns)
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
        if (fireAnimation.Length > 0 && !playingFireAnimation)
            StartCoroutine(PlayFireAnimation());
    }
}
