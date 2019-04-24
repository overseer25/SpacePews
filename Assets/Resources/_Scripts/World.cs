using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
	private Tilemap grid;
	public TerrainTile asteroidTileSet;

	private const int ASTEROID_MAXHEIGHT = 50;
	private const int ASTEROID_MINHEIGHT = 10;
	private const int ASTEROID_MAXWIDTH = 100;
	private const int ASTEROID_MINWIDTH = 20;
	private const int gridX = 100;
	private const int gridY = 100;

	private void Start()
	{
		grid = GetComponentInChildren<Tilemap>();
		GenerateAsteroids(1);
	}

	public void GenerateAsteroids(int amount)
	{
		for(int i = 0; i < amount; i++)
		{
			int x = Random.Range(0, gridX);
			int y = Random.Range(0, gridY);
			int width = Random.Range(ASTEROID_MINWIDTH, ASTEROID_MAXWIDTH);
			int height = Random.Range(ASTEROID_MINHEIGHT, ASTEROID_MAXHEIGHT);
			GenerateAsteroid(x, y, width, height);
		}
	}

	public void GenerateAsteroid(int x, int y, int width, int height)
	{
		var positions = new List<Vector3Int>();
		var position = Vector3Int.zero;
		for (int i = x - width/2; i <= x + width/2; i++)
		{
			for(int j = y - height/2; j <= y + height/2; j++)
			{
				position.Set(i, j, 0);
				if (grid.GetTile(position) != null)
					return;
				else
					positions.Add(position);
			}
		}

		foreach(var pos in positions)
			grid.SetTile(pos, asteroidTileSet);

	}
}
