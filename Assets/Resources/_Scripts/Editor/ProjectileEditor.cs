using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A custom inspector for the Projectile script.
/// </summary>
[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    private string damage = "";
    private string speed = "";
    private string lifetime = "";
    private ParticleEffect destroyEffect;
    private AudioClip fireSound;
    private string playSpeed = "";
    private bool homingToggle = false;
    private string homingDistance = "";
    private string homingRotationSpeed = "";

    public override void OnInspectorGUI()
    {
        int result = 0;
        float resultf = 0.0f;
        var projectile = target as Projectile;

        // ----- STATS Section ----- //
        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("    Lifetime", GUILayout.MaxWidth(268f));
        lifetime = EditorGUILayout.TextField(projectile.lifetime.ToString(), GUILayout.MaxWidth(100f));
        if (int.TryParse(lifetime, out result))
        {
            projectile.lifetime = result;
        }
        EditorGUILayout.EndHorizontal();

        // ----- EFFECTS Section ----- //
        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
        // -- HOMING -- //
        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.LabelField("    Homing", GUILayout.MaxWidth(268f));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("homing"), GUIContent.none, GUILayout.MaxWidth(100f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        if (projectile.homing)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("        - Homing Distance", GUILayout.MaxWidth(268f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("homingDistance"), GUIContent.none, GUILayout.MaxWidth(100f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("        - Homing Rotation Speed", GUILayout.MaxWidth(268f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("homingTurnSpeed"), GUIContent.none, GUILayout.MaxWidth(100f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            projectile.homingDistance = 0.0f;
            projectile.homingTurnSpeed = 0.0f;
        }
        // -- Splitting -- //
        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.LabelField("    Splitting", GUILayout.MaxWidth(268f));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("splitting"), GUIContent.none, GUILayout.MaxWidth(100f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        if (projectile.splitting)
        {
            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("        - Projectiles", GUILayout.MaxWidth(150f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("splitProjectiles"), GUIContent.none, true, GUILayout.MaxWidth(500f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("        - Split on Collision", GUILayout.MaxWidth(268f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("splitOnCollision"), GUIContent.none, GUILayout.MaxWidth(100f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("        - Split After Time", GUILayout.MaxWidth(268f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("splitAfterTime"), GUIContent.none, GUILayout.MaxWidth(100f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            serializedObject.Update();
            EditorGUILayout.LabelField("        - Destroy on Split", GUILayout.MaxWidth(268f));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("destroyOnSplit"), GUIContent.none, GUILayout.MaxWidth(100f));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        // ----- AUDIO/VISUAL Section ----- //
        EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("destroyEffect"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fireSound"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
        if (projectile.sprites.Length > 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("    Play Speed", GUILayout.MaxWidth(285f));
            playSpeed = EditorGUILayout.TextField(projectile.playspeed.ToString(), GUILayout.MaxWidth(100f));
            if (float.TryParse(playSpeed, out resultf))
            {
                projectile.playspeed = resultf;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
