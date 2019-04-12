using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A custom inspector for the Projectile script.
/// </summary>
[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{

    public override void OnInspectorGUI()
    {
		string tooltip;
        var projectile = serializedObject.targetObject as Projectile;
        EditorGUILayout.Space();

        // ----- STATS Section ----- //
        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		tooltip = "The maximum damage the projectile can inflict.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDamage"), new GUIContent("    Max Damage", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.LabelField("dmg", GUILayout.MaxWidth(268f));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "The minimum damage the projectile can inflict.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("minDamage"), new GUIContent("    Min Damage", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.LabelField("dmg", GUILayout.MaxWidth(268f));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "The speed at which the projectile moves.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"), new GUIContent("    Speed", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.LabelField("m/s", GUILayout.MaxWidth(268f));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "The chance that the projectile will inflict critical damage.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("critChance"), new GUIContent("    Critical Chance", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.LabelField("%", GUILayout.MaxWidth(268f));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "The multiplier applied to the damage if the attack is a critical.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("critMultiplier"), new GUIContent("    Critical Multiplier", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.LabelField("x", GUILayout.MaxWidth(268f));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Amount of time the projectile spends in the game world before disappearing.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("lifetime"), new GUIContent("    Lifetime", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.LabelField("seconds", GUILayout.MaxWidth(268f));
		EditorGUILayout.EndHorizontal();

        // ----- EFFECTS Section ----- //
        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
		// -- HOMING -- //
		EditorGUILayout.BeginHorizontal();
		tooltip = "Does this projectile home in on targets?";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("homing"), new GUIContent("    Homing?", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
        if (projectile.homing)
        {
			EditorGUILayout.BeginHorizontal();
			tooltip = "Distance away from the target before homing can begin.";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("homingDistance"), new GUIContent("       - Homing Distance", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.LabelField("m", GUILayout.MaxWidth(268f));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			tooltip = "Homing rotation speed.";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("homingTurnSpeed"), new GUIContent("       - Homing Rotation Speed", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.LabelField("rad/s", GUILayout.MaxWidth(268f));
			EditorGUILayout.EndHorizontal();
        }
		// -- Splitting -- //
		EditorGUILayout.BeginHorizontal();
		tooltip = "Does this projectile split into other projectiles?";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("splitting"), new GUIContent("    Splitting?", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
        if (projectile.splitting)
        {
			EditorGUILayout.BeginHorizontal();
			tooltip = "The projectiles that get spawned when splitting, and the angle they spawn at.";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("splitProjectiles"), new GUIContent("        - Projectiles", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			tooltip = "Does the projectile split on collision?";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("splitOnCollision"), new GUIContent("        - Split on Collision?", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			tooltip = "Does the projectile split after an amount of time has passed?";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("splitAfterTime"), new GUIContent("        - Split After Time?", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			tooltip = "Does the projectile disappear after the first split, or does it continue to split for it's lifetime?";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("destroyOnSplit"), new GUIContent("        - Destroy on Split?", tooltip), true, GUILayout.MaxWidth(500f));
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fireSprites"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tooltip = "Does the projectile animate as it travels?";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("animated"), new GUIContent("   Animated?", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
		if(projectile.animated)
		{
			EditorGUILayout.BeginHorizontal();
			tooltip = "The animation that plays as the projectile travels.";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"), new GUIContent("       Sprite Animation", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.BeginHorizontal();
			tooltip = "The sprite the projectile uses while traveling.";
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("sprite"), new GUIContent("       Sprite", tooltip), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
		}


    }
}
