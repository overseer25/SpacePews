using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChargedWeapon))]
public class ChargedWeaponEditor : BaseWeaponEditor
{

    public override void OnInspectorGUI()
    {
        var chargedWeapon = target as ChargedWeapon;
        EditorGUILayout.Space();

        DisplayPropertySection(chargedWeapon);

        // ----- WEAPON STATS SECTION ----- //
        DisplayWeaponStatsSection(false, false);

        EditorGUILayout.BeginHorizontal();
        tooltip = "The amount of time it takes for the weapon to cooldown before being able to fire again. The speed of the cooldown animation is determined by this value.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeToCooldown"), new GUIContent("    Cooldown Time", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tooltip = "The amount of time it takes for the weapon to decharge. Decharging occurs when the user releases the fire button before the weapon is charged." +
                  "The speed of the de-charge animation is determined by this value.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeToDecharge"), new GUIContent("    De-charge Time", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        // ----- AUDIO/VISUAL SECTION ----- //
        DisplayAudioVisualSection(chargedWeapon, false);

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargingAnimation"), new GUIContent("    Charging Animation"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargedAnimationLoop"), new GUIContent("    Charged Animation (Looped)"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        if (chargedWeapon.chargedAnimationLoop.Length > 0)
        {
            EditorGUILayout.BeginHorizontal();
            tooltip = "The play speed of the charged animation loop.";
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("chargedLoopPlayspeed"), new GUIContent("       Charged Anim Play Speed", tooltip), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        tooltip = "If a decharging animation is not specified, then the charging animation will just be played in reverse.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dechargeAnimation"), new GUIContent("    Decharge Animation"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldownAnimation"), new GUIContent("    Cooldown Animation"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeSound"), new GUIContent("    Charging Sound"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tooltip = "This sound is looped so long as the weapon is charged.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargedSound"), new GUIContent("    Charged Sound", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

    }

}
