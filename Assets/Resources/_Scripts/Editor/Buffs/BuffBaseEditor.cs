using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Contains methods useful for custom buff editors.
/// </summary>
public abstract class BuffBaseEditor : Editor
{
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
	/// Display description texts.
	/// </summary>
	public void DisplayDescriptions()
	{
		EditorGUILayout.BeginHorizontal();
		string tooltip = "Pretty text for the info screen.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("infoScreenText"), new GUIContent("    Pretty Text", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Plain text version of the pretty text.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("plainText"), new GUIContent("    Plain Text", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Text displayed when hovering over the buff icon.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("buffIconText"), new GUIContent("    Buff Icon Text", tooltip), true, GUILayout.MaxWidth(500f));
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
