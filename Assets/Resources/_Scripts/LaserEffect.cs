using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffect : MonoBehaviour
{

    private float interactionRange = 15.0f;
    private float width = 0.02f;
    private LineRenderer line;
    private GameObject player;
    public GameObject laserContactSprite;
    public AudioSource laserContactSound;
    public AudioSource mineableContactSound; // Sound the laser makes when in contact with a mineable object. Used for mining lasers.
    public Gradient laserColors; // Color(s) of the laser
    public bool isMiningLaser = false;

    private float contactSpriteFrequency = 0.2f; // The frequency at which the contact sprite animation is allowed to play.
    private float timePassed = 0.0f; // Used in conjunction with contactSpriteFrequency to delay sprite spawning.


    private bool playingContactAudio = false;
    private bool playingContactAudioMineable = false;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        line = GetComponent<LineRenderer>();

        line.colorGradient = laserColors;
        line.startWidth = width;
        line.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
    }

    // Update is called once per frame
    void Update()
    {
        // If laser is disabled, don't do raycasting.
        if(line.enabled)
        {

            // Raycast for detecting collision/ "~(1 << 5) is a LayerMask. Layer 5 is the UI layer, and we don't want the raycast detecting the UI."
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, interactionRange, ~(1 << 5));

            // Laser is contacting something.
            if (hit)
            {
                line.colorGradient = laserColors; // Set colors
                switch (hit.collider.tag)
                {
                    
                    case "Enemy":
                    case "Immovable":
                    case "Shop":
                    case "Asteroid":
                    case "Item":
                        // Draw line to collision
                        line.SetPosition(0, transform.position);
                        line.SetPosition(1, transform.position + (transform.up * Vector3.Distance(transform.position, hit.point)));

                        if(Time.time > timePassed)
                        {
                            timePassed = Time.time + contactSpriteFrequency;
                            Instantiate(laserContactSprite, hit.point, transform.rotation);
                        }

                        // Play contact audio
                        if (!playingContactAudio)
                        {
                            laserContactSound.Play();
                            playingContactAudio = true;
                        }
                        // If playing mining audio, turn it off
                        if (playingContactAudioMineable)
                        {
                            mineableContactSound.Stop();
                            playingContactAudioMineable = false;
                        }

                        break;
                    case "Mineable": // This should only do something special for mining lasers

                        if(isMiningLaser)
                        {
                            // Draw line to collision
                            line.SetPosition(0, transform.position);
                            line.SetPosition(1, transform.position + (transform.up * Vector3.Distance(transform.position, hit.point)));

                            if (Time.time > timePassed)
                            {
                                timePassed = Time.time + contactSpriteFrequency;
                                Instantiate(laserContactSprite, hit.point, transform.rotation);
                            }

                            // if playing regular contact audio, turn it off.
                            if (playingContactAudio)
                            {
                                laserContactSound.Stop();
                                playingContactAudio = false;
                            }

                            // Play mineable contact audio
                            if (!playingContactAudioMineable)
                            {
                                mineableContactSound.Play();
                                playingContactAudioMineable = true;
                            }

                            Mineable objectHit = hit.transform.gameObject.GetComponent<Mineable>();
                            MineResource(objectHit); // Mine the resource.
                        }     
                        break;
                    default:
                        // Draw line to range
                        line.SetPosition(0, transform.position);
                        line.SetPosition(1, transform.position + (transform.up * interactionRange));
                        // Stop contact sound
                        if (playingContactAudio)
                        {
                            laserContactSound.Stop();
                            playingContactAudio = false;
                        }
                        // Stop mining sound
                        if (playingContactAudioMineable)
                        {
                            mineableContactSound.Stop();
                            playingContactAudioMineable = false;
                        }
                        break;
                }
                
            }
            // Laser is not contacting anything
            else
            {
                // Draw line to range
                line.SetPosition(0, transform.position);
                line.SetPosition(1, transform.position + (transform.up * interactionRange));
                // Stop contact sound
                if (playingContactAudio)
                {
                    laserContactSound.Stop();
                    playingContactAudio = false;
                }
                // Stop mining sound
                if (playingContactAudioMineable)
                {
                    mineableContactSound.Stop();
                    playingContactAudioMineable = false;
                }
            }
        }
        // If laser is disabled
        else
        {
            // If Laser is shut off, disable the contact audio, if playing.
            if (playingContactAudio)
            {
                laserContactSound.Stop();
                playingContactAudio = false;
            }
            // stop mining sound
            if (playingContactAudioMineable)
            {
                mineableContactSound.Stop();
                playingContactAudioMineable = false;
            }
        }


    }


    private float mineTime = 0.0f;
    private int counter = 0; // Counts the number of resources pulled thus far. Destroy object when counter = objectHit.amount.

    /// <summary>
    /// Mines the resource.
    /// </summary>
    /// <param name="objectHit"></param>
    void MineResource(Mineable objectHit)
    {
        if(Time.time > mineTime)
        {
            mineTime = Time.time + objectHit.mineRate;
            counter++;
        }
        if(counter >= objectHit.amount)
        {
            for(int i = 0; i < objectHit.amount; i++)
            {
                Instantiate(objectHit.resource, objectHit.transform.position + new Vector3(Random.insideUnitCircle.x * 0.5f, Random.insideUnitCircle.y * 0.5f, 0), Quaternion.identity);
            }
            Instantiate(objectHit.explosion, objectHit.transform.position, Quaternion.identity);

            Destroy(objectHit.gameObject);
            counter = 0;
        }
    }
}
