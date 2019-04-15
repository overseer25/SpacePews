using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
[CanEditMultipleObjects]
public class ItemEditor : BaseItemEditor
{
    public override void OnInspectorGUI()
    {
        var item = serializedObject.targetObject as Item;
		item.itemColor = ItemColors.colors[(int)item.itemTier];

		// ----- PROPERTIES SECTION ----- //
		DisplayItemProperties();

        // ----- AUDIO/VISUAL SECTION ----- //
        EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);
		DisplayAnimation();
    }
}
