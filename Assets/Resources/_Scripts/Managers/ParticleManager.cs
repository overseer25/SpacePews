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
    public static void PlayParticle(ParticleEffect particle, GameObject position, Quaternion? rotation = null)
    {
        var exp = ParticlePool.current.GetPooledObject();
        if (exp == null)
            return;
        exp.Copy(particle);
        if (rotation == null)
            exp.SetTransform(position);
        else
            exp.SetTransform(position.transform.position, (Quaternion)rotation);
        exp.gameObject.SetActive(true);
    }
}
