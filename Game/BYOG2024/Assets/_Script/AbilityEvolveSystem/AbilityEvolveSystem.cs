using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Entity;
using Entity.Abilities;
using Entity.Player;
using UnityEngine;

namespace Combat
{
	public class AbilityEvolveSystem : MonoBehaviour
	{
		[SerializeField] private AbilityEvolution abilityEvolutionTable;
		[SerializeField] private EntityHealthManager playerHealthManager;
		[SerializeField] private PlayerAbilitySystem playerAbilitySystem;
		[SerializeField] private float healEachAbsorb;
		private Dictionary<EnemyTypes, int> _gluttonyCountOfRespectiveTypes = new Dictionary<EnemyTypes, int>();

		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
		}

		public void AbsorbKill(EnemyTypes type)
		{
			if (Enum.IsDefined(typeof(EnemyTypes), type))
			{
				if (!_gluttonyCountOfRespectiveTypes.ContainsKey(type))
					_gluttonyCountOfRespectiveTypes.Add(type, 0);
				_gluttonyCountOfRespectiveTypes[type] += 1;
				playerHealthManager.Heal(healEachAbsorb);

				//here we check and directly evolve ability
				for (int i = 0; i < 2; i++)
				{
					AbilityBase abilityBase = abilityEvolutionTable.NextEvolution(playerAbilitySystem.AbilityOwned[i]);
					if (abilityBase == null)
						break;
					List<EnemyTypes> enemyTypes = _gluttonyCountOfRespectiveTypes.Keys.ToList();
					foreach (EnemyTypes types in enemyTypes)
					{
						if (abilityBase.enemyTypeForEvolution.ContainsKey(types))
						{
							if (_gluttonyCountOfRespectiveTypes[types] >= abilityBase.enemyTypeForEvolution[types])
							{
								playerAbilitySystem.SwitchUsableAbility(playerAbilitySystem.AbilityOwned[i],abilityBase);
								_gluttonyCountOfRespectiveTypes[types] -= abilityBase.enemyTypeForEvolution[types];
							}
						}
					}
				}
			}
		}
	}

	[Flags]
	public enum EnemyTypes
	{
		NonElemental = 1 << 0,
		Fire = 1 << 1,
		Decay = 1 << 2,
		Lightning = 1 << 3
	}
}