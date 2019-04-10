using System.Collections;
using UnityEngine;

public class ChargedWeapon : WeaponComponentBase
{
    // Animations
    public Sprite[] chargingAnimation;
    public Sprite[] chargedAnimationLoop;
    public Sprite[] cooldownAnimation;
    public Sprite[] dechargeAnimation;

    // Sounds
    public AudioClip chargeSound;
    public AudioClip chargedSound;

    // Timing variables
    public float chargedLoopPlayspeed;
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

    // Animation indices
    private int chargingIndex;
    private int chargedIndex;
    private int cooldownIndex;
    private int dechargeIndex;

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
                StartCoroutine(Charge());
                StopCoroutine(CancelCharge());
            }
            else if (charged)
                StartCoroutine(PlayChargedAnimation());
        }
    }

    /// <summary>
    /// Handles canceling the charging.
    /// </summary>
    public void CancelFire()
    {
        if (this != null)
        {
            StopCoroutine(Charge());
            StartCoroutine(CancelCharge());
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
        StopCoroutine(CancelCharge());
        charged = false;
        chargedIndex = 0;
        chargingIndex = 0;
        decharging = false;
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
        if (fireAnimation.Length > 0 && !playingFireAnimation)
            StartCoroutine(PlayFireAnimation());
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
        chargingIndex = 0;
        chargedIndex = 0;
        cooldownIndex = 0;
        dechargeIndex = 0;

        spriteRenderer.sprite = sprites[0];

        StopAllCoroutines();
    }

    /// <summary>
    /// While the player holds the fire button, a charging animation will play. Once that animation has completed, the player may
    /// release the fire button to attack.
    /// </summary>
    private IEnumerator Charge()
    {
        chargingAnimActive = true;
        spriteRenderer.sprite = chargingAnimation[chargingIndex];
        chargingIndex++;
        // Once the animation finishes, exit.
        if (chargingIndex == chargingAnimation.Length)
        {
            chargingIndex--;
            charged = true;
            yield break;
        }
        playChargeSound();

        yield return new WaitForSeconds(chargeSound.length / chargingAnimation.Length);

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
        // Play the decharge animation, if it exists.
        if (dechargeAnimation.Length != 0)
        {
            spriteRenderer.sprite = dechargeAnimation[dechargeIndex];
            yield return new WaitForSeconds(timeToDecharge / dechargeAnimation.Length);
            dechargeIndex++;
            if (dechargeIndex == dechargeAnimation.Length)
            {
                dechargeIndex = 0;
                chargingIndex = 0;
                charged = false;
                decharging = false;
                dechargingActive = false;
                chargingAnimActive = false;
                yield break;
            }
        }
        // Otherwise, just reverse the charging animation.
        else
        {
            spriteRenderer.sprite = chargingAnimation[chargingIndex];
            yield return new WaitForSeconds(timeToDecharge / chargingAnimation.Length);
            if (chargingIndex == 0)
            {
                charged = false;
                decharging = false;
                dechargingActive = false;
                chargingAnimActive = false;
                yield break;
            }
            chargingIndex--;
        }
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
        spriteRenderer.sprite = chargedAnimationLoop[chargedIndex];
        yield return new WaitForSeconds(chargedLoopPlayspeed);
        chargedAnimActive = false;
        chargedIndex++;
        if (chargedIndex == chargedAnimationLoop.Length)
            chargedIndex = 0;
    }

    /// <summary>
    /// Play the cooldown animation.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Cooldown()
    {
        if (cooldownAnimation.Length <= 1)
        {
            coolingDown = false;
            yield break;
        }

        if (cooldownAnimActive)
            yield break;

        cooldownAnimActive = true;
        spriteRenderer.sprite = cooldownAnimation[cooldownIndex];
        yield return new WaitForSeconds(timeToCooldown / cooldownAnimation.Length);

        cooldownAnimActive = false;
        cooldownIndex++;
        if (cooldownIndex == cooldownAnimation.Length)
        {
            cooldownIndex = 0;
            coolingDown = false;
            spriteRenderer.sprite = sprites[0];
            yield break;
        }
    }
}
