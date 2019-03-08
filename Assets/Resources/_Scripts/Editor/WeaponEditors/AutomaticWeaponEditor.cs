using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutomaticWeapon))]
public class AutomaticWeaponEditor: BaseWeaponEditor
{
    public override void OnInspectorGUI()
    {
        var automaticWeapon = target as AutomaticWeapon;
        EditorGUILayout.Space();

        DisplayPropertySection(automaticWeapon);

        // ----- WEAPON STATS SECTION ----- //
        DisplayWeaponStatsSection();

        // ----- AUDIO/VISUAL SECTION ----- //
        DisplayAudioVisualSection(automaticWeapon);
    }
}
