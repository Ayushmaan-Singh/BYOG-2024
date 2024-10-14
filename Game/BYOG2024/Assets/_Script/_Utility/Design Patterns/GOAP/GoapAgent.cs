using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace AstekUtility.DesignPattern.GOAP
{
	public abstract class GoapAgent : MonoBehaviour
	{
		protected AgentGoal _lastGoal;
		public AgentGoal CurrentGoal;
		public ActionPlan ActionPlan;
		public AgentAction CurrentAction;

		public Dictionary<string, Belief> Beliefs;
		public HashSet<AgentAction> Actions;
		public HashSet<AgentGoal> Goals;

		protected IGoapPlanner _planner;

		/// <summary>
		/// This always runs in FrameUpdate
		/// </summary>
		protected void RunGoap()
		{
			//FrameUpdate the plan and current action if there is one
			if (CurrentAction == null)
			{
				CalculatePlan();
				if (ActionPlan != null && ActionPlan.Actions.Count > 0)
				{
					CurrentGoal = ActionPlan.AgentGoal;
					CurrentAction = ActionPlan.Actions.Pop();
					CurrentAction.Start();
				}
			}

			//if we have a current action execute it
			if (ActionPlan != null && CurrentAction != null)
			{
				CurrentAction.Update(Time.deltaTime);
				if (CurrentAction.Complete)
				{
					CurrentAction.Stop();
					CurrentAction = null;

					if (ActionPlan.Actions.Count == 0)
					{
						_lastGoal = CurrentGoal;
						CurrentGoal = null;
					}
				}
			}
		}

		protected void CalculatePlan()
		{
			float priorityLevel = CurrentGoal?.Priority ?? 0;

			HashSet<AgentGoal> goalsToCheck = Goals;

			//If we have a current goal, we only want to check goals with higher priority
			if (CurrentGoal != null)
			{
				goalsToCheck = new HashSet<AgentGoal>(Goals.Where(g => g.Priority > priorityLevel));
			}

			ActionPlan potentialPlans = _planner.Plan(this, goalsToCheck, _lastGoal);
			if (potentialPlans != null)
				ActionPlan = potentialPlans;
		}
	}
}