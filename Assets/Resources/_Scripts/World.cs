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
	private const int gridX = 500;
	private const int gridY = 500;

	private void Start()
	{
		grid = GetComponentInChildren<Tilemap>();
		GenerateAsteroids(3);
	}

	public void GenerateAsteroids(int amount)
	{
		for(int i = 0; i < amount; i++)
		{
			int x = Random.Range(-gridX/2, gridX/2);
			int y = Random.Range(-gridY/2, gridY/2);
			int width = Random.Range(ASTEROID_MINWIDTH, ASTEROID_MAXWIDTH);
			int height = Random.Range(ASTEROID_MINHEIGHT, ASTEROID_MAXHEIGHT);
			GenerateAsteroid(x, y, width, height);
		}
	}

	public void GenerateAsteroid(int x, int y, int width, int height)
	{
		var positions = new List<Vector3Int>();
		var position = Vector3Int.zero;
		int amplitude = 1;
		bool reverseAmplitude = false;
		for (int i = x - width/2; i <= x + width/2; i++)
		{
			float noise = Mathf.PerlinNoise((float)i / height, x) * amplitude;
			float lowerNoise = Mathf.PerlinNoise((float)i / height, y) * amplitude;
			int yCoord = y + (int)noise;
			position.Set(i, yCoord, 0);
			positions.Add(position);
			while(yCoord >= y - (int)lowerNoise)
			{
				yCoord--;
				position.Set(i, yCoord, 0);
				positions.Add(position);
			}
			if (amplitude >= width / 2)
				reverseAmplitude = true;

			if (!reverseAmplitude)
				amplitude++;
			else
				amplitude--;
			//for(int j = y - height/2; j <= y + height/2; j++)
			//{
			//	position.Set(i, j, 0);
			//	if (grid.GetTile(position) != null)
			//		return;
			//	else
			//		positions.Add(position);
			//}
		}

		foreach(var pos in positions)
			grid.SetTile(pos, asteroidTileSet);

	}
}
