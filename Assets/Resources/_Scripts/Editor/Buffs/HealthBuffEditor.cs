using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for health buff objects.
/// </summary>
[CustomEditor(typeof(HealthBuff))]
public class HealthBuffEditor : BuffBaseEditor
{
	string tooltip;

	public override void OnInspectorGUI()
	{
		HealthBuff buff = target as HealthBuff;

        DisplayDescription(buff);

		EditorGUILayout.BeginHorizontal();
		tooltip = "Is this a static value or a multiplier?";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isMultiplier"), new GUIContent("    Is Multiplier?", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		if(buff.isMultiplier)
		{
			EditorGUILayout.BeginHorizontal();
			tooltip = "Percentage of current max health to modify by.";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("multiplier"), new GUIContent("    Multiplier", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.BeginHorizontal();
			tooltip = "Static value to modify max health by.";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("healthAmount"), new GUIContent("    Amount", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			tooltip = "Is this a buff or a debuff?";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("debuff"), new GUIContent("    Is Debuff?", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
		}
	}
}
