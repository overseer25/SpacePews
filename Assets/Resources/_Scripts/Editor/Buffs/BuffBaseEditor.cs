using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Contains methods useful for custom buff editors.
/// </summary>
[CustomEditor(typeof(Buff))]
public abstract class BuffBaseEditor : Editor
{
	/// <summary>
	/// Each Buff can have a title that is displayed in the inspector to help the developer
	/// understand what the buff does.
	/// </summary>
	public abstract void DisplayTitle();

}
