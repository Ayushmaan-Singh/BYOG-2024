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
		public readonly UnmanagedSubject<(float max, float current)> HealthSubject = new UnmanagedSubject<(float max, float current)>();

		//Ability System
		public readonly UnmanagedSubject<(int index, AbilityBase to)> AbilityChangedSubject = new UnmanagedSubject<(int index, AbilityBase to)>();
		public readonly UnmanagedSubject<(int index, AbilityBase.State state)> AbilityStateChangedSubject = new UnmanagedSubject<(int index, AbilityBase.State state)>();
		public readonly UnmanagedSubject<(int index, float amount)> AbilityProgressChangedSubject = new UnmanagedSubject<(int index, float amount)>();
		public readonly UnmanagedSubject<int> ActiveAbilitySwitchedSubject = new UnmanagedSubject<int>();

		private void Awake()
		{
			playerRTSet.RegisterPlayer(this).SetOwner(this);
		}

		private void OnDestroy()
		{
			playerRTSet.DeregisterPlayer(this).SetOwner(null);
		}

		private void OnEnable()
		{
			ServiceLocator.For(this).Register(this);
		}

		private void OnDisable()
		{
			ServiceLocator.For(this)?.Deregister(this);
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

		public AbilityBase GetAbilityAtIndex(int index) => index < abilitySystem.AbilityOwned.Length ? abilitySystem.AbilityOwned[index] : null;
		public AbilityBase.State? GetAbilityStateAtIndex(int index) => index < abilitySystem.AbilityOwned.Length ? abilitySystem.AbilityOwned[index].CurrentState : null;
		public float? AbilityProgressAtIndex(int index) => index < abilitySystem.AbilityOwned.Length ? abilitySystem.AbilityOwned[index].Progress : null;
		public bool? IsAbilityAtIndexActiveAbility(int index) => index < abilitySystem.AbilityOwned.Length ? abilitySystem.GetActiveAbility == abilitySystem.AbilityOwned[index] : null;
		public int ActiveAbilityIndex() => abilitySystem.ActiveAbilityIndex;

		public ConsumableEntity[] ConsumableAbilityOwned()
		{
			AbilityBase[] abilities = abilitySystem.AbilityOwned;
			abilities = abilities.Where(ability => ability.GetComponentInChildren<ConsumableEntity>() != null).ToArray();
			ConsumableEntity[] consumableAbilities = new ConsumableEntity[abilities.Length];
			int count = consumableAbilities.Length;
			for (int i = 0; i < count; i++)
				consumableAbilities[i] = abilities[i].GetComponentInChildren<ConsumableEntity>();
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