﻿using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{

    public SpriteAnimation particleSprites;
    [SerializeField]
    private AudioClip sound;
	private AudioSource audioSource;
	private new SpriteRenderer renderer;
	private bool isFree;
	private Coroutine animate;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		renderer = GetComponent<SpriteRenderer>();
	}

    /// <summary>
    /// Get the sound associated with this particle effect.
    /// </summary>
    /// <returns></returns>
    public AudioClip GetSound()
    {
        return sound;
    }

	/// <summary>
	/// Play the animation for the sprite. Once the animation is completed, hide the particle effect.
	/// </summary>
	/// <returns></returns>
	private IEnumerator PlayEffect()
	{
		Sprite frame;
		do
		{
			frame = particleSprites.GetNextFrame();
			renderer.sprite = frame;
			yield return new WaitForSeconds(particleSprites.playSpeed);

		} while (frame != null);

		DisableEffect();
		yield return null;
	}

	/// <summary>
	/// Enable the effect, and allow the animation and sound to play.
	/// </summary>
	public void EnableEffect()
	{
		renderer.enabled = true;
		isFree = false;
		if(sound != null)
			audioSource.PlayOneShot(sound);
		particleSprites.ResetAnimation();
		if (animate != null)
			Debug.Log("The particle effect " + this + " is already playing. The particle effect  must not have been disabled before reuse.");
		else
			animate = StartCoroutine(PlayEffect());
	}

	/// <summary>
	/// This will disable the particle effect, and allow the particle effect pool to reuse it.
	/// </summary>
	public void DisableEffect()
	{
		if(animate != null)
		{
			StopCoroutine(animate);
			animate = null;
		}
		renderer.enabled = false;
		isFree = true;
	}

	/// <summary>
	/// Can this particle effect be freely used by the particle effect pool?
	/// </summary>
	/// <returns></returns>
	public bool IsFree()
	{
		return isFree;
	}

    /// <summary>
    /// Copy the values of another ParticleEffect.
    /// </summary>
    /// <param name="other"></param>
    public void Copy(ParticleEffect other)
    {
        particleSprites = other.particleSprites;
        sound = other.sound;
    }

    /// <summary>
    /// Set the position of the effect, based on the target.
    /// </summary>
    public void SetTransform(Vector2 position)
    {
		SetTransform(position, Quaternion.identity);
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
