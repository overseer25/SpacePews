using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public static World current;
	public WorldTile defaultTile;
	public int worldMinX = -100;
	public int worldMaxX = 100;
	public int worldMinY = -100;
	public int worldMaxY = 100;
	public Vector3 gridOffset = new Vector3(0.5f, 0.5f, 0);
	public float gridScale = 1.0f;
	public float tileScale = 6.25f;
	public Color gridColor = Color.grey;
	public Color gridCenterColor = Color.white;
	public bool visualize = true;

	[HideInInspector]
	public WorldTile[,] grid;
    private List<WorldTile> tiles;

	private const int ASTEROID_MAX_RADIUS = 20;
	private const int ASTEROID_MIN_RADIUS = 4;

	void Start()
	{
		if (current == null)
		{
            tiles = new List<WorldTile>();
			grid = new WorldTile[worldMaxX - worldMinX, worldMaxY - worldMinY];
			current = this;
		}
		GenerateAsteroids();
	}

    public void Update()
    {
        UpdateLighting();
    }

    public void GenerateAsteroids()
	{
		System.Random rand = new System.Random();
		for (int i = 0; i < 20; i++)
		{
			int x = rand.Next(worldMinX, worldMaxX);
			int y = rand.Next(worldMinY, worldMaxY);
			int radius = rand.Next(ASTEROID_MIN_RADIUS, ASTEROID_MAX_RADIUS);
			var points = CheckAsteroidPosition(x, y, radius);
			if (points != null)
				GenerateAsteroid(points);
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
		int boundarySize = 5;
		var result = new List<Vector2>();
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				if (i * i + j * j <= radius * radius)
				{
					// For checking boundaries.
					int boundaryX = i * boundarySize + x + worldMaxX;
					int boundaryY = j * boundarySize + y + worldMaxY;

					// Where the tiles get placed.
					int xCoord = i + x;
					int yCoord = j + y;
					int gridX = xCoord + worldMaxX;
					int gridY = yCoord + worldMaxY;
					if (gridX < 0 || gridY < 0 || gridX >= worldMaxX - worldMinX || gridY >= worldMaxY - worldMinY)
						return null;

					// If the area contains a non-null space, then it is not safe to place this asteroid.
					if (!OutsideBounds(boundaryX, boundaryY) && grid[boundaryX, boundaryY] != null)
					{
						return null;
					}
					else
						result.Add(new Vector2(gridX, gridY));
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Check if the given position is within the bounds of the map or not.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static bool OutsideBounds(int x, int y)
	{
		var result = (x >= current.worldMaxX - current.worldMinX) || x < 0 || (y >= current.worldMaxY - current.worldMinY) || y < 0;
		return result;
	}

    /// <summary>
    /// Remove a tile from the world.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void RemoveTile(int x, int y)
    {
        if (grid[x,y] == null)
            return;

        tiles.Remove(grid[x, y]);
        Destroy(grid[x, y].gameObject);
        grid[x, y] = null;
    }

    /// <summary>
    /// Spawn a tile to the world.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void AddTile(TileData data, int x, int y)
    {
        WorldTile tile = Instantiate(defaultTile, new Vector2(x * gridScale, y * gridScale), Quaternion.identity, transform) as WorldTile;
        tile.Initialize(data, x, y);
        grid[x, y] = tile;
        tiles.Add(tile);
    }

    /// <summary>
    /// Update the lighting of the blocks on the screen.
    /// </summary>
    private void UpdateLighting()
    {
        foreach (var tile in tiles)
            tile.UpdateLightLevel();
    }

    /// <summary>
    /// Generate an asteroid at the given location. This method also determines which sprite to use for
    /// any given block. At this point, it is guaranteed that each position is free.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void GenerateAsteroid(List<Vector2> positions)
	{
		var stoneTiles = TileSetCollection.current.stone;
		foreach (var position in positions)
		{
            AddTile(stoneTiles, (int)position.x, (int)position.y);
		}

		foreach (var position in positions)
			grid[(int)position.x, (int)position.y].UpdateTile();
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
