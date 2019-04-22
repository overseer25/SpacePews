using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class holds all data related to a world tile, including the contextual sprites, and whether or not
/// it is harvestable/destroyable.
/// 
/// 
/// MISSING TILES: Single border with dot in opposite corner(s), single/double/triple dots on borderless tile.
/// </summary>
[CreateAssetMenu(menuName ="World/Tile Data")]
public class TileData : ScriptableObject
{
	/// <summary>
	/// Is the tile a resource node?
	/// </summary>
	public bool mineable;

	/// <summary>
	/// Is the tile destroyable by explosions and the like?
	/// </summary>
	public bool destroyable;

	public float timeToMine;

	public Item loot;

	/// <summary>
	/// A tile with a bottom border.
	/// </summary>
	[Tooltip("A tile with a bottom border.")]
	public Sprite bottomBorder;

	/// <summary>
	/// A tile with borders on the bottom and left side, as well as a border pixel for the top right.
	/// Used for thin corners.
	/// </summary>
	[Tooltip("A tile with borders on the bottom and left side, as well as a border pixel for the top right. Used for thin corners.")]
	public Sprite bottomLeftCorner1;

	/// <summary>
	/// A tile with no borders, but a border pixel in the bottom left corner.
	/// Used for thick corners.
	/// </summary>
	[Tooltip("A tile with no borders, but a border pixel in the bottom left corner. Used for thick corners.")]
	public Sprite bottomLeftDot;

	/// <summary>
	/// A tile with borders on the bottom and right side, as well as a border pixel for the top left.
	/// Used for thin corners.
	/// </summary>
	[Tooltip("A tile with borders on the bottom and right side, as well as a border pixel for the top left. Used for thin corners.")]
	public Sprite bottomRightCorner1;

	/// <summary>
	/// A tile with no borders, but a border pixel in the bottom right corner.
	/// Used for thick corners.
	/// </summary>
	[Tooltip("A tile with no borders, but a border pixel in the bottom right corner. Used for thick corners.")]
	public Sprite bottomRightDot;

	/// <summary>
	/// A tile with a full border. Used for single blocks.
	/// </summary>
	[Tooltip("A tile with all borders.")]
	public Sprite fullBorder;

	/// <summary>
	/// A tile with a left border.
	/// </summary>
	[Tooltip("A tile with a left border.")]
	public Sprite leftBorder;

	/// <summary>
	/// A tile with no border.
	/// </summary>
	[Tooltip("A tile with no borders.")]
	public Sprite noBorder;

	/// <summary>
	/// A tile with no bottom border.
	/// </summary>
	[Tooltip("A tile with no bottom border.")]
	public Sprite noBottom;

	/// <summary>
	/// A tile with no horizontal borders.
	/// </summary>
	[Tooltip("A tile with no horizontal borders.")]
	public Sprite noHorizontal;

	/// <summary>
	/// A tile with no left border.
	/// </summary>
	[Tooltip("A tile with no left border.")]
	public Sprite noLeft;

	/// <summary>
	/// A tile with no right border.
	/// </summary>
	[Tooltip("A tile with no right border.")]
	public Sprite noRight;

	/// <summary>
	/// A tile with no top border.
	/// </summary>
	[Tooltip("A tile with no top border.")]
	public Sprite noTop;

	/// <summary>
	/// A tile with no vertical borders.
	/// </summary>
	[Tooltip("A tile with no vertical borders.")]
	public Sprite noVertical;

	/// <summary>
	/// A tile with a right border.
	/// </summary>
	[Tooltip("A tile with a right border.")]
	public Sprite rightBorder;

	/// <summary>
	/// A tile with a top border.
	/// </summary>
	[Tooltip("A tile with a top border.")]
	public Sprite topBorder;

	/// <summary>
	/// A tile with borders on the top and left side, as well as a border pixel for the bottom right.
	/// Used for thin corners.
	/// </summary>
	[Tooltip("A tile with borders on the top and left side, as well as a border pixel for the bottom right. Used for thin corners.")]
	public Sprite topLeftCorner1;

	/// <summary>
	/// A tile with no borders, but a border pixel in the top left corner.
	/// Used for thick corners.
	/// </summary>
	[Tooltip("A tile with no borders, but a border pixel in the top left corner. Used for thick corners.")]
	public Sprite topLeftDot;

	/// <summary>
	/// A tile with borders on the top and right side, as well as a border pixel for the bottom left.
	/// Used for thin corners.
	/// </summary>
	[Tooltip("A tile with borders on the top and right side, as well as a border pixel for the bottom left. Used for thin corners.")]
	public Sprite topRightCorner1;

	/// <summary>
	/// A tile with no borders, but a border pixel in the top right corner.
	/// Used for thick corners.
	/// </summary>
	[Tooltip("A tile with no borders, but a border pixel in the top right corner. Used for thick corners.")]
	public Sprite topRightDot;








}
