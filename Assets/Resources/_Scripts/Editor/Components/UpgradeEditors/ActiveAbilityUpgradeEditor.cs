using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActiveAbilityComponent))]
public class ActiveAbilityUpgradeEditor : BaseItemEditor
{
	private string tip;

	public override void OnInspectorGUI()
	{
		// ----- PROPERTIES SECTION ----- //
		DisplayItemProperties(false);

		// ----- ACTIVE ABILITY SECTION ----- //
		EditorGUILayout.LabelField("Active Ability", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		tip = "The active ability the component gives the player.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("ability"), new GUIContent("    Active Ability", tip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		// ----- AUDIO/VISUAL SECTION ----- //
		EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);
		DisplayAnimation();
	}
}
