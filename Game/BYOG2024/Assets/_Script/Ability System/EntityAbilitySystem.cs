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
		[SerializeField, InlineProperty] protected List<AbilityBase> abilitySystem;

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