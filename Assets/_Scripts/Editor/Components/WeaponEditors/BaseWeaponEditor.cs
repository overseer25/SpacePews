using UnityEditor;
using UnityEngine;

/// <summary>
/// Base editor methods for all weapons.
/// </summary>
public class BaseWeaponEditor : BaseItemEditor
{
	/// <summary>
	/// Display the projectile to be fired.
	/// </summary>
	public void DisplayProjectile()
	{
		EditorGUILayout.BeginHorizontal();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("projectile"), new GUIContent("    Projectile"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// Display the firerate.
	/// </summary>
	public void DisplayFirerate()
	{
		EditorGUILayout.BeginHorizontal();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("firerate"), new GUIContent("    Fire rate"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// Display the shot spread.
	/// </summary>
	public void DisplayShotspread()
	{
		EditorGUILayout.BeginHorizontal();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("shotSpread"), new GUIContent("    Shot spread"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// Display the fire animation.
	/// </summary>
	public void DisplayFireAnimation()
	{
		EditorGUILayout.BeginHorizontal();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("fireAnimation"), new GUIContent("    Fire Animation"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}
}
