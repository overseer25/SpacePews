using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponComponent))]
public class WeaponComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var weaponComponent = target as WeaponComponent;
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
        EditorGUILayout.LabelField("    Item Type", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemType"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Visible", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("visible"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Description", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        // ----- WEAPON STATS SECTION ----- //
        EditorGUILayout.LabelField("Weapon Stats", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectile"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Firerate", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("firerate"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Shot Spread", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shotSpread"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        // ----- AUDIO/VISUAL SECTION ----- //
        EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"), new GUIContent("Sprites","First sprite is the default sprite" +
                                                                                                ", and should always be set. Any subsequent sprites " +
                                                                                                "added can be animated."), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        if (weaponComponent.sprites.Length > 1)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playspeed"), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fireAnimation"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        if (weaponComponent.fireAnimation.Length > 0)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fireAnimPlayspeed"), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

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
