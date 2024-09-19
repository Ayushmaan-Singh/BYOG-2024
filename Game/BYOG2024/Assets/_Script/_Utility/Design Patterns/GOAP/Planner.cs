using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace AstekUtility.DesignPattern.GOAP
{
	public interface IGoapPlanner
	{
		ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null);
	}

	public class GoapPlanner : IGoapPlanner
	{

		public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null)
		{
			//Order goals by priority ascending
			List<AgentGoal> orderedGoals = goals
				.Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
				.OrderByDescending(g => g == mostRecentGoal ? g.Priority - 0.01 : g.Priority)
				.ToList();

			//Try to solve each goal in order, we use depth first search
			foreach (AgentGoal goal in orderedGoals)
			{
				Node goalNode = new Node(null, null, goal.DesiredEffects, 0);

				//If we can find a path to the goal, return the plan
				if (FindPath(goalNode, agent.Actions))
				{
					if (goalNode.IsLeafDead)
						continue;

					Stack<AgentAction> actionStack = new Stack<AgentAction>();
					while (goalNode.Leaves.Count > 0)
					{
						Node cheapestLeaf = goalNode.Leaves.OrderBy(leaf => leaf.Cost).First();
						goalNode = cheapestLeaf;
						actionStack.Push(cheapestLeaf.Action);
					}
					return new ActionPlan(goal, actionStack, goalNode.Cost);
				}
			}
			
			Debug.LogWarning("No plan found");
			return null;
		}

		private bool FindPath(Node parent, HashSet<AgentAction> actions)
		{
			foreach (AgentAction action in actions)
			{
				HashSet<Belief> requiredEffects = parent.RequiredEffects;

				//Remove any effects that evaluate to true, there is no action to take
				requiredEffects.RemoveWhere(b => b.Evaluate());

				//If there are no required effects to fulfill, we have a plan
				if (requiredEffects.Count == 0)
					return true;

				if (action.Effects.Any(requiredEffects.Contains))
				{
					HashSet<Belief> newRequiredEffects = new HashSet<Belief>(requiredEffects);
					newRequiredEffects.ExceptWith(action.Effects);
					newRequiredEffects.UnionWith(action.Precondition);

					HashSet<AgentAction> newAvailableActions = new HashSet<AgentAction>(actions);
					newAvailableActions.Remove(action);

					Node newNode = new Node(parent, action, newRequiredEffects, parent.Cost + action.Cost);

					//Explore the new node recursively
					if (FindPath(newNode, newAvailableActions))
					{
						parent.Leaves.Add(newNode);
						newRequiredEffects.ExceptWith(newNode.Action.Precondition);
					}

					//If all effects at this depth have been satisfied, return true;
					if (newRequiredEffects.Count == 0)
						return true;
				}
			}
			return false;
		}
	}

	public class Node
	{
		public Node Parent { get; }
		public AgentAction Action { get; }
		public HashSet<Belief> RequiredEffects { get; }
		public List<Node> Leaves { get; }
		public float Cost { get; }

		public bool IsLeafDead => Leaves.Count == 0 && Action == null;

		public Node(Node parent, AgentAction action, HashSet<Belief> effects, float cost)
		{
			Parent = parent;
			Action = action;
			RequiredEffects = new HashSet<Belief>();
			Leaves = new List<Node>();
			Cost = cost;
		}
	}

	public class ActionPlan
	{
		public AgentGoal AgentGoal { get; }
		public Stack<AgentAction> Actions;
		public float TotalCost { get; set; }

		public ActionPlan(AgentGoal goal, Stack<AgentAction> actions, float totalCost)
		{
			AgentGoal = goal;
			Actions = actions;
			TotalCost = totalCost;
		}
	}
}