using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{

    public Sprite[] particleSprites;
    public float playspeed = 0.1f;
    private float changeSprite = 0.0f;
    private int index = 0;
    private bool expired;
    [SerializeField]
    private AudioClip sound;

    private void OnEnable()
    {
        var audioSource = GetComponent<AudioSource>();
        if (audioSource != null && sound != null)
            audioSource.PlayOneShot(sound);
        index = 0;
        expired = false;
    }

    /// <summary>
    /// Get the sound associated with this particle effect.
    /// </summary>
    /// <returns></returns>
    public AudioClip GetSound()
    {
        return sound;
    }

    // Update is called once per frame
    void Update()
    {
        if (!expired)
        {
            if (Time.time > changeSprite)
            {
                if (index >= particleSprites.Length)
                {
                    if(sound != null)
                        StartCoroutine(WaitToDisable());
                    else
                        gameObject.SetActive(false);
                }
                else
                {
                    changeSprite = Time.time + playspeed;
                    GetComponentInChildren<SpriteRenderer>().sprite = particleSprites[index];
                    index++;
                }
            }
        }
    }

    /// <summary>
    /// Wait to disable the Particle Effect until the sound finishes playing.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitToDisable()
    {
        expired = true;
        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(sound.length);

        gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Copy the values of another ParticleEffect.
    /// </summary>
    /// <param name="other"></param>
    public void Copy(ParticleEffect other)
    {
        particleSprites = other.particleSprites;
        playspeed = other.playspeed;
        changeSprite = other.changeSprite;
        index = other.index;
        sound = other.sound;
    }

    /// <summary>
    /// Set the position and rotation of the effect, based on the target.
    /// </summary>
    public void SetTransform(GameObject target)
    {
        transform.position = target.transform.position;
        transform.rotation = target.transform.rotation;
    }

    /// <summary>
    /// Set the position and rotation of the effect, based on the given position and rotation.
    /// </summary>
    public void SetTransform(Vector2 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}
