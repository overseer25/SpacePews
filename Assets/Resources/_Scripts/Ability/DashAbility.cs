using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a dash ability, which accelerates the player forward at a high speed.
/// </summary>
[CreateAssetMenu(menuName = "Abilities/DashAbility")]
public class DashAbility : Ability
{
    /// <summary>
    /// The speed the dash maxes out to.
    /// </summary>
    public float speed;

    /// <summary>
    /// The rate at which the dash reaches it's max speed.
    /// </summary>
    public float acceleration;

    /// <summary>
    /// The rate at which the player returns to their max speed.
    /// </summary>
    public float deceleration;

    /// <summary>
    /// Activates the dash ability.
    /// </summary>
    public override void Activate()
    {
        throw new System.NotImplementedException();
    }
}
