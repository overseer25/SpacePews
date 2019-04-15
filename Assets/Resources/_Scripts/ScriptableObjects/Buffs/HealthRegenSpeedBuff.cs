using System.Collections;
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

	/// <summary>
	/// Build description of buff.
	/// </summary>
	/// <returns></returns>
	public override string BuildDescription()
	{
		string result;
		if (debuff)
			result = "Lengthens health regen delay by " + regenSpeed + " seconds";
		else
			result = "Reduces health regen delay by " + regenSpeed + " seconds";

		return result;
	}

	/// <summary>
	/// String for hovering over buff icon.
	/// </summary>
	/// <returns></returns>
	public override string BuildBuffIconString()
	{
		if (debuff)
			return "Decreased health regen speed";
		return "Increased health regen speed";
	}

	/// <summary>
	/// Build the screen to be used by the info screen.
	/// </summary>
	/// <returns></returns>
	public override string BuildInfoScreenString()
    {
        string result;
        if (debuff)
            result = "Lengthens health regen delay by <color=\"red\">" + regenSpeed + "</color> seconds";
        else
            result = "Reduces health regen delay by <color=\"green\">" + regenSpeed + "</color> seconds";

        return result;
    }
}
