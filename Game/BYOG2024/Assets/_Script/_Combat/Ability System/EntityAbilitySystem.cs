using System;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Entity.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entity
{
	public class EntityAbilitySystem : MonoBehaviour
	{
		[Header("Entity Ability System")]
		[SerializeField, InlineProperty] protected AbilityBase[] abilities;
		[SerializeField] protected Transform abilityHolder;

		protected AbilityBase _activeAbility;
		protected int _activeAbilityIndex = 0;
		
		public AbilityBase[] GetAbilities => abilities;
		public AbilityBase GetActiveAbility => _activeAbility;
		
		public Vector3? AimAt { get; protected set; }

		protected void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		protected void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}
	}
}