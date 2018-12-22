using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains methods used to interact with the ParticleEffect pool. Other scripts should not directly
/// interface with the ParticleEffect pool.
/// </summary>
public class ParticleManager : MonoBehaviour
{
    /// <summary>
    /// Allocates and plays the specified particle effect.
    /// </summary>
    public static void PlayParticle(ParticleEffect particle, GameObject position)
    {
        var audioSource = particle.GetComponent<AudioSource>();
        var exp = ParticlePool.current.GetPooledObject();
        exp.Copy(particle);
        exp.SetTransform(position);
        if (audioSource == null)
            Debug.LogError(particle + " does not contain an audio source.");
        else if(particle.GetSound() != null)
            particle.GetComponent<AudioSource>().PlayOneShot(particle.GetSound());
        exp.gameObject.SetActive(true);
    }
}
