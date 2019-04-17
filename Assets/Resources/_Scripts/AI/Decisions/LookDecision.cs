using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Decisions/Look")]
public class LookDecision : Decision
{
	public override bool Decide(StateController controller)
	{
		return Look(controller);
	}

	private bool Look(StateController controller)
	{
		var hits = Physics2D.CircleCastAll(controller.gameObject.transform.position, controller.actor.detectionRange, controller.transform.up, controller.actor.detectionDistance);
		Debug.DrawRay(controller.gameObject.transform.position, controller.transform.up * controller.actor.detectionDistance, Color.green);

		foreach (var hit in hits)
		{
			if(hit.collider.CompareTag("Player") && !hit.transform.gameObject.GetComponent<PlayerHealthController>().IsDead())
			{
				controller.targetToChase = hit.transform;
				return true;
			}
		}

		return false;
	}
}
