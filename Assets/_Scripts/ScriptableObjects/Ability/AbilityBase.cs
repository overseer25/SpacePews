using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic class used for designing a player's active ability. Upgrade components that the player collects can give them
/// an active ability.
/// </summary>
public abstract class AbilityBase : ScriptableObject, IEquatable<AbilityBase>
{
	public string abilityName;
    public ParticleEffect useEffect;
	public float cooldownTime;
	[HideInInspector]
	public bool recharging = false;

	private float time;

	/// <summary>
	/// Activate the ability.
	/// </summary>
	/// <param name="player">The player receiving the ability.</param>
	public abstract void Activate(GameObject player);

	/// <summary>
	/// Get the amount of time remaining on the cooldown.
	/// </summary>
	/// <returns></returns>
	public float GetCooldownTimeRemaining()
	{
		if (!recharging)
			return 0.0f;
		return time / cooldownTime;
	}

	/// <summary>
	/// Begin the cooldown for the ability. The ability cannot be used again until the cooldown is complete.
	/// </summary>
	/// <returns></returns>
	public IEnumerator Cooldown()
	{
		recharging = true;
		for (time = cooldownTime; time > 0; time -= Time.deltaTime)
			yield return null;
		recharging = false;
	}

	/// <summary>
	/// Override for the Equals method.
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Equals(AbilityBase other)
	{
		return this == other;
	}

	/// <summary>
	/// Override for the == operator.
	/// </summary>
	/// <param name="a1"></param>
	/// <param name="a2"></param>
	/// <returns></returns>
	public static bool operator ==(AbilityBase a1, AbilityBase a2)
	{
		if (ReferenceEquals(a1, a2))
			return true;
		else if (ReferenceEquals(a1, null))
			return false;
		else if (ReferenceEquals(a2, null))
			return false;

		return a1.abilityName == a2.abilityName;
	}

	/// <summary>
	/// Override for the != operator.
	/// </summary>
	/// <param name="a1"></param>
	/// <param name="a2"></param>
	/// <returns></returns>
	public static bool operator !=(AbilityBase a1, AbilityBase a2)
	{
		return !(a1 == a2);
	}
}
