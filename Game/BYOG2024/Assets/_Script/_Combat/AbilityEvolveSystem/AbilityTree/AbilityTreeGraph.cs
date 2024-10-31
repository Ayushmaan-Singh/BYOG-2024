using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Abilities;
using UnityEngine;
using XNode;

namespace Combat.AbilityEvolution
{
	public class AbilityTreeGraph : NodeGraph
	{
		/// <summary>
		/// These are nodes that don't have a prev node
		/// </summary>
		[SerializeField, Tooltip("Nodes that don't have a prev node")] private AbilityTreeNode[] startNodes;
		[SerializeField] private AbilityTreeNode[] gainableAbilities;


		public AbilityTreeNode[] FindGainableAbility(Dictionary<ConsumableEntityType, int> resourcesAvailable) => gainableAbilities.Where(abilityNode => abilityNode.CanGainAbility(resourcesAvailable)).ToArray();

		/// <summary>
		/// Returns a list of possible evolutions for current ability
		/// </summary>
		/// <param name="resourcesAvailable"></param>
		/// <param name="ability"></param>
		/// <returns></returns>
		public AbilityTreeNode[] FindEvolutionsOfThisAbility(Dictionary<ConsumableEntityType, int> resourcesAvailable, AbilityBase ability)
		{
			return GetNodeInGraph(ability.GetType())?.NextAbilities?.Where(abilityNode => abilityNode.CanEvolveAbility(resourcesAvailable)).ToArray();
		}

		/// <summary>
		/// Same as above but uses last known node and is better for performance
		/// </summary>
		/// <param name="resourcesAvailable"></param>
		/// <param name="ability"></param>
		/// <returns></returns>
		public AbilityTreeNode[] FindEvolutionsOfThisAbility(Dictionary<ConsumableEntityType, int> resourcesAvailable, AbilityTreeNode ability)
		{
			List<AbilityTreeNode> nextAbilities = ability.NextAbilities;
			return nextAbilities.Where(abilityNode => abilityNode.CanEvolveAbility(resourcesAvailable)).ToArray();
		}

		private AbilityTreeNode GetNodeInGraph(Type abilityType)
		{
			int count = startNodes.Length;
			for (int i = 0; i < count; i++)
			{
				AbilityTreeNode node = SearchTreeRecursivelyForAbility(startNodes[i], abilityType);
				if (node != null)
					return node;
			}
			return null;
		}

		private AbilityTreeNode SearchTreeRecursivelyForAbility(AbilityTreeNode treeNode, Type abilityType)
		{
			if (treeNode.AbilityBaseType == abilityType)
				return treeNode;

			List<AbilityTreeNode> abilityNode = treeNode.NextAbilities;

			if (abilityNode == null || !abilityNode.Any())
				return null;
			
			int count = abilityNode.Count;
			for (int i = 0; i < count; i++)
			{
				AbilityTreeNode node = SearchTreeRecursivelyForAbility(abilityNode[i], abilityType);
				if (node != null)
					return node;
			}

			return null;
		}
	}
}