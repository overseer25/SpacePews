using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Enemy Chase")]
public class EnemyChaseAction : ActionBase
{
	public override void Act(StateController controller)
	{
		Chase(controller);
	}

	private void Chase(StateController controller)
	{
		controller.gameObject.GetComponent<EnemyMovementController>().MoveTowardPoint(controller.targetToChase.position);
	}
}
