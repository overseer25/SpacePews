using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager class for the player to add and remove timed buffs.
/// It also manages the buff grid below the player health.
/// </summary>
public class BuffManager : MonoBehaviour
{
	public static BuffManager current;
	public GameObject buffGrid;
	public BuffIcon defaultIcon;
	public Actor playerActor;

	// A data structure associating the buff to an index in the buff grid.
	private List<Buff> appliedBuffs;
	private List<BuffIcon> buffIcons;

	// Start is called before the first frame update
	void Start()
    {
		if (current == null)
		{
			current = this;
			appliedBuffs = new List<Buff>();
			buffIcons = new List<BuffIcon>();
		}
	}

	/// <summary>
	/// Apply a buff to the player, and add it's icon to the buff grid.
	/// </summary>
	public void AddBuff(Buff buff)
	{
		if(buff.icon != null)
		{
			var icon = Instantiate(defaultIcon, buffGrid.transform);
			icon.Initialize(buff);
			buffIcons.Add(icon);
			appliedBuffs.Add(icon.buff);
			icon.buff.Apply(playerActor);
		}
	}

	/// <summary>
	/// Remove a buff from the player, and remove the icon from the buff grid.
	/// </summary>
	/// <param name="buff"></param>
	public void RemoveBuff(Buff buff)
	{
		foreach(var icon in buffIcons)
		{
			if(icon.buff == buff)
			{
				icon.buff.Remove(playerActor);
				appliedBuffs.Remove(icon.buff);
				buffIcons.Remove(icon);
				if(icon.gameObject != null)
					Destroy(icon.gameObject);
				return;
			}
		}
	}

	/// <summary>
	/// Remove all the buffs on the player, and disables all icons.
	/// </summary>
	public void RemoveAllBuffs()
	{
		StopAllCoroutines();
		foreach(var buff in appliedBuffs)
		{
			RemoveBuff(buff);
		}
	}
}
