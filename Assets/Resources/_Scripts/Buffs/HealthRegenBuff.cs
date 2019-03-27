using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives the entity a buff/debuff to their health regen.
/// </summary>
[CreateAssetMenu(menuName = "Buffs/Health Regen Amount Buff")]
public class HealthRegenBuff : Buff
{
	public int regenAmount;

	/// <summary>
	/// Add buff to actor.
	/// </summary>
	/// <param name="actor"></param>
	public override void Apply(Actor actor)
	{
		if (debuff)
			actor.healthRegenAmount -= regenAmount;
		else
			actor.healthRegenAmount += regenAmount;
	}

	/// <summary>
	/// Remove buff on actor.
	/// </summary>
	/// <param name="actor"></param>
	public override void Remove(Actor actor)
	{
		if (debuff)
			actor.healthRegenAmount += regenAmount;
		else
			actor.healthRegenAmount -= regenAmount;
	}

	/// <summary>
	/// Build description of buff.
	/// </summary>
	/// <returns></returns>
	public override string BuildDescription()
	{
		string result;
		if (debuff)
			result = "Decreases health regen amount by " + regenAmount;
		else
			result = "Increases health regen amount by " + regenAmount;

		return result;
	}
}
