using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An animation containing a sprite list, play speed, and allows the user to specify whether or not this animation
/// loops.
/// </summary>
[CreateAssetMenu(menuName = "New Sprite Animation")]
public class SpriteAnimation : ScriptableObject
{
	public Sprite[] frames;
	public float playSpeed;
	public float reversePlaySpeed;
	public bool looping;

	private int index;

	/// <summary>
	/// Get the next frame of the animation.
	/// </summary>
	/// <returns>Null if the animation has completed, the next animation sprite otherwise. If the animation hasn't started yet,
	/// then the frame will be the 0th frame.</returns>
	public Sprite GetNextFrame()
	{
		Sprite result;
		if (index >= frames.Length)
		{
			if (looping)
				index = 0;
			else
				return null;
		}
		if (index < 0)
			index = 0;

		result = frames[index++];
		return result;
	}

	public int GetIndex()
	{
		return index;
	}

	/// <summary>
	/// Get the previous frame of the animation.
	/// </summary>
	/// <returns></returns>
	public Sprite GetPreviousFrame()
	{
		Sprite result;
		index--;
		if(index < 0)
		{
			if (looping)
				index = frames.Length - 1;
			else
				return null;
		}
		result = frames[index];
		return result;
	}

	/// <summary>
	/// Get a specific frame in the animation.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public Sprite GetFrame(int index)
	{
		if (index >= frames.Length || index < 0)
			return null;
		else
			return frames[index];
	}

	/// <summary>
	/// Reset the animation
	/// </summary>
	/// <param name="frame">Start the animation at this frame.</param>
	public void ResetAnimation(int frame = 0)
	{
		if (frame >= frames.Length || frame < 0)
			frame = 0;

		index = frame;
	}
}
