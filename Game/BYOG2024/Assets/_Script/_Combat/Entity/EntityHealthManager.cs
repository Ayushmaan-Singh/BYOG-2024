using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.VisualFeedback;
using Combat;
using Entity.Abilities;
using UnityEngine;

namespace Entity
{
	public class EntityHealthManager : MonoBehaviour
	{
		[SerializeField] private MeshFlashFX meshFlashFX;
		[SerializeField] private float bodyDecayAfter;

		private float _currentHP;
		public bool IsAlive => _currentHP > 0;
		public float CurrentHP => _currentHP;

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		private void Start()
		{
			_currentHP = ServiceLocator.For(this).Get<EntityStatSystem>().GetInstanceStats(Stats.Hp);
		}

		public void Damage(float amount)
		{
			if (IsAlive)
			{
				_currentHP = Mathf.Clamp(_currentHP - amount, 0, ServiceLocator.For(this).Get<EntityStatSystem>().GetInstanceStats(Stats.Hp));
				if (amount > 0)
					meshFlashFX?.Play();
				
				if (!IsAlive)
				{
					//Death Animation
					Invoke(nameof(DestroyBodyAfter),bodyDecayAfter);
				}
			}
		}

		public void Heal(float amount)
		{
			if (IsAlive)
				_currentHP = Mathf.Clamp(_currentHP + amount, 0, ServiceLocator.For(this).Get<EntityStatSystem>().GetInstanceStats(Stats.Hp));
		}

		private void DestroyBodyAfter()
		{
			Destroy(ServiceLocator.For(this).gameObject);
		}
	}
}