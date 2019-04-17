using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Chaser Attack")]
public class ChaserAttackAction : ActionBase
{
	public override void Act(StateController controller)
	{
		Attack(controller);
	}

	private void Attack(StateController controller)
	{
		var actor = controller.actor as EnemyActor;
		var hits = Physics2D.CircleCastAll(controller.gameObject.transform.position, actor.attackRange, controller.transform.up, actor.attackDistance);
		foreach (var hit in hits)
		{
			if (hit.collider.CompareTag("Player") && controller.CheckIfCountdownElapsed(actor.dashCooldown))
			{
				controller.gameObject.GetComponent<EnemyMovementController>().Dash();
			}
		}
	}
}
