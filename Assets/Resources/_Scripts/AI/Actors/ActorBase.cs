using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines base stats for an actor in the game.
/// </summary>
public abstract class ActorBase : ScriptableObject
{
	public string actorName;

	public int health;

	public float physicalResistance;

	public float detectionRange;

	public float detectionDistance;

	public float attackRange;

	public float attackDistance;
}
