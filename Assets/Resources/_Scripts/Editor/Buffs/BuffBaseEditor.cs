using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Contains methods useful for custom buff editors.
/// </summary>
public abstract class BuffBaseEditor : Editor
{

    /// <summary>
    /// Display the description.
    /// </summary>
    /// <param name="buff"></param>
    public void DisplayDescription(Buff buff)
    {
        string tip = "A general overview of what the buff does.";
        EditorGUILayout.LabelField(new GUIContent(buff.BuildDescription(), tip), EditorStyles.boldLabel);
    }

	public void DisplayTime()
	{
		EditorGUILayout.BeginHorizontal();
		string tooltip = "The time the buff will be applied before being removed.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("timeInSeconds"), new GUIContent("    Time Applied (in seconds)", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// Display field for the Icon.
	/// </summary>
	public void DisplayIcon()
	{
		EditorGUILayout.BeginHorizontal();
		string tooltip = "The icon for the buff. This is displayed in the player HUD.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"), new GUIContent("    Icon", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}
}
