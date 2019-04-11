using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityComponent : UpgradeComponentBase
{
	/// <summary>
	/// The ability this component gives.
	/// </summary>
	public AbilityBase ability;

	/// <summary>
	/// Given the player, sets the ability of the player, or removes it.
	/// </summary>
	public void SetMounted(PlayerController player, bool val)
	{
		base.SetMounted(val);
		if (val)
			player.SetAbility(ability);
		else
			player.SetAbility(null);
	}

	/// <summary>
	/// The text that will be displayed in the info screen of the component.
	/// </summary>
	/// <returns></returns>
	public string BuildInfoScreenText()
	{
		var result = "Adds the ability to dash. Press the <b>Ability</b> button to use.";

		return result;
	}
}
