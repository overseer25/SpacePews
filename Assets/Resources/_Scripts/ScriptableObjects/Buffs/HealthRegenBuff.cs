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
    public bool disableRegen;

	/// <summary>
	/// Add buff to actor.
	/// </summary>
	/// <param name="actor"></param>
	public override void Apply(Actor actor)
	{
        if (disableRegen)
            actor.disableHealthRegen = true;
		else if (debuff)
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
		if(disableRegen)
			actor.disableHealthRegen = false;
        if (debuff)
			actor.healthRegenAmount += regenAmount;
		else
			actor.healthRegenAmount -= regenAmount;
	}
}
