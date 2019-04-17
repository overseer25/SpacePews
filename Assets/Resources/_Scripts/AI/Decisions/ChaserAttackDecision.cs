using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/Chaser Attack")]
public class ChaserAttackDecision : Decision
{
	public override bool Decide(StateController controller)
	{
		var actor = controller.actor as EnemyActor;
		var hits = Physics2D.CircleCastAll(controller.gameObject.transform.position, controller.actor.attackRange, controller.transform.up, controller.actor.attackDistance);
		foreach (var hit in hits)
		{
			if (hit.collider.CompareTag("Player"))
			{

				return true;
			}
		}
		return false;
	}
}
