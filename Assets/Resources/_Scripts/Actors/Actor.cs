using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all Actors in the game. Actors can be enemies, players, npcs, and other such things.
/// </summary>
public abstract class Actor : MonoBehaviour
{
	public int health;
}
