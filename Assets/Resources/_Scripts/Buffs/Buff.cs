using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class used by all Buff/Debuff effects
/// </summary>
public abstract class Buff : ScriptableObject
{
	/// <summary>
	/// Is this enhancing the player's stats, or reducing them?
	/// </summary>
	public bool debuff;

	/// <summary>
	/// Apply the effect of the buff/debuff to the actor.
	/// </summary>
	public abstract void Apply(Actor actor);

	/// <summary>
	/// Remove the buff/debuff from the actor.
	/// </summary>
	/// <param name="actor"></param>
	public abstract void Remove(Actor actor);

	/// <summary>
	/// Build the description of the buff based on the settings.
	/// </summary>
	/// <returns></returns>
	public abstract string BuildDescription();

    /// <summary>
    /// The text to be display in the info screen.
    /// </summary>
    /// <returns></returns>
    public abstract string BuildInfoScreenString();

}
