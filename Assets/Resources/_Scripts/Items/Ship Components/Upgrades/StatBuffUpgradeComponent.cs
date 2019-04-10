using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A stat buff upgrade component. These kinds of components can apply several buffs to the player.
/// </summary>
public class StatBuffUpgradeComponent : UpgradeComponentBase
{
	/// <summary>
	/// The buffs that will be applied to the player.
	/// </summary>
	public Buff[] buffs;

	/// <summary>
	/// Apply the buff list to the player.
	/// </summary>
	/// <param name="actor"></param>
    public void ApplyAllBuffs(Actor actor)
	{
		foreach(var buff in buffs)
		{
			buff.Apply(actor);
		}
	}

	/// <summary>
	/// Remove all the buffs added by this component.
	/// </summary>
	/// <param name="actor"></param>
	public void RemoveAllBuffs(Actor actor)
	{
		foreach (var buff in buffs)
		{
			buff.Remove(actor);
		}
	}

	/// <summary>
	/// Mount or unmount the stat buff component, and add/remove buffs on the actor.
	/// </summary>
	/// <param name="actor">The actor on which to apply/remove the buffs.</param>
	/// <param name="val">mounting or unmounting.</param>
	public void SetMounted(Actor actor, bool val)
	{
		base.SetMounted(val);
		if (val)
			ApplyAllBuffs(actor);
		else
			RemoveAllBuffs(actor);
	}
}
