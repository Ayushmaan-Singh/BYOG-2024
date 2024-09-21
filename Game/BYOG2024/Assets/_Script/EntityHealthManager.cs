using System;
using System.Collections;
using System.Collections.Generic;
using _Script;
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
		[SerializeField] private DamageBlinking damageBlinking;
		[SerializeField] private DamageBlinking gluttonyConsumption;
		[SerializeField] private float bodyDecayAfter;

		private CoroutineTask _gettingAbsorbedTask;

		private float _currentHP;
		public bool IsAlive => _currentHP > 0;
		public float CurrentHP => _currentHP;

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
			_gettingAbsorbedTask = new CoroutineTask(GettingAbsorbedVisual(), this, false);
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
				if (damageBlinking != null && amount > 0)
					damageBlinking.Play();
				if (!IsAlive)
				{
					//Death Animation
					StartCoroutine(DestroyAfter());
				}
			}
		}

		public void Heal(float amount)
		{
			if (IsAlive)
				_currentHP = Mathf.Clamp(_currentHP + amount, 0, ServiceLocator.For(this).Get<EntityStatSystem>().GetInstanceStats(Stats.Hp));
		}

		private void OnParticleCollision(GameObject particle)
		{
			if (!CompareTag("Enemy"))
				return;
			
			ParticleEffectMediator effectMediator = particle.GetComponentInParent<ParticleEffectMediator>();
			if (effectMediator)
			{
				Damage(effectMediator.Damage);
			}
		}

		public void GettingEatenByGluttony()
		{
			if (IsAlive || _gettingAbsorbedTask.Running)
				return;

			StopCoroutine(nameof(DestroyAfter));
			ServiceLocator.ForSceneOf(this).Get<AbilityEvolveSystem>().AbsorbKill
				(ServiceLocator.For(this).Get<EnemyController>().Type);
			_gettingAbsorbedTask.Start();
		}

		private IEnumerable GettingAbsorbedVisual()
		{
			gluttonyConsumption.Play();
			yield return new WaitWhile(() =>
				gluttonyConsumption.CurrentState == DamageBlinking.EffectState.Running
				|| gluttonyConsumption.CurrentState == DamageBlinking.EffectState.CanBeExecuted);
			Destroy(ServiceLocator.For(this).gameObject);
		}

		private IEnumerator DestroyAfter()
		{
			yield return new WaitForSeconds(bodyDecayAfter);
			Destroy(ServiceLocator.For(this).gameObject);
		}
	}
}