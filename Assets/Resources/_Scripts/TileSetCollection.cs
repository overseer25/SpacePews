using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetCollection : MonoBehaviour
{
	public static TileSetCollection current;

	public TileData stone;

	void Awake()
	{
		if (current == null)
			current = this;
	}
}
