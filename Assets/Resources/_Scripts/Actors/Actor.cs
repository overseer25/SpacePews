using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all Actors in the game. Actors can be enemies, players, npcs, and other such things.
/// </summary>
public class Actor : MonoBehaviour
{
	/// <summary>
	/// The base health of the actor. This is used for multipliers so that they don't apply/remove 
	/// too much or too little health.
	/// </summary>
	public int baseHealth;

	/// <summary>
	/// The max health of the actor. This is the value that gets modified.
	/// </summary>
	[HideInInspector]
	public int health;

	/// <summary>
	/// Regen tick rate.
	/// </summary>
	public float healthRegenSpeed;

	/// <summary>
	/// Health regen per tick.
	/// </summary>
	public int healthRegenAmount;

    /// <summary>
    /// When true, disables health regeneration on this actor.
    /// </summary>
    public bool disableHealthRegen;

	private void Awake()
	{
		health = baseHealth;
	}

}
