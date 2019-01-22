using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunComponent : WeaponComponent
{
    // The number of rounds to make up the shotgun spread.
    [SerializeField]
    internal int numberOfRounds;
    [SerializeField]
    internal int ShotSpeedMin;

    /// <summary>
    /// Fire the shotgun projectile.
    /// </summary>
    /// <param name="time"> The current time that has passed. If the</param>
    public override void Fire()
    {
        for(int i = 0; i < numberOfRounds; i++)
        {
            var projectile = GetComponent<ProjectilePool>().GetPooledObject();
            if (projectile == null)
                continue;
            var damage = ComputeDamage();
            var speed = UnityEngine.Random.Range(ShotSpeedMin, shotSpeed);
            projectile.GetComponent<Projectile>().Initialize(damage, speed, (damage > maxDamage) ? true : false);
            projectile.transform.position = shotSpawn.transform.position;
            Quaternion rotation = shotSpawn.transform.transform.rotation;
            var angle = UnityEngine.Random.Range(-shotSpread, shotSpread);
            rotation *= Quaternion.Euler(0, 0, angle);
            projectile.transform.rotation = rotation;
            projectile.SetActive(true);
        }
       
        audioSource.clip = projectile.GetComponent<Projectile>().GetFireSound();
        audioSource.Play();
    }
}
