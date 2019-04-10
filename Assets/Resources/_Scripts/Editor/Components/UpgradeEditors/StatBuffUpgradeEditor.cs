using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatBuffUpgradeComponent))]
public class StatBuffUpgradeEditor : UpgradeBaseEditor
{
	private string tip;

	public override void OnInspectorGUI()
	{
		StatBuffUpgradeComponent component = target as StatBuffUpgradeComponent;

		// ----- PROPERTIES SECTION ----- //
		DisplayPropertiesSection(component);

		// ----- BUFF SECTION ----- //
		EditorGUILayout.LabelField("Buffs", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		tip = "The buffs/debuffs this upgrade component has.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("buffs"), new GUIContent("    Buff List", tip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		// ----- AUDIO/VISUAL SECTION ----- //
		DisplayAudioVisualSection(component);
	}
}
