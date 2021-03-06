﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Contains methods used to interact with the ParticleEffect pool. Other scripts should not directly
/// interface with the ParticleEffect pool.
/// </summary>
public class ParticleManager : MonoBehaviour
{
	private static ParticleManager current;

    /// <summary>
    /// Allocates and plays the specified particle effect.
    /// </summary>
    public static void PlayParticle(ParticleEffect particle, Vector2 position, Quaternion? rotation = null)
    {
        var part = ParticlePool.current.GetPooledObject() as ParticleEffect;
        if (part == null)
            return;

        part.Copy(particle);
        if (rotation == null)
            part.SetTransform(position);
        else
            part.SetTransform(position, (Quaternion)rotation);
		part.EnableEffect();

    }
}
