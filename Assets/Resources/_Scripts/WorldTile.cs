﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a block in the world. ID must be unique.
/// </summary>
public class WorldTile : MonoBehaviour
{
	public TileData data;

	private Coroutine miningCoroutine;
	private bool beingMined;
	private float miningSpeedModifier;
	private SpriteRenderer spriteRenderer;
    private float lightLevel = 1.0f;

	// this tile's position on the grid.
	private int gridx, gridy;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	/// <summary>
	/// Initialize with a TileData object.
	/// </summary>
	/// <param name="data"></param>
	public void Initialize(TileData data, int x, int y)
	{
		this.data = data;
		gridx = x;
		gridy = y;
	}

	/// <summary>
	/// Update the sprite based on the surrounding tiles.
	/// </summary>
	public void UpdateTile()
	{
		var newSprite = GetSpriteForTile();
		spriteRenderer.sprite = newSprite;
	}

	/// <summary>
	/// Copy data of one World Tile to another.
	/// </summary>
	/// <param name="other"></param>
	public void Copy(WorldTile other)
	{
		data = other.data;
	}

	public void StartMining(float? modifier = null)
	{
		if(miningCoroutine == null)
		{
			if (modifier != null)
				miningSpeedModifier = (float)modifier;
			miningCoroutine = StartCoroutine(Mine());
		}
	}

    /// <summary>
    /// Get the list of this tile's neighboring tiles.
    /// </summary>
    /// <returns></returns>
    public List<WorldTile> GetNeighbors()
    {
        var result = new List<WorldTile>();

        if (!World.OutsideBounds(gridx - 1, gridy + 1) && World.current.grid[gridx - 1, gridy + 1] != null)
            result.Add(World.current.grid[gridx - 1, gridy + 1]);
        if (!World.OutsideBounds(gridx, gridy + 1) && World.current.grid[gridx, gridy + 1] != null)
            result.Add(World.current.grid[gridx, gridy + 1]);
        if (!World.OutsideBounds(gridx + 1, gridy + 1) && World.current.grid[gridx + 1, gridy + 1] != null)
            result.Add(World.current.grid[gridx + 1, gridy + 1]);
        if (!World.OutsideBounds(gridx - 1, gridy) && World.current.grid[gridx - 1, gridy] != null)
            result.Add(World.current.grid[gridx - 1, gridy]);
        if (!World.OutsideBounds(gridx + 1, gridy) && World.current.grid[gridx + 1, gridy] != null)
            result.Add(World.current.grid[gridx + 1, gridy]);
        if (!World.OutsideBounds(gridx - 1, gridy - 1) && World.current.grid[gridx - 1, gridy - 1] != null)
            result.Add(World.current.grid[gridx - 1, gridy - 1]);
        if (!World.OutsideBounds(gridx, gridy - 1) && World.current.grid[gridx, gridy - 1] != null)
            result.Add(World.current.grid[gridx, gridy - 1]);
        if (!World.OutsideBounds(gridx + 1, gridy - 1) && World.current.grid[gridx + 1, gridy - 1] != null)
            result.Add(World.current.grid[gridx + 1, gridy - 1]);

        return result;
    }

	public void StopMining()
	{
		if (miningCoroutine != null)
		{
			miningSpeedModifier = 0;
			StopCoroutine(miningCoroutine);
			miningCoroutine = null;
			beingMined = false;
		}
	}

    /// <summary>
    /// Update the light level of the tile.
    /// </summary>
    public void UpdateLightLevel()
    {
        var neighbors = GetNeighbors();

        if (neighbors.Count == 8)
        {
            foreach (var neighbor in neighbors)
            {
                if (neighbor != null)
                    if (neighbor.lightLevel > lightLevel)
                        lightLevel = neighbor.lightLevel;
            }
            lightLevel -= 0.2f;
        }
        else
            lightLevel = 1.0f;

        spriteRenderer.color = new Color(lightLevel, lightLevel, lightLevel);
    }

