using System;
using System.Collections.Generic;
using System.Linq;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Observer.Unmanaged;
using Combat;
using Combat.Player;
using Entity.Abilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Entity.Player
{
	public class PlayerMediator : MonoBehaviour
	{
		[field:SerializeField] public Rigidbody MainBodyRb { get; private set; }
		[field:SerializeField] public Transform MainMovementBody { get; private set; }
		[field:SerializeField] public Transform MainRotationBody { get; private set; }
		[field:SerializeField] public Collider ColliderDetectedByContextSteering { get; private set; }

		[SerializeField] private PlayerRuntimeSet playerRTSet;
		[SerializeField] private PlayerController playerController;
		[SerializeField] private EntityHealthManager healthManager;
		[SerializeField] private EntityStatSystem statSystem;
		[SerializeField] private PlayerAbilitySystem abilitySystem;

		public Type playerType => playerController.GetType();
		public AbilityBase[] AbilityOwned => abilitySystem.AbilityOwned;

		//Observed Subjects

		//Health Manager
		public readonly UnmanagedSubject<(float max, float current)> healthSubject = new UnmanagedSubject<(float max, float current)>();

		//Ability System
		public readonly UnmanagedSubject<(int index, AbilityBase to)> AbilityChangedSubject = new UnmanagedSubject<(int index, AbilityBase to)>();
		public readonly UnmanagedSubject<(int index, AbilityBase.State state)> AbilityStateChangedSubject = new UnmanagedSubject<(int index, AbilityBase.State state)>();
		public readonly UnmanagedSubject<(int index, float amount)> AbilityProgressChangedSubject = new UnmanagedSubject<(int index, float amount)>();
		public readonly UnmanagedSubject<int> ActiveAbilitySwitchedSubject = new UnmanagedSubject<int>();

		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
			playerRTSet.RegisterPlayer(this).SetOwner(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
			playerRTSet.DeregisterPlayer(this).SetOwner(null);
		}

		#region Health Manager

		public float CurrentHp => healthManager.CurrentHP;
		public float MaxHp => statSystem.GetInstanceStats(Stats.Hp);
		
		public void Damage(float amount)
		{
			healthManager.Damage(amount);
		}

		public void Heal(float amount)
		{
			healthManager.Heal(amount);
		}

		#endregion

		#region Ability System

		public Consumable[] ConsumableAbilityOwned()
		{
			AbilityBase[] abilities = abilitySystem.AbilityOwned;
			abilities = abilities.Where(ability => ability.GetComponentInChildren<Consumable>() != null).ToArray();
			Consumable[] consumableAbilities = new Consumable[abilities.Length];
			int count = consumableAbilities.Length;
			for (int i = 0; i < count; i++)
				consumableAbilities[i] = abilities[i].GetComponentInChildren<Consumable>();
			return consumableAbilities;
		}

		public void ChangeAbility(AbilityBase switchFrom, AbilityBase switchTo)
		{
			if (abilitySystem.TryChangeAbility(switchFrom, switchTo))
			{
				AbilityChangedSubject.Notify((abilitySystem.AbilityOfTypeAtIndex(switchFrom), switchTo));
			}
		}

		public void ChangeAbility(int index, AbilityBase switchTo)
		{
			if (abilitySystem.TryChangeAbility(index, switchTo))
			{
				AbilityChangedSubject.Notify((index, switchTo));
			}
		}

		#endregion

	}
}