using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningComponent : ShipComponent
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
    public ParticleEffect laserContactSprite;

    private float contactSpriteFrequency = 0.2f; // The frequency at which the contact sprite animation is allowed to play.
    private float timePassed = 0.0f; // Used in conjunction with contactSpriteFrequency to delay sprite spawning.

    private LineRenderer line;
    private bool playingContactAudio;
    private bool playingContactAudioMineable;

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

                    Mineable objectHit = hit.transform.gameObject.GetComponent<Mineable>();
                    MineResource(objectHit); // Mine the resource.
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
            StopContactAudio();
            UpdateLinePosition(lineSpawner.transform.up * laserLength);
        }
    }

    /// <summary>
    /// Show the contact sprite effect.
    /// </summary>
    private void ShowContactSprite(Vector2 pos, Quaternion rot)
    {
        ParticleEffect exp = ParticlePool.current.GetPooledObject();
        exp.Copy(laserContactSprite);
        exp.SetTransform(pos, rot);
        exp.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivate the laser.
    /// </summary>
    public void StopFire()
    {
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

    /// <summary>
    /// Mines the resource.
    /// </summary>
    /// <param name="objectHit"></param>
    void MineResource(Mineable objectHit)
    {
        //All out of resources, return
        if (objectHit.depleted)
        {
            return;
        }
        //otherwise still resources to be had, spawn us some if we are over the time of being able to
        float currentMineTime = objectHit.AddTimeMined(Time.deltaTime * miningRate);
        if (currentMineTime >= objectHit.mineRate)
        {
            objectHit.SpawnLoot(GetComponentInParent<MovementController>().gameObject);
        }
    }
}
