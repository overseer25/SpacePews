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
	public override void OnInspectorGUI()
	{
		Buff buff = target as Buff;

		EditorGUILayout.BeginHorizontal();
		string tip = "A general overview of what the buff does.";
		EditorGUILayout.LabelField(new GUIContent(buff.BuildDescription(), tip), EditorStyles.boldLabel);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

	}
}
