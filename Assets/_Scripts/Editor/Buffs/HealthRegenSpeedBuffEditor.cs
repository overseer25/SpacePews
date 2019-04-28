using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for health regen speed buff objects.
/// </summary>
[CustomEditor(typeof(HealthRegenSpeedBuff))]
public class HealthRegenSpeedBuffEditor : BuffBaseEditor
{
	private string tooltip;

	public override void OnInspectorGUI()
	{
        var buff = target as HealthRegenSpeedBuff;

        EditorGUILayout.BeginHorizontal();
		tooltip = "Is this a buff or a debuff?";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("debuff"), new GUIContent("    Is Debuff?", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Static value to modify regen speed by.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("regenSpeed"), new GUIContent("    Regen Speed", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		DisplayTime();
		DisplayIcon();
		DisplayDescriptions();
	}
}
