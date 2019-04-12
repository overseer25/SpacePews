using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StorageComponent))]
public class StorageComponentEditor : BaseItemEditor
{
    public override void OnInspectorGUI()
    {
        var storageComponent = serializedObject.targetObject as StorageComponent;
        EditorGUILayout.Space();

		// ----- PROPERTIES SECTION ----- //
		DisplayItemProperties(false);
		storageComponent.itemType = ItemType.Storage;
		storageComponent.visible = false;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("    Capacity", GUILayout.MaxWidth(10f));
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("slotCount"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		// ----- AUDIO/VISUAL SECTION ----- //
		EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);
		DisplayAnimation();
	}
}
