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

	private int multiplierAmount;

	/// <summary>
	/// Apply the health buff/debuff to the provided actor.
	/// </summary>
	/// <param name="actor"></param>
	public override void Apply(Actor actor)
	{
		if (isMultiplier)
		{
			multiplierAmount = (int)(actor.baseHealth * multiplier) - actor.baseHealth;
			actor.health += multiplierAmount;
		}
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
			actor.health -= multiplierAmount;
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
			if (multiplier > 1.0f)
				result = "Boosts base health by " + (int)((multiplier - 1) * 100) + "%.";
			else
				result = "Cuts base health by " + (int)((1 - multiplier) * 100) + "%.";
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
			if(multiplier > 1.0f)
				result = "Boosts base health by <color=\"green\">" + (int)((multiplier - 1) * 100) + "%</color>.";
			else
				result = "Cuts base health by <color=\"red\">" + (int)((1 - multiplier) * 100) + "%</color>.";
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
