using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action that an enemy does when patrolling.
/// </summary>
[CreateAssetMenu(menuName ="AI/Actions/Enemy Patrol")]
public class EnemyPatrolAction : ActionBase
{
	public override void Act(StateController controller)
	{
		Patrol(controller);
	}

	/// <summary>
	/// Patrol an area, based on the waypoints of the controller.
	/// </summary>
	/// <param name="controller"></param>
	private void Patrol(StateController controller)
	{
		GameObject waypoint;
		var remainingDistance = (controller.gameObject.transform.position - controller.GetCurrentWaypoint().transform.position).sqrMagnitude;

		if (remainingDistance > 0.2f)
			waypoint = controller.GetCurrentWaypoint();
		else
			waypoint = controller.GetNextWaypoint();

		controller.gameObject.GetComponent<EnemyMovementController>().MoveTowardPoint(waypoint.transform.position);
	}
}
