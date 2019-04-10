using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a dash ability, which accelerates the player forward at a high speed.
/// </summary>
[CreateAssetMenu(menuName = "Abilities/Dash Ability")]
public class DashAbility : Ability
{
    /// <summary>
    /// The speed the dash maxes out to.
    /// </summary>
    public float speed;

    /// <summary>
    /// Activates the dash ability.
    /// </summary>
    public override void Activate(GameObject player)
    {
		var movementController = player.GetComponent<MovementController>();
		movementController.MoveDirection(movementController.GetShip().transform.up * speed);

        if(useEffect != null)
            ParticleManager.PlayParticle(useEffect, player.transform.position, movementController.GetShip().transform.rotation);
    }
}
