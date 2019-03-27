using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all Actors in the game. Actors can be enemies, players, npcs, and other such things.
/// </summary>
public abstract class Actor : MonoBehaviour
{
	/// <summary>
	/// The max health of the actor.
	/// </summary>
	public int health;

	/// <summary>
	/// Regen tick rate.
	/// </summary>
	public float healthRegenSpeed;

	/// <summary>
	/// Health regen per tick.
	/// </summary>
	public int healthRegenAmount;
}
