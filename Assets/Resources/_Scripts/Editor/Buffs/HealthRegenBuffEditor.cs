using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for health regen buff objects.
/// </summary>
[CustomEditor(typeof(HealthRegenBuff))]
public class HealthRegenBuffEditor : BuffBaseEditor
{
	private string tooltip;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Is this a buff or a debuff?";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("debuff"), new GUIContent("    Is Debuff?", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Static value to modify regen amount by.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("regenAmount"), new GUIContent("    Regen Amount", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}
}
