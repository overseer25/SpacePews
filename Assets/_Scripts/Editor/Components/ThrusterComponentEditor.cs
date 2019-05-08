﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThrusterComponent))]
public class ThrusterComponentEditor : BaseItemEditor
{
    public override void OnInspectorGUI()
    {
        var thrusterComponent = serializedObject.targetObject as ThrusterComponent;
        EditorGUILayout.Space();

		// ----- PROPERTIES SECTION ----- //
		DisplayItemProperties(false);
		thrusterComponent.itemType = ItemType.Thruster;
		thrusterComponent.visible = true;

        // ----- THRUSTER STATS SECTION ----- //
        EditorGUILayout.LabelField("Thruster Stats", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Max Speed", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpeed"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Acceleration", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("acceleration"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Deceleration", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("deceleration"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Rotation Speed", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeed"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

		// ----- AUDIO/VISUAL SECTION ----- //
		EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);
		DisplayAnimation();

		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engine"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
    }
}