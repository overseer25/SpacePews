using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actor data for the player.
/// </summary>
public class PlayerActor : Actor
{
	/// <summary>
	/// Regen tick rate.
	/// </summary>
	public float healthRegenSpeed;

	/// <summary>
	/// Health regen per tick.
	/// </summary>
	public int healthRegenAmount;
}
