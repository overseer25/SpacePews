using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a pool that will manage the maximum number of particles that can exist in the world.
/// If the maximum number of particles is reached, and more particles are being spawned, the pool will need
/// to clear out the oldest particles in the world, and replace them with the newly spawned ones.
/// </summary>
public class ParticlePool : BasePool
{

    public static ParticlePool current;

    // The object pooled.
    public ParticleEffect defaultParticle;

    // The internal list of objects.
    private List<ParticleEffect> particlePool;

    private void Awake()
    {
        particlePool = new List<ParticleEffect>();
        current = this;
    }

	/// <summary>
	/// Set the size of the pool.
	/// </summary>
	public override void SetPoolSize(int size)
	{
		if (particlePool == null)
			particlePool = new List<ParticleEffect>();

		if (size == poolSize)
			return;

		int difference = poolSize - size;

		if (size > poolSize)
		{
			// Add on to the list.
			for (int i = poolSize; i < size; i++)
			{
				var particle = Instantiate(defaultParticle, gameObject.transform);
				particle.DisableEffect();
				particlePool.Add(particle);
			}
		}
		else
		{
			// Subtract from the list.
			for (int i = poolSize - 1; i >= poolSize - difference; i--)
			{
				Destroy(particlePool[particlePool.Count - 1].gameObject);
				particlePool.RemoveAt(particlePool.Count - 1);
			}
		}

		poolSize = particlePool.Count;
	}

	/// <summary>
	/// Gets an unused object in the object pool, if it exists.
	/// </summary>
	/// <returns> Unused GameObject in object pool, or the oldest active one (LRU). </returns>
	public override object GetPooledObject()
    {
        if (particlePool == null)
            return null;

        foreach (var particle in particlePool)
        {
            if (particle != null && particle.IsFree())
            {
				return particle;
            }
        }
		// Otherwise, get the least recently used effect in the pool.
		var result = particlePool[oldest++];
		oldest %= poolSize;
		return result;
	}
}
