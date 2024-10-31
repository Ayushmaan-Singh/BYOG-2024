using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Combat.AbilityEvolution;
using Combat.Player;
using Combat.UI;
using Entity;
using Entity.Abilities;
using Entity.Player;
using Global;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Combat
{
	public class AbilityEvolveSystem : SerializedMonoBehaviour
	{
		[SerializeField] private PlayerRuntimeSet playerRTSet;
		[SerializeField] private AbilityTreeGraph abilityEvolutionTree;
		[SerializeField] private PrefabCollection abilityPrefabCollection;
		[SerializeField] private AbilityUnlockableUI unlockableUI;
		[SerializeField] private EvolutionUI evolutionUI;

		private readonly Dictionary<AbilityBase, AbilityTreeNode[]> _possibleEvolution;
		private readonly Dictionary<AbilityBase, AbilityTreeNode[]> _unlockableAbility;

		[OdinSerialize]private readonly Dictionary<ConsumableEntityType, int> _consumedEntities = new Dictionary<ConsumableEntityType, int>();
		public int this[ConsumableEntityType type] => _consumedEntities.GetValueOrDefault(type);

		private void OnEnable()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}

		// private IEnumerator Start()
		// {
		// 	yield return new WaitForSeconds(2);
		// 	TryGainAbility();
		// }

		private void OnDisable()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
		}

		public void ConsumeEntity(ConsumableEntity entity)
		{
			ConsumableEntityType[] consumableEntitiesArray = entity.EntityTypes;
			foreach (ConsumableEntityType consumableEntityType in consumableEntitiesArray)
			{
				if (!_consumedEntities.TryAdd(consumableEntityType, 1))
					_consumedEntities[consumableEntityType] += 1;
			}
		}
		
		public void TryEvolving()
		{
			AbilityBase[] abilities = playerRTSet.Owner.AbilityOwned;
			Dictionary<AbilityBase, AbilityTreeNode[]> abilityEvolutions = new Dictionary<AbilityBase, AbilityTreeNode[]>();

			int count = abilities.Length;
			for (int i = 0; i < count; i++)
			{
				AbilityTreeNode[] evolved = abilityEvolutionTree.FindEvolutionsOfThisAbility(_consumedEntities, abilities[i]);
				if (evolved is { Length: > 0 })
					abilityEvolutions.Add(abilities[i], evolved);
			}
			if (abilityEvolutions.Count > 0)
			{
				evolutionUI.Reset();
				evolutionUI.EvolutionAbility(abilityEvolutions);
				evolutionUI.Activate();
			}
		}
		public void TryGainAbility()
		{
			ConsumableEntity[] abilities = playerRTSet.Owner.ConsumableAbilityOwned();
			Dictionary<ConsumableEntityType, int> resourcesAvailable = new Dictionary<ConsumableEntityType, int>(_consumedEntities);

			int count = abilities.Length;
			for (int i = 0; i < count; i++)
			{
				ConsumableEntityType[] consumableEntitiesArray = abilities[i].EntityTypes;
				foreach (ConsumableEntityType consumableEntityType in consumableEntitiesArray)
				{
					if (!resourcesAvailable.TryAdd(consumableEntityType, 1))
						resourcesAvailable[consumableEntityType] += 1;
				}
			}
			AbilityTreeNode[] gainableAbility = abilityEvolutionTree.FindGainableAbility(resourcesAvailable);

			if (gainableAbility.Length > 0)
			{
				unlockableUI.Reset();
				unlockableUI.UnlockableAbility(gainableAbility);
				unlockableUI.Activate();
			}
		}
	}
}