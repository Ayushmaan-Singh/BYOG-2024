using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Combat.AbilityEvolution;
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
		[SerializeField] private AbilityTreeGraph abilityEvolutionTree;
		[SerializeField] private PrefabCollection abilityPrefabCollection;
		[SerializeField] private AbilityUnlockableUI unlockableUI;
		[SerializeField] private EvolutionUI evolutionUI;

		private readonly Dictionary<AbilityBase, AbilityTreeNode[]> _possibleEvolution;
		private readonly Dictionary<AbilityBase, AbilityTreeNode[]> _unlockableAbility;

		[OdinSerialize]private readonly Dictionary<ConsumableEntities, int> _consumedEntities = new Dictionary<ConsumableEntities, int>();
		public int this[ConsumableEntities type] => _consumedEntities.GetValueOrDefault(type);

		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}

		// private IEnumerator Start()
		// {
		// 	yield return new WaitForSeconds(2);
		// 	TryGainAbility();
		// }

		private void OnDestroy()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
		}

		public void ConsumeEntity(Consumable entity)
		{
			_consumedEntities.TryAdd(entity.EntityType, 0);
			_consumedEntities[entity.EntityType]++;
		}
		
		public void TryEvolving()
		{
			AbilityBase[] abilities = ServiceLocator.ForSceneOf(this).Get<PlayerMediator>().AbilityOwned;
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
			Consumable[] abilities = ServiceLocator.ForSceneOf(this).Get<PlayerMediator>().ConsumableAbilityOwned();
			Dictionary<ConsumableEntities, int> resourcesAvailable = new Dictionary<ConsumableEntities, int>(_consumedEntities);

			int count = abilities.Length;
			for (int i = 0; i < count; i++)
			{
				if (!resourcesAvailable.ContainsKey(abilities[i].EntityType))
					resourcesAvailable.Add(abilities[i].EntityType, 1);
				else
					resourcesAvailable[abilities[i].EntityType] += 1;
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