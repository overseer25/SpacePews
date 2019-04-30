using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningComponent : ShipComponentBase
{
    public float miningRate;
    public float laserWidth;
    public float laserLength;
    public GameObject lineSpawner;
    public AudioClip miningLaserBase;
    public AudioClip contactBase;
    public AudioClip contactMineable;
    public AudioSource contactAudioSource;
    public AudioSource baseAudioSource;
    public ParticleEffect laserContactParticle;

    private float contactSpriteFrequency = 0.2f; // The frequency at which the contact sprite animation is allowed to play.
    private float timePassed = 0.0f; // Used in conjunction with contactSpriteFrequency to delay sprite spawning.

    private LineRenderer line;
    private bool playingContactAudio;
    private bool playingContactAudioMineable;
    private HarvestableObject currentTarget;
    private DynamicParticle currentMiningParticle;

    protected override void Awake()
    {
        base.Awake();
        itemType = ItemType.Turret;
        baseAudioSource.clip = miningLaserBase;
        line = gameObject.GetComponent<LineRenderer>();
        line.startWidth = laserWidth;
        line.endWidth = laserWidth;
        Shader shader = Shader.Find("Particles/Alpha Blended Premultiply");
        if(shader != null)
            line.material = new Material(shader);
        line.positionCount = 2;
    }

    // Update is called once per frame
    public void Fire()
    {
        line.enabled = true;
        // Raycast for detecting collision/ "~(1 << 5) is a LayerMask. Layer 5 is the UI layer, and we don't want the raycast detecting the UI."
        int mask = 1 << LayerMask.NameToLayer("UI");
        mask |= 1 << LayerMask.NameToLayer("Item");
        mask |= 1 << LayerMask.NameToLayer("ShipComponent");
        mask = ~mask;
        RaycastHit2D hit = Physics2D.Raycast(lineSpawner.transform.position, lineSpawner.transform.up, laserLength, mask);

        if (!baseAudioSource.isPlaying)
        {
            baseAudioSource.Play();
        }

        // Laser is contacting something.
        if (hit)
        {
            switch (hit.collider.tag)
            {
                case "Enemy":
                case "Immovable":
                case "Shop":
                case "Asteroid":
                    UpdateLinePosition(lineSpawner.transform.up * Vector3.Distance(lineSpawner.transform.position, hit.point));
                    if (Time.time > timePassed)
                    {
                        timePassed = Time.time + contactSpriteFrequency;
                        ShowContactSprite(hit.point, lineSpawner.transform.rotation);
                    }
                    PlayContactAudio();
                    break;
                case "Mineable": // This should only do something special for mining lasers

                    UpdateLinePosition(lineSpawner.transform.up * Vector3.Distance(lineSpawner.transform.position, hit.point));
                    if (Time.time > timePassed)
                    {
                        timePassed = Time.time + contactSpriteFrequency;
                        ShowContactSprite(hit.point, lineSpawner.transform.rotation);
                    }
                    PlayContactAudio();
                    var objectHit = hit.collider.GetComponent<HarvestableObject>();
                    if(currentTarget != objectHit)
                    {
                        if(currentMiningParticle != null)
                            currentMiningParticle.Stop();
                        currentTarget = objectHit;
                        currentTarget.CancelMining();
                        currentMiningParticle = Instantiate(objectHit.miningParticle);
                        currentMiningParticle.Play();
                    }
                    currentTarget.Mine(miningRate);
                    if (currentMiningParticle != null)
                    {
                        currentMiningParticle.Move(hit.point, transform.rotation.eulerAngles.z);
                    }
                    break;
                default:
                    UpdateLinePosition(lineSpawner.transform.up * laserLength);
                    // Stop contact sound
                    StopContactAudio();
                    break;
            }

        }
        // Laser is not contacting anything
        else
        {
            ResetCurrentTarget();
            StopContactAudio();
            UpdateLinePosition(lineSpawner.transform.up * laserLength);
        }
    }

    /// <summary>
    /// Show the contact sprite effect.
    /// </summary>
    private void ShowContactSprite(Vector2 pos, Quaternion rot)
    {
		ParticleManager.PlayParticle(laserContactParticle, pos, rot);
    }

    private void ResetCurrentTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.CancelMining();
            currentTarget = null;
        }

        if (currentMiningParticle != null)
            currentMiningParticle.Stop();
    }

    /// <summary>
    /// Deactivate the laser.
    /// </summary>
    public void StopFire()
    {
        ResetCurrentTarget();
        if(line != null)
        {
            line.enabled = false;
            if (baseAudioSource.isPlaying)
            {
                baseAudioSource.Stop();
            }
            StopContactAudio();
        }
    }

    /// <summary>
    /// Get the mining rate of this mining laser.
    /// </summary>
    /// <returns></returns>
    public float GetMiningRate()
    {
        return miningRate;
    }

    /// <summary>
    /// Update the position of the line.
    /// </summary>
    private void UpdateLinePosition(Vector3 endPoint)
    {
        // Draw line to range
        line.SetPosition(0, lineSpawner.transform.position);
        line.SetPosition(1, lineSpawner.transform.position + endPoint);
    }

    /// <summary>
    /// Is the mining laser active?
    /// </summary>
    public bool IsActive()
    {
        return line.enabled;
    }

    /// <summary>
    /// Plays the appropriate mining sound effect.
    /// </summary>
    private void PlayContactAudio()
    {
        // Play contact audio
        if (!playingContactAudio)
        {
            contactAudioSource.Stop();
            contactAudioSource.clip = contactBase;
            contactAudioSource.Play();
            playingContactAudio = true;
        }
        // If playing mining audio, turn it off
        if (playingContactAudioMineable)
        {
            contactAudioSource.Stop();
            contactAudioSource.clip = contactMineable;
            contactAudioSource.Play();
            playingContactAudioMineable = false;
        }
    }

    /// <summary>
    /// Stop the audio.
    /// </summary>
    private void StopContactAudio()
    {
        // Stop contact sound
        contactAudioSource.Stop();
        playingContactAudio = false;
        playingContactAudioMineable = false;
    }
}
