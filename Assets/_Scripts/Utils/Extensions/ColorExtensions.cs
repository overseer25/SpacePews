using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for the Unity Color class.
/// </summary>
public static class ColorExtensions
{
	/// <summary>
	/// Converts an RGB color to its hex form.
	/// </summary>
	/// <param name="color"></param>
	/// <returns></returns>
    public static string ToHex(this Color color)
	{
		int r = (int)Math.Round(color.r * 255);
		int g = (int)Math.Round(color.g * 255);
		int b = (int)Math.Round(color.b * 255);

		string result = "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");

		return result;
	}
}
