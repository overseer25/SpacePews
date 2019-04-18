using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a block in the world. ID must be unique.
/// </summary>
public class WorldTile : MonoBehaviour
{
	public Sprite tile;
	public bool mineable;
	public bool destroyable;

	private void Start()
	{
		GetComponent<SpriteRenderer>().sprite = tile;
	}

	/// <summary>
	/// Copy data of one World Tile to another.
	/// </summary>
	/// <param name="other"></param>
	public void Copy(WorldTile other)
	{
		mineable = other.mineable;
		destroyable = other.destroyable;
		tile = other.tile;
		GetComponent<SpriteRenderer>().sprite = tile;
	}
}
