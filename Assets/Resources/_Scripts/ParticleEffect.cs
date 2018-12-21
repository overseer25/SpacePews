using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour {

    public Sprite[] particleSprites;
    public float playspeed = 0.1f;
    private float changeSprite = 0.0f;
    private int index = 0;
    [SerializeField]
    private AudioClip sound;

    private void OnEnable()
    {
        var explosionSound = GetComponent<AudioSource>();
        explosionSound.PlayOneShot(sound);
        index = 0;
    }

	// Update is called once per frame
	void Update ()
    {
	    if(Time.time > changeSprite)
        {
            if (index >= particleSprites.Length) { gameObject.SetActive(false); }
            else
            {
                changeSprite = Time.time + playspeed;
                GetComponentInChildren<SpriteRenderer>().sprite = particleSprites[index];
                index++;
            }     
        }
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
}
