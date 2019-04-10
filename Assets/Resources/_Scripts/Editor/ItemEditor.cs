using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var item = target as Item;
        EditorGUILayout.Space();

		item.itemColor = ItemColors.colors[(int)item.itemTier];

        // ----- PROPERTIES SECTION ----- //
        EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"), new GUIContent("      Name"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemTier"), new GUIContent("      Tier"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("      Value"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stackable"), new GUIContent("      Stackable"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        if (item.stackable)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stackSize"), new GUIContent("          Stack Size"), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }
        else
            item.stackSize = 0;

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(20f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), new GUIContent("Description"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // ----- AUDIO/VISUAL SECTION ----- //
        EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"), new GUIContent("Sprites", "First sprite is the default sprite" +
                                                                                                ", and should always be set. Any subsequent sprites " +
                                                                                                "added can be animated."), true, GUILayout.MaxWidth(500f)); serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        if (item.sprites.Length > 1)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playspeed"), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }
    }
}
