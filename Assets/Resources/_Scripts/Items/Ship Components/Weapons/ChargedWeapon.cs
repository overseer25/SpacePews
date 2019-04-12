using System.Collections;
using UnityEngine;

public class ChargedWeapon : WeaponComponentBase
{
	// Animations
	public SpriteAnimation chargingAnimation;
	public SpriteAnimation chargedAnimationLoop;
	public SpriteAnimation cooldownAnimation;

	// Sounds
	public AudioClip chargeSound;
	public AudioClip chargedSound;

	// Timing variables
	public float timeToCooldown;
	public float timeToDecharge;

	// State variables
	private bool coolingDown;
	private bool charged;
	private bool charging;
	private bool decharging;

	// Internal State variables
	private bool cooldownAnimActive;
	private bool chargedAnimActive;
	private bool chargingAnimActive;
	private bool dechargingActive;

	private Coroutine chargingCoroutine;
	private Coroutine cancelCoroutine;
	private Coroutine chargedCoroutine;

	protected override void Update()
	{
		if (!charging && !coolingDown)
			base.Update();
	}

	/// <summary>
	/// Is the weapon currently charging up?
	/// </summary>
	/// <returns></returns>
	public bool IsCharging()
	{
		return charging;
	}

	/// <summary>
	/// Is the weapon decharging (occurs when the player releases the fire button too early)?
	/// </summary>
	/// <returns></returns>
	public bool IsDecharging()
	{
		return decharging;
	}

	/// <summary>
	/// Is the weapon currently charged?
	/// </summary>
	/// <returns></returns>
	public bool IsCharged()
	{
		return charged;
	}

	/// <summary>
	/// Is the weapon cooling down.
	/// </summary>
	/// <returns></returns>
	public bool IsCoolingDown()
	{
		return coolingDown;
	}

	private void OnDisable()
	{
		Revert();
	}

	private void OnDestroy()
	{
		Revert();
	}

	/// <summary>
	/// If the weapon is ready to charge, begin charging it.
	/// </summary>
	public override void CheckFire()
	{
		if (this != null)
		{
			if (!chargingAnimActive && !charged)
			{
				chargingCoroutine = StartCoroutine(Charge());
				if (cancelCoroutine != null)
				{
					StopCoroutine(cancelCoroutine);
					cancelCoroutine = null;
				}
			}
			else if (charged)
				chargedCoroutine = StartCoroutine(PlayChargedAnimation());
		}
	}

	/// <summary>
	/// Handles canceling the charging.
	/// </summary>
	public void CancelFire()
	{
		if (this != null)
		{
			if (chargingCoroutine != null)
			{
				StopCoroutine(chargingCoroutine);
				chargingCoroutine = null; 
			}
			if (chargedCoroutine != null)
			{
				StopCoroutine(chargedCoroutine);
				chargedCoroutine = null; 
			}
			cancelCoroutine = StartCoroutine(CancelCharge());
		}
	}

	/// <summary>
	/// Start the cooldown process.
	/// </summary>
	public void StartCooldown()
	{
		if (this != null)
			StartCoroutine(Cooldown());
	}

	/// <summary>
	/// Fires the charged weapon.
	/// </summary>
	public override void Fire()
	{
		audioSource.Stop();
		audioSource.loop = false;
		charged = false;
		coolingDown = true;
		chargedAnimActive = false;
		chargingAnimActive = false;

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
	}

	/// <summary>
	/// Resets the charged weapon to it's default state. This is called when the player is killed.
	/// </summary>
	private void Revert()
	{
		// State variables
		coolingDown = false;
		charged = false;
		charging = false;
		decharging = false;

		// Internal State variables
		cooldownAnimActive = false;
		chargedAnimActive = false;
		chargingAnimActive = false;
		dechargingActive = false;

		// Animation indices
		chargingAnimation.ResetAnimation();
		chargedAnimationLoop.ResetAnimation();
		cooldownAnimation.ResetAnimation();

		if (animated)
		{
			idleAnimation.ResetAnimation();
			spriteRenderer.sprite = idleAnimation.GetNextFrame();
		}
		else
			spriteRenderer.sprite = itemSprite;

		StopAllCoroutines();
		chargingCoroutine = null;
		cancelCoroutine = null;
		chargedCoroutine = null;
	}

	/// <summary>
	/// While the player holds the fire button, a charging animation will play. Once that animation has completed, the player may
	/// release the fire button to attack.
	/// </summary>
	private IEnumerator Charge()
	{
		chargingAnimActive = true;
		var sprite = chargingAnimation.GetNextFrame();
		// Once the animation finishes, exit.
		if (sprite == null)
		{
			charged = true;
			chargingAnimation.ResetAnimation();
			yield break;
		}
		spriteRenderer.sprite = sprite;
		playChargeSound();
		yield return new WaitForSeconds(chargeSound.length / chargingAnimation.frames.Length);

		chargingAnimActive = false;
	}

	/// <summary>
	/// Play the charge sound once, and begins the delay for the charged sound.
	/// </summary>
	private void playChargeSound()
	{
		if (!audioSource.isPlaying)
		{
			audioSource.PlayOneShot(chargeSound);
			audioSource.clip = chargedSound;
			audioSource.loop = true;
			audioSource.PlayDelayed(chargeSound.length);
		}
	}

	/// <summary>
	/// Cancel the charge. This can occur when the player releases the fire button before it is fully charged.
	/// </summary>
	private IEnumerator CancelCharge()
	{
		if (!decharging)
			audioSource.Stop();
		if (dechargingActive)
			yield break;

		decharging = true;

		dechargingActive = true;
		var sprite = chargingAnimation.GetPreviousFrame();
		if (sprite == null)
		{
			charged = false;
			decharging = false;
			dechargingActive = false;
			chargingAnimActive = false;
			yield break;
		}
		spriteRenderer.sprite = sprite;
		yield return new WaitForSeconds(timeToDecharge / chargingAnimation.frames.Length);
		dechargingActive = false;
	}

	/// <summary>
	/// Once the weapon has been charged, play the charged animation loop.
	/// </summary>
	/// <returns></returns>
	private IEnumerator PlayChargedAnimation()
	{
		if (chargedAnimActive)
		{
			yield break;
		}

		chargedAnimActive = true;
		var sprite = chargedAnimationLoop.GetNextFrame();
		spriteRenderer.sprite = sprite;
		yield return new WaitForSeconds(chargedAnimationLoop.playSpeed);
		chargedAnimActive = false;
	}

	/// <summary>
	/// Play the cooldown animation.
	/// </summary>
	/// <returns></returns>
	private IEnumerator Cooldown()
	{
		if (cooldownAnimActive)
			yield break;

		cooldownAnimActive = true;

		var sprite = cooldownAnimation.GetNextFrame();
		if (sprite == null)
		{
			coolingDown = false;
			cooldownAnimation.ResetAnimation();
			if (animated)
			{
				idleAnimation.ResetAnimation();
				spriteRenderer.sprite = idleAnimation.GetNextFrame();
			}
			else
				spriteRenderer.sprite = itemSprite;

			cooldownAnimActive = false;
			yield break;
		}
		spriteRenderer.sprite = sprite;
		yield return new WaitForSeconds(timeToCooldown / cooldownAnimation.frames.Length);

		cooldownAnimActive = false;
	}
}
