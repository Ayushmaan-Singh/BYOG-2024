using System;
using System.Collections.Generic;
using System.Linq;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Entity.Abilities;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Combat.AbilityEvolution
{
	[NodeWidth(300)]
	public class AbilityTreeNode : Node
	{
		[Input] public int PrevNode;
		[Output] public int NextNode;

		[SerializeField, Space] private GameObject abilityBaseGameObject;
		public Type AbilityBaseType => abilityBaseGameObject.GetComponentInChildren<AbilityBase>().GetType();

		[SerializeField, InlineProperty, Space, TableList] private List<AbilityRequirements> gainRequirements = new List<AbilityRequirements>();
		[SerializeField, InlineProperty, TableList] private List<AbilityRequirements> evolveRequirements = new List<AbilityRequirements>();

		public List<AbilityTreeNode> NextAbilities => GetOutputPort("NextNode").GetConnections().Select(port => port.node as AbilityTreeNode).ToList();
		public List<AbilityTreeNode> PrevAbilities => GetInputPort("PrevNode").GetConnections().Select(port => port.node as AbilityTreeNode).ToList();

		public bool CanGainAbility([NotNull] Dictionary<ConsumableEntities, int> resourcesAvailable)
		{
			if (resourcesAvailable == null)
				throw new ArgumentNullException(nameof(resourcesAvailable));
			return gainRequirements.Count > 0
			       && gainRequirements.All(requirement =>
				       resourcesAvailable.ContainsKey(requirement.Consumable) && resourcesAvailable[requirement.Consumable] >= requirement.Count);
		}

		public bool CanEvolveAbility([NotNull] Dictionary<ConsumableEntities, int> resourcesAvailable)
		{
			if (resourcesAvailable == null)
				throw new ArgumentNullException(nameof(resourcesAvailable));
			return evolveRequirements.Count > 0
			       && evolveRequirements.All(requirement =>
				       resourcesAvailable.ContainsKey(requirement.Consumable) && resourcesAvailable[requirement.Consumable] >= requirement.Count);
		}


		[System.Serializable]
		public class AbilityRequirements
		{
			[field:SerializeField] public ConsumableEntities Consumable { get; private set; }
			[field:SerializeField] public int Count { get; private set; }
		}
	}
}