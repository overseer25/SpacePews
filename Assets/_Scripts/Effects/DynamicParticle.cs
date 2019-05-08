using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A <see cref="DynamicParticle"/> is different from a <see cref="ParticleEffect"/> in that a <see cref="DynamicParticle"/> utilizes
/// the built-in Unity particle system to spawn sprites dynamically, rather than playing a set animation.
/// 
/// This type of particle is instantiated, not pooled, as I don't forsee enough of these occuring to really get a performance boost from pooling.
/// </summary>
public class DynamicParticle : MonoBehaviour
{
    public AudioClip[] possibleSoundEffects;
    private ParticleSystem emitter;
    private AudioSource audioSource;

    /// <summary>
    /// Play the emitter.
    /// </summary>
    public void Play()
    {
        emitter = GetComponent<ParticleSystem>();
        if (emitter.isPlaying)
            return;

        audioSource = GetComponent<AudioSource>();
        if (emitter == null || audioSource == null)
        {
            Debug.LogError("Dynamic Particle " + this + " does not contain either a particle system or audio source.");
            Destroy(gameObject);
            return;
        }
        var clip = possibleSoundEffects[Random.Range(0, possibleSoundEffects.Length - 1)];
        if(clip != null)
            audioSource.PlayOneShot(clip);
        emitter.Play();

        // If it is not a looping particle effect, destroy it after time.
        if(!emitter.main.loop)
            StartCoroutine(DestroyAfterTime(emitter.main.duration));
    }

    /// <summary>
    /// Stop and destroy the emitter.
    /// </summary>
    public void Stop()
    {
        emitter.Stop();
        StartCoroutine(DestroyAfterTime(emitter.main.startLifetime.constantMax)); // Let the particles finish their lifespan before destroying.
    }

    /// <summary>
    /// Move the particle to a new position and rotation.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void Move(Vector2 position, float rotation)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