    /// <summary>
	/// Update the sprites of the nearby neighbors.
	/// 
	/// There is going to be a lot of duplicates hit during this computation, so this could do
	/// with a lot of optimization.
	/// </summary>
	private void UpdateNeighbors()
    {
        foreach (var neighbor in GetNeighbors())
            neighbor.UpdateTile();
    }

    /// <summary>
    /// Mine the tile.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Mine()
	{
		if(!beingMined)
		{
			var time = data.timeToMine;
			if (miningSpeedModifier != 0.0f)
				time /= miningSpeedModifier;
			beingMined = true;
			yield return new WaitForSeconds(time);
			SpawnLoot();
		}
	}

	/// <summary>
	/// Spawn loot and destroy the block.
	/// </summary>
	private void SpawnLoot()
	{
		var item = ItemPool.current.GetPooledObject() as Item;
		if (item == null)
			return;

		item.Initialize(gameObject, data.loot);

		// Set this tile's world position to null so that other tiles update accordingly.
		UpdateNeighbors();
        World.current.RemoveTile(gridx, gridy);
	}

	/// <summary>
	/// Determine which sprite from the given tileset to give the tile at the given position.
	/// </summary>
	/// <param name="tiles"></param>
	/// <param name="position"></param>
	/// <returns></returns>
	private Sprite GetSpriteForTile()
	{
		bool topBorder = false, topRightCorner = false, rightBorder = false, bottomRightCorner = false, bottomBorder = false,
								bottomLeftCorner = false, leftBorder = false, topLeftCorner = false;
        
		// Check the 8 grid positions around the current position to determine the sprite to use.
		if (World.OutsideBounds(gridx, gridy + 1) || World.current.grid[gridx, gridy + 1] == null)
			topBorder = true;
		if (World.OutsideBounds(gridx + 1, gridy + 1) || World.current.grid[gridx + 1, gridy + 1] == null)
			topRightCorner = true;
		if (World.OutsideBounds(gridx + 1, gridy) || World.current.grid[gridx + 1, gridy] == null)
			rightBorder = true;
		if (World.OutsideBounds(gridx + 1, gridy - 1) || World.current.grid[gridx + 1, gridy - 1] == null)
			bottomRightCorner = true;
		if (World.OutsideBounds(gridx, gridy - 1) || World.current.grid[gridx, gridy - 1] == null)
			bottomBorder = true;
		if (World.OutsideBounds(gridx - 1, gridy - 1) || World.current.grid[gridx - 1, gridy - 1] == null)
			bottomLeftCorner = true;
		if (World.OutsideBounds(gridx - 1, gridy) || World.current.grid[gridx - 1, gridy] == null)
			leftBorder = true;
		if (World.OutsideBounds(gridx - 1, gridy + 1) || World.current.grid[gridx - 1, gridy + 1] == null)
			topLeftCorner = true;

		// Now, determine which tile to use.
		if (topBorder && rightBorder && bottomBorder && leftBorder)
        {
            return data.fullBorder;
        }

		if (topBorder && rightBorder && bottomBorder)
			return data.noLeft;

		if (topBorder && leftBorder && bottomBorder)
			return data.noRight;

		if (topBorder && leftBorder && rightBorder)
			return data.noBottom;

		if (topBorder && rightBorder)
			return data.topRightCorner1;

		if (topBorder && bottomBorder)
			return data.noVertical;

		if (topBorder && leftBorder)
			return data.topLeftCorner1;

		if (topBorder)
			return data.topBorder;

		if (rightBorder && bottomBorder && leftBorder)
			return data.noTop;

		if (rightBorder && bottomBorder)
			return data.bottomRightCorner1;

		if (rightBorder && leftBorder)
			return data.noHorizontal;

		if (rightBorder)
			return data.rightBorder;

		if (bottomBorder && leftBorder)
			return data.bottomLeftCorner1;

		if (bottomBorder)
			return data.bottomBorder;

		if (leftBorder)
			return data.leftBorder;

		return data.noBorder;
	}

}
