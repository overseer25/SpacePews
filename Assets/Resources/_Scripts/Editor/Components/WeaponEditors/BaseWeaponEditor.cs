using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponComponentBase))]
public class BaseWeaponEditor : ComponentBaseEditor
{
    public string tooltip;

    public override void OnInspectorGUI()
    {
        var component = target as WeaponComponentBase;
        EditorGUILayout.Space();

        DisplayPropertySection(component);
    }

    /// <summary>
    /// Creates the part of the inspector for the component's properties.
    /// </summary>
    public void DisplayPropertySection(WeaponComponentBase component)
    {
        // ----- PROPERTIES SECTION ----- //
        EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"), new GUIContent("    Name"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tooltip = "Determines the color of text to display for the item name. Higher tiers represent higher grade items.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemTier"), new GUIContent("    Tier", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        component.itemType = ItemType.Turret;

        EditorGUILayout.BeginHorizontal();
        tooltip = "The base value of the item in the in-game economy. Tier should be assessed before assigning a value.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("    Value (In Credits)", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        component.visible = true;

        EditorGUILayout.BeginHorizontal();
        tooltip = "Further details about the weapon to be displayed to the user.";
        EditorGUILayout.LabelField("    ", GUILayout.MaxWidth(12f));
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), new GUIContent("Description", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Display the weapon stats section of the inspector.
    /// </summary>
    /// <param name="showFirerate"></param>
    /// <param name="showSpread"></param>
    public void DisplayWeaponStatsSection(bool showFirerate = true, bool showSpread = true)
    {
        EditorGUILayout.LabelField("Weapon Stats", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectile"), new GUIContent("    Projectile"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        if (showFirerate)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("firerate"), new GUIContent("    Fire rate"), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        if (showSpread)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shotSpread"), new GUIContent("    Shot spread"), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Displays the audio/visual section of the inspector.
    /// </summary>
    public void DisplayAudioVisualSection(WeaponComponentBase component, bool showFireAnimation = true)
    {
        EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        tooltip = "First sprite is the default sprite, and should always be set. Any subsequent sprites will turn this into an idle animation.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"), new GUIContent("    Sprites", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        if (component.sprites.Length > 1)
        {
            EditorGUILayout.BeginHorizontal();
            tooltip = "The rate at which to play the idle animation";
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playspeed"), new GUIContent("    Idle Anim Play Speed"), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        if (showFireAnimation)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fireAnimation"), new GUIContent("    Fire Animation"), true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
    }
}
