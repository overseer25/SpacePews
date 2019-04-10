using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticWeapon : WeaponComponentBase
{
    private bool waitingForFire;
    private Coroutine waitForFireRoutine;
    /// <summary>
    /// Fire the weapon if it is ready to fire the next round.
    /// </summary>
    public override void CheckFire()
    {
        if (!waitingForFire)
        {
            Fire();
            waitForFireRoutine = StartCoroutine(WaitForFire());
        }
    }

    private IEnumerator WaitForFire()
    {
        waitingForFire = true;
        yield return new WaitForSeconds(firerate);
        waitingForFire = false;
    }

    /// <summary>
    /// Fire the weapon.
    /// </summary>
    public override void Fire()
    {
        foreach (var shotSpawn in shotSpawns)
        {
            var projectile = ProjectilePool.current.GetPooledObject() as Projectile;
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
        StartCoroutine(PlayFireAnimation());
    }
}
