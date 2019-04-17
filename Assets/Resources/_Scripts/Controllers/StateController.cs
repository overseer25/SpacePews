using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
	public State currentState;
	public bool active;
	public bool visualization;
	public ActorBase actor;
	public State remainState;
	public GameObject[] waypoints;
	private int currentWaypoint;
	private float stateTimeElapsed;
	[HideInInspector] public Transform targetToChase;

	void Update()
	{
		if (!active || currentState == null)
			return;
		currentState.UpdateState(this);
	}

	/// <summary>
	/// Get the next waypoint.
	/// </summary>
	/// <returns></returns>
	public GameObject GetNextWaypoint()
	{
		currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
		return waypoints[currentWaypoint];
	}

	/// <summary>
	/// Get the current waypoint.
	/// </summary>
	/// <returns></returns>
	public GameObject GetCurrentWaypoint()
	{
		return waypoints[currentWaypoint];
	}

	/// <summary>
	/// Transition to a new state.
	/// </summary>
	/// <param name="newState"></param>
	public void TransitionToState(State newState)
	{
		if(newState != remainState)
		{
			currentState = newState;
			OnExitState();
		}
	}

	/// <summary>
	/// Check if an amount of time has passed.
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	public bool CheckIfCountdownElapsed(float duration)
	{
		stateTimeElapsed += Time.deltaTime;
		if (stateTimeElapsed >= duration)
		{
			stateTimeElapsed = 0;
			return true;
		}
		else
			return false;
	}

	private void OnExitState()
	{
		stateTimeElapsed = 0;
	}

	/// <summary>
	/// Visualization for state.
	/// </summary>
	void OnDrawGizmos()
	{
		if (!visualization)
			return;

		if(currentState != null && actor != null && active)
		{
			Gizmos.color = currentState.gizmoColor;
			Gizmos.DrawWireSphere(gameObject.transform.position, actor.detectionRange);
		}
	}
}
