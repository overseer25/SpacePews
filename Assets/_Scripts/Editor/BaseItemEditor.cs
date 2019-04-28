using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Base methods for item editors for displaying.
/// </summary>
public class BaseItemEditor : Editor
{
	private string tooltip;
	private float changeSprite = 0;
	private int index = 0;
	private bool startAnimation = false;
	private double animationStartTime;

	/// <summary>
	/// Display the item's name.
	/// </summary>
    public void DisplayItemName()
	{
		EditorGUILayout.BeginHorizontal();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"), new GUIContent("    Name"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// Display the item's tier.
	/// </summary>
	public void DisplayItemTier()
	{
		EditorGUILayout.BeginHorizontal();
		tooltip = "Determines the color of text to display for the item name. Higher tiers represent higher grade items.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("itemTier"), new GUIContent("    Tier", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}

	public void DisplayItemValue()
	{
		EditorGUILayout.BeginHorizontal();
		tooltip = "The base value of the item in the in-game economy. Tier should be assessed before assigning a value.";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("    Value (In Credits)", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// Display the stackable checkbox. If ticked, the stack size option will appear; otherwise, the item will not be stackable.
	/// </summary>
	public void DisplayStackable()
	{
		var item = serializedObject.targetObject as Item;
		EditorGUILayout.BeginHorizontal();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("stackable"), new GUIContent("      Stackable"), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		if (item.stackable)
		{
			EditorGUILayout.BeginHorizontal();
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("stackSize"), new GUIContent("          Stack Size"), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// Display the description of the item.
	/// </summary>
	public void DisplayDescription()
	{
		EditorGUILayout.BeginHorizontal();
		tooltip = "Further details about the item to be displayed to the user.";
		EditorGUILayout.LabelField("    ", GUILayout.MaxWidth(12f));
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), new GUIContent("Description", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}

	/// <summary>
	/// Display the animated checkbox. If ticked, the sprite animation option will appear; otherwise, the sprite option will appear.
	/// </summary>
	public void DisplayAnimation()
	{
		EditorGUILayout.Space();
		var item = serializedObject.targetObject as Item;
		EditorGUILayout.BeginHorizontal();
		tooltip = "Is the sprite for this item animated?";
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("animated"), new GUIContent("    Idle Animation?", tooltip), true, GUILayout.MaxWidth(500f));
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();

		if (item.animated)
		{
			EditorGUILayout.BeginHorizontal();
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("idleAnimation"), new GUIContent("         Sprite Animation"), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
			if (!startAnimation && item.idleAnimation != null)
			{
				animationStartTime = EditorApplication.timeSinceStartup;
				startAnimation = true;
				EditorApplication.update += Animate;
			}
		}
		else
		{
			startAnimation = false;
			EditorApplication.update -= Animate;
			EditorGUILayout.BeginHorizontal();
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("itemSprite"), new GUIContent("         Sprite"), true, GUILayout.MaxWidth(500f));
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
			item.GetComponentInChildren<SpriteRenderer>().sprite = item.itemSprite;
		}
		EditorGUILayout.Space();
	}

	/// <summary>
	/// Display all the base properties of the item. All items should have these properties.
	/// </summary>
	public void DisplayItemProperties(bool stackable = true)
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
		DisplayItemName();
		DisplayItemTier();
		DisplayItemValue();
		if(stackable)
			DisplayStackable();
		DisplayDescription();
	}

	void OnDisable()
	{
		startAnimation = false;
		EditorApplication.update -= Animate;
	}

	/// <summary>
	/// Animate the sprite in the window.
	/// </summary>
	private void Animate()
	{
		var item = serializedObject.targetObject as Item;
		if (item == null || item.idleAnimation == null)
		{
			return;
		}
		if (EditorApplication.timeSinceStartup > changeSprite + animationStartTime)
		{
			var sprite =  item.idleAnimation.GetFrame(index);
			index = (index + 1) % item.idleAnimation.frames.Length;
			item.GetComponent<SpriteRenderer>().sprite = sprite;
			changeSprite += item.idleAnimation.playSpeed;
		}
	}
}
