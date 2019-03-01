using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic class used for designing a player's active ability. Upgrade components that the player collects can give them
/// an active ability.
/// </summary>
public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public string abilityDescription;
    public float cooldown;
    public bool recharging = false;

    /// <summary>
    /// Activate the ability.
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// Begin the cooldown for the ability. The ability cannot be used again until the cooldown is complete.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Cooldown()
    {
        if(!recharging)
        {
            recharging = true;
            yield return new WaitForSeconds(cooldown);
            recharging = false;
        }
    }
}
