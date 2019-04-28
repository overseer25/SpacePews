using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutomaticWeapon))]
public class AutomaticWeaponEditor: BaseWeaponEditor
{
    public override void OnInspectorGUI()
    {
        var automaticWeapon = serializedObject.targetObject as AutomaticWeapon;
        EditorGUILayout.Space();

		// ----- PROPERTIES SECTION ----- //
		DisplayItemProperties(false);
		automaticWeapon.itemType = ItemType.Turret;
		automaticWeapon.visible = true;

		// ----- WEAPON STATS SECTION ----- //
		EditorGUILayout.LabelField("Weapon Stats", EditorStyles.boldLabel);
		DisplayProjectile();
		DisplayFirerate();
		DisplayShotspread();

		// ----- AUDIO/VISUAL SECTION ----- //
		EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);
		DisplayAnimation();
		DisplayFireAnimation();
	}
}
