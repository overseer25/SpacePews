using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UpgradeBaseEditor : ComponentBaseEditor
{

	public void DisplayPropertiesSection(UpgradeComponentBase component)
	{
		base.DisplayPropertiesSection();
		component.itemType = ItemType.Upgrade;
		component.visible = false;

	}
}
