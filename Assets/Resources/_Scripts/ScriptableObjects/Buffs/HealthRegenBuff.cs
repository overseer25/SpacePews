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

	/// <summary>
	/// Build description of buff.
	/// </summary>
	/// <returns></returns>
	public override string BuildDescription()
	{
		string result;
        if (disableRegen)
            result = "Disables health regen.";
        else if (debuff)
            result = "Decreases health regen amount by " + regenAmount;
        else
            result = "Increases health regen amount by " + regenAmount;

		return result;
	}

    /// <summary>
    /// Build the screen to be used by the info screen.
    /// </summary>
    /// <returns></returns>
    public override string BuildInfoScreenString()
    {
        string result;
        if (disableRegen)
            result = "Disables health regen.";
        else if (debuff)
            result = "Decreases health regen amount by <color=\"red\">" + regenAmount + "</color>";
        else
            result = "Increases health regen amount by <color=\"green\">" + regenAmount + "</color>";

        return result;
    }
}
