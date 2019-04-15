using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatBuffUpgradeComponent))]
public class StatBuffUpgradeEditor : BaseItemEditor
{
	private string tip;

	public override void OnInspectorGUI()
	{
		// ----- PROPERTIES SECTION ----- //
		DisplayItemProperties(false);

		// ----- BUFF SECTION ----- //
		EditorGUILayout.LabelField("Buffs", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		tip = "The buffs/debuffs this upgrade component has.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("buffs"), new GUIContent("    Buff List", tip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		// ----- AUDIO/VISUAL SECTION ----- //
		EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);
		DisplayAnimation();
	}
}
