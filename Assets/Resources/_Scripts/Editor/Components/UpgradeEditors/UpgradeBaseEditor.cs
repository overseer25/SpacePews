using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpgradeComponentBase))]
public class UpgradeBaseEditor : ComponentBaseEditor
{

	public void DisplayPropertiesSection(UpgradeComponentBase component)
	{
		base.DisplayPropertiesSection();
		component.itemType = ItemType.Upgrade;
		component.visible = false;

	}

	public override void DisplayAudioVisualSection(ShipComponentBase component)
	{
		base.DisplayAudioVisualSection(component);
	}
}
