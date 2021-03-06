﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff for modifying the regen speed of the actor.
/// </summary>
[CreateAssetMenu(menuName = "Buffs/Health Regen Speed Buff")]
public class HealthRegenSpeedBuff : Buff
{

	public float regenSpeed;

	/// <summary>
	/// Add buff to actor.
	/// </summary>
	/// <param name="actor"></param>
	public override void Apply(Actor actor)
	{
		if (debuff)
			actor.healthRegenSpeed += regenSpeed;
		else
			actor.healthRegenSpeed -= regenSpeed;
	}

	/// <summary>
	/// Remove buff frome actor.
	/// </summary>
	/// <param name="actor"></param>
	public override void Remove(Actor actor)
	{
		if (debuff)
			actor.healthRegenSpeed -= regenSpeed;
		else
			actor.healthRegenSpeed += regenSpeed;
	}
}
