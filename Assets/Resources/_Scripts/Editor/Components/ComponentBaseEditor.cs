using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShipComponentBase))]
public class ComponentBaseEditor : Editor
{
	ShipComponentBase component;
	private string tooltip;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}

	/// <summary>
	/// Display the properties of the component.
	/// </summary>
	public virtual void DisplayPropertiesSection()
	{
		EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"), new GUIContent("    Name"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Determines the color of text to display for the component name. Higher tiers represent higher grade components.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("itemTier"), new GUIContent("    Tier", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "The base value of the component in the in-game economy. Tier should be assessed before assigning a value.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("    Value (In Credits)", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Further details about the component to be displayed to the user.";
		EditorGUILayout.LabelField("    ", GUILayout.MaxWidth(12f));
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), new GUIContent("Description", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// Display the audio/visual properties of the component.
	/// </summary>
	public virtual void DisplayAudioVisualSection(ShipComponentBase component)
	{
		EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		tooltip = "First sprite is the default sprite, and should always be set. Any subsequent sprites will turn this into an idle animation.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"), new GUIContent("    Sprites", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
		if (component.sprites.Length > 1)
		{
			EditorGUILayout.BeginHorizontal();
			tooltip = "The rate at which to play the idle animation";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("playspeed"), new GUIContent("    Idle Anim Play Speed"), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
		}
	}
}
