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
		if (isMultiplier)
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

	/// <summary>
	/// Build description of buff.
	/// </summary>
	/// <returns></returns>
	public override string BuildDescription()
	{
		string result;
		if (isMultiplier)
		{
			result = "Multiplies an entity's health by " + multiplier + "x.";
		}
		else
		{
			if (debuff)
				result = "Decreases ";
			else
				result = "Increases ";
			result += " an entity's health by " + healthAmount + " points";
		}
		return result;
	}

    /// <summary>
    /// Build the screen to be used by the info screen.
    /// </summary>
    /// <returns></returns>
    public override string BuildInfoScreenString()
    {
        string result;
        if (isMultiplier)
        {
            result = "Multiplies health by <color=\"green\">" + multiplier + "x</color>.";
        }
        else
        {
            if (debuff)
                result = "Decreases health by <color=\"red\">" + healthAmount + "</color> points";
            else
                result = "Increases health by <color=\"green\">" + healthAmount + "</color> points";
        }
        return result;
    }
}
