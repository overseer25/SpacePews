using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A useable item that applies temporary buffs to the player.
/// </summary>
public class Consumable : Item
{
	public Buff[] Buffs;

    /// <summary>
    /// Use the item.
    /// </summary>
    public void Consume()
	{
		foreach(var buff in Buffs)
		{
			BuffManager.current.AddBuff(buff);
		}
	}
}
