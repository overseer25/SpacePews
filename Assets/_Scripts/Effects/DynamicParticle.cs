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
        audioSource = GetComponent<AudioSource>();
        if (emitter == null || audioSource == null)
        {
            Debug.LogError("Dynamic Particle " + this + " does not contain either a particle system or audio source.");
            Destroy(gameObject);
            return;
        }
        var clip = possibleSoundEffects[Random.Range(0, possibleSoundEffects.Length - 1)];
        audioSource.PlayOneShot(clip);
        emitter.Play();
        StartCoroutine(StopAfterTime(emitter.main.duration));
    }

    /// <summary>
    /// Stop and destroy the emitter.
    /// </summary>
    public void Stop()
    {
        emitter.Stop();
        Destroy(gameObject);
    }

    private IEnumerator StopAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
