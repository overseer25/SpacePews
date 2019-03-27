using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StorageComponent))]
public class StorageComponentEditor : ComponentBaseEditor
{
    public override void OnInspectorGUI()
    {
        var storageComponent = target as StorageComponent;
        EditorGUILayout.Space();

		// ----- PROPERTIES SECTION ----- //
		DisplayPropertiesSection();
		storageComponent.itemType = ItemType.Storage;
		storageComponent.visible = false;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("    Capacity", GUILayout.MaxWidth(10f));
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("slotCount"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		// ----- AUDIO/VISUAL SECTION ----- //
		DisplayAudioVisualSection(storageComponent);
    }
}
