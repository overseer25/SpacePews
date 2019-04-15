using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MiningComponent))]
public class MiningComponentEditor : BaseItemEditor
{
    public override void OnInspectorGUI()
    {
        var miningComponent = target as MiningComponent;
        EditorGUILayout.Space();

		// ----- PROPERTIES SECTION ----- //
		EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
		DisplayItemProperties(false);
		miningComponent.itemType = ItemType.Turret;
		miningComponent.visible = true;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("    Mining Rate", GUILayout.MaxWidth(10f));
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("miningRate"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		// ----- LASER STATS SECTION ----- //
		EditorGUILayout.LabelField("Laser Stats", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Laser Width", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("laserWidth"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Laser Length", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("laserLength"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

		// ----- AUDIO/VISUAL SECTION ----- //
		DisplayAnimation();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("miningLaserBase"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("contactBase"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("contactMineable"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("contactAudioSource"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("baseAudioSource"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("laserContactSprite"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
    }
}
