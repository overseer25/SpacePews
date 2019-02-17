using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StorageComponent))]
public class StorageComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var miningComponent = target as StorageComponent;
        EditorGUILayout.Space();

        // ----- PROPERTIES SECTION ----- //
        EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Name", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Item Tier", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemTier"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Value", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Visible", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("visible"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Capacity", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("slotCount"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Description", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        // ----- AUDIO/VISUAL SECTION ----- //
        EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pickupSound"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hoverText"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
    }
}
