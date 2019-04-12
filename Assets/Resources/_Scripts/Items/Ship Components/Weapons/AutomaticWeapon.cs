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

    /// <summary>
    /// Fire the weapon.
    /// </summary>
    public override void Fire()
    {
		if (animating)
		{
			animating = false;
			StopCoroutine(animate);
			animate = null;
		}

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
		if(fireAnimation != null && !playingFireAnimation)
			StartCoroutine(PlayFireAnimation());
    }

	/// <summary>
	/// Play the weapons fire animation, if it exists.
	/// </summary>
	/// <returns></returns>
	private IEnumerator PlayFireAnimation()
	{
		playingFireAnimation = true;

		var frame = fireAnimation.GetNextFrame();
		while (frame != null)
		{
			yield return new WaitForSeconds(fireAnimation.playSpeed);
			frame = fireAnimation.GetNextFrame();
			spriteRenderer.sprite = frame;
		}

		playingFireAnimation = false;

		if (idleAnimation != null && !animating)
		{
			animate = StartCoroutine(Animate());
		}
		else if (itemSprite != null)
		{
			spriteRenderer.sprite = itemSprite;
		}
		fireAnimation.ResetAnimation();
		yield break;
	}

	private IEnumerator WaitForFire()
	{
		waitingForFire = true;
		yield return new WaitForSeconds(firerate);
		waitingForFire = false;
	}
}
