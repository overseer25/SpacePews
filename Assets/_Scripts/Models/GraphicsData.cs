using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serialized graphics data used by the graphics menu in the options.
/// </summary>
[Serializable]
public class GraphicsData
{
    public bool vsync;
	public bool damageNumbers;
    public int aspectRatio;
    public int resolution;
    public int framerate;
    public int itemCount;
    public int effectCount;

	/// <summary>
	/// Creates a GraphicsData object with default values.
	/// </summary>
	public GraphicsData()
	{
		vsync = false;
		damageNumbers = true;
		aspectRatio = 0;
		resolution = 0;
		framerate = 0;
		itemCount = 64;
		effectCount = 64;
	}
}
