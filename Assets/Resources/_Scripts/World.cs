using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public static World current;
	public int worldMinX = -100;
	public int worldMaxX = 100;
	public int worldMinY = -100;
	public int worldMaxY = 100;
	public Vector3 gridOffset = new Vector3(0.5f, 0.5f, 0);
	public float gridScale = 1.0f;
	public float tileScale = 3.0f;
	public Color gridColor = Color.grey;
	public Color gridCenterColor = Color.white;
	public bool visualize = true;

	private WorldTile[,] grid;
	private const int ASTEROID_MAX_RADIUS = 25;
	private const int ASTEROID_MIN_RADIUS = 6;

	void Start()
	{
		if (current == null)
		{
			grid = new WorldTile[worldMaxX - worldMinX, worldMaxY - worldMinY];
			current = this;
		}
		GenerateAsteroids();
	}

	public void GenerateAsteroids()
	{
		System.Random rand = new System.Random();
		for (int i = 0; i < 10; i++)
		{
			int x = rand.Next(worldMinX, worldMaxX);
			int y = rand.Next(worldMinY, worldMaxY);
			int radius = rand.Next(ASTEROID_MIN_RADIUS, ASTEROID_MAX_RADIUS);
			var points = CheckAsteroidPosition(x, y, radius);
			if (points != null)
				GenerateAsteroid(points);
			else
				i--; // try again.
		}
	}

	/// <summary>
	/// Check that the position in the world grid is safe to place the asteroid.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="radius"></param>
	/// <returns></returns>
	public List<Vector2> CheckAsteroidPosition(int x, int y, int radius)
	{
		var result = new List<Vector2>();
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				if (i * i + j * j <= radius * radius)
				{
					int xCoord = i + x;
					int yCoord = j + y;
					int gridX = xCoord + worldMaxX;
					int gridY = yCoord + worldMaxY;
					if (gridX < 0 || gridY < 0 || gridX >= worldMaxX - worldMinX || gridY >= worldMaxY - worldMinY)
						return null;

					// If the area contains a non-null space, then it is not safe to place this asteroid.
					if (grid[gridX, gridY] != null)
					{
						return null;
					}
					else
						result.Add(new Vector2(xCoord, yCoord));
				}
			}
		}
		// Area is completely empty.
		return result;
	}

	/// <summary>
	/// Generate an asteroid at the given location.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	private void GenerateAsteroid(List<Vector2> positions)
	{
		var obj = Resources.Load("_Prefabs/World/Tiles/StoneTile") as GameObject;

		foreach(var position in positions)
		{
			int gridX = (int)position.x + worldMaxX;
			int gridY = (int)position.y + worldMaxY;
			grid[gridX, gridY] = Instantiate(obj, new Vector2(position.x * gridScale, position.y * gridScale), Quaternion.identity, transform).GetComponent<WorldTile>();
		}
		//for (int i = -radius; i <= radius; i++)
		//{
		//	for (int j = -radius; j <= radius; j++)
		//	{
		//		if (i * i + j * j <= radius * radius)
		//		{
		//			int xCoord = i + x;
		//			int yCoord = j + y;
		//			int gridX = xCoord + worldMaxX;
		//			int gridY = yCoord + worldMaxY;
		//			if (gridX < 0 || gridY < 0 || gridX >= worldMaxX - worldMinX || gridY >= worldMaxY - worldMinY)
		//				continue;
		//			if (grid[gridX, gridY] == null)
		//			{
		//				grid[gridX, gridY] = Instantiate(obj, new Vector2(xCoord * gridScale, yCoord * gridScale), Quaternion.identity, transform).GetComponent<WorldTile>();
		//			}
		//		}
		//	}
		//}
	}

	void OnDrawGizmos()
	{
		if (!visualize)
			return;

		// Draw vertical.
		for (int i = worldMinX; i < worldMaxX; i++)
		{
			if (i == 0 || i == -1)
				Gizmos.color = gridCenterColor;
			else
				Gizmos.color = gridColor;

			var linePos1 = new Vector3(i, worldMinY, 0) * gridScale;
			var linePos2 = new Vector3(i, worldMaxY, 0) * gridScale;
			Gizmos.DrawLine(linePos1 + gridOffset, linePos2 + gridOffset);
		}

		// Draw horizontal
		for (int i = worldMinY; i < worldMaxY; i++)
		{
			if (i == 0 || i == -1)
				Gizmos.color = gridCenterColor;
			else
				Gizmos.color = gridColor;

			var linePos1 = new Vector3(worldMinX, i, 0) * gridScale;
			var linePos2 = new Vector3(worldMaxX, i, 0) * gridScale;
			Gizmos.DrawLine(linePos1 + gridOffset, linePos2 + gridOffset);
		}
	}
}
