using System;
using System.Collections;
using UnityEngine;

public class ChargedWeapon : WeaponComponent
{
    public Sprite[] chargingAnimation;
    public Sprite[] chargedAnimationLoop;
    public Sprite[] cooldownAnimation;
    public float chargedLoopPlayspeed;
    public float timeToCooldown;
    public float timeToCharge;
    public float cooldown;
    public bool cooldownActive;
    private bool cooldownAnimActive;
    private bool chargedAnimActive;
    private bool chargingAnimActive;
    private bool dechargingActive;
    public bool charged;
    public bool charging;
    public bool decharging;
    public AudioClip chargeSound;

    private int chargingIndex;
    private int chargedIndex;
    private int cooldownIndex;

    protected override void Update()
    {
        if (!charging && !playingFireAnimation && !cooldownActive)
            base.Update();
    }

    /// <summary>
    /// While the player holds the fire button, a charging animation will play. Once that animation has completed, the player may
    /// release the fire button to attack.
    /// </summary>
    public IEnumerator Charge()
    {
        if (charging)
            yield break;

        if(!audioSource.isPlaying)
            audioSource.PlayOneShot(chargeSound);

        charging = true;
        spriteRenderer.sprite = chargingAnimation[chargingIndex];
        yield return new WaitForSeconds(timeToCharge / chargingAnimation.Length);
        charging = false;
        chargingIndex++;
        // Once the animation finishes, exit.
        if (chargingIndex == chargingAnimation.Length)
        {
            charged = true;
            audioSource.Stop();
            yield break;
        }
    }

    /// <summary>
    /// Cancel the charge. This can occur when the player releases the fire button before it is fully charged.
    /// </summary>
    public IEnumerator CancelCharge()
    {
        if (chargingIndex > 0)
        {
            audioSource.Stop();
        }
        
        decharging = true;

            spriteRenderer.sprite = chargingAnimation[chargingIndex];
        dechargingActive = true;
        yield return new WaitForSeconds(timeToCharge / chargingAnimation.Length);
        dechargingActive = false;
        if (chargingIndex == 0)
        {
            decharging = false;
            yield break;
        }
        chargingIndex--;
    }

    /// <summary>
    /// Once the weapon has been charged, play the charged animation loop.
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayChargedAnimation()
    {
        if (!charged || chargedAnimActive)
            yield break;

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
    public IEnumerator Cooldown()
    {
        if(cooldownAnimation.Length <= 1)
        {
            cooldownActive = false;
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
            cooldownActive = false;
            spriteRenderer.sprite = sprites[0];
            yield break;
        }
    }

    /// <summary>
    /// Fires the charged weapon.
    /// </summary>
    public override void Fire()
    {
        audioSource.Stop();
        StopCoroutine(CancelCharge());
        base.Fire();
        charged = false;
        chargedIndex = 0;
        chargingIndex = 0;
        decharging = false;
        cooldownActive = true;
        chargedAnimActive = false;
    }
}
