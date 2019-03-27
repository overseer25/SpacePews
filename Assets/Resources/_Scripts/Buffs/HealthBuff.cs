using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives the entity a buff/debuff to their health, either as a multiplier or static value.
/// </summary>
[CreateAssetMenu(menuName = "Buffs/Health Buff")]
public class HealthBuff : Buff
{
	public bool isMultiplier; // Determines whether this is a multiplier or a static value.

	public int healthAmount;
	public float multiplier;

	/// <summary>
	/// Apply the health buff/debuff to the provided actor.
	/// </summary>
	/// <param name="actor"></param>
	public override void Apply(Actor actor)
	{
		if(isMultiplier)
			actor.health = (int)(actor.health * multiplier);
		else
			actor.health = (debuff) ? (actor.health - healthAmount) : (actor.health + healthAmount);
	}

	/// <summary>
	/// Remove the health buff/debuff to the provided actor.
	/// </summary>
	/// <param name="actor"></param>
	public override void Remove(Actor actor)
	{
		if (isMultiplier)
			actor.health = (int)(actor.health / multiplier);
		else
			actor.health = (debuff) ? (actor.health + healthAmount) : (actor.health - healthAmount);
	}
}
