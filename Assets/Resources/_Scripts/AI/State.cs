using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/State")]
public class State : ScriptableObject
{
	public ActionBase[] actions;
	public StateTransition[] transitions;
	public Color gizmoColor = Color.grey;

    public void UpdateState(StateController controller)
	{
		DoActions(controller);
		CheckTransitions(controller);
	}

	private void DoActions(StateController controller)
	{
		foreach(var action in actions)
		{
			action.Act(controller);
		}
	}

	/// <summary>
	/// Check each decision to see if a transition needs to be made to a new state.
	/// </summary>
	/// <param name="controller"></param>
	private void CheckTransitions(StateController controller)
	{
		foreach(var transition in transitions)
		{
			bool decisionSucceeded = transition.decision.Decide(controller);

			if (decisionSucceeded)
				controller.TransitionToState(transition.trueState);
			else
				controller.TransitionToState(transition.falseState);
		}
	}
}
