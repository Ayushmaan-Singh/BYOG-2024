using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using AstekUtility.Input;
using Combat;
using Entity.Player;
using Global.Pool;
using UnityEngine;

namespace Entity.Abilities
{
	public class DarkArrowVolley : AbilityBase
	{
		[Header("Ability Data")]
		[SerializeField] private float damagePerHit;
		[SerializeField] private float cooldown;
		[SerializeField] private GameObject attackPrefab;

		[Header("Visuals")]
		[SerializeField] private Transform indicatorHolder;
		[SerializeField] private ParticleSystem[] indicators;

		private bool _indicatorOn = false;

		private CountdownTimer _cooldownTimer;

		private void Awake()
		{
			_cooldownTimer = new CountdownTimer(cooldown);
			_cooldownTimer.OnTimerStop += () =>
			{
				_cooldownTimer.Reset();
				CurrentState = State.Usable;
			};
		}

		private void Update()
		{
			switch (CurrentState)
			{

				case State.Usable:

					indicatorHolder.position = ServiceLocator.For(this).Get<EntityAbilitySystem>().AimAt ?? Vector3.zero;
					if (!_indicatorOn && ServiceLocator.For(this).Get<EntityAbilitySystem>().GetActiveAbility == this)
					{
						_indicatorOn = true;
						foreach (ParticleSystem indicator in indicators)
						{
							indicator.gameObject.SetActive(true);
							indicator.Play();
						}
					}

					if (_indicatorOn && ServiceLocator.For(this).Get<EntityAbilitySystem>().GetActiveAbility != this)
					{
						_indicatorOn = false;
						foreach (ParticleSystem indicator in indicators)
						{
							indicator.Stop();
							indicator.Clear();
						}
					}

					break;

				case State.Unusable:

					_cooldownTimer.Tick(Time.deltaTime);
					if (_indicatorOn)
					{
						_indicatorOn = false;
						foreach (ParticleSystem indicator in indicators)
						{
							indicator.Stop();
							indicator.Clear();
						}
					}

					break;

				case State.InProgress:

					if (_indicatorOn)
					{
						_indicatorOn = false;
						foreach (ParticleSystem indicator in indicators)
						{
							indicator.Stop();
							indicator.Clear();
						}
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void Execute()
		{
			if (CurrentState != State.Usable)
				return;

			CurrentState = State.Unusable;
			Transform holder = ServiceLocator.ForSceneOf(this).Get<ParticleEffectsInGameObjectPool>().transform;
			GameObject spawn = Instantiate(attackPrefab, ServiceLocator.For(this).Get<EntityAbilitySystem>().AimAt??Vector3.zero, Quaternion.identity, holder);
			spawn.GetComponentInChildren<ParticleEffectManager>().SetAbility(this, damagePerHit);
			spawn.GetComponentInChildren<ParticleEffectManager>().ParticleEffectOn();
			_cooldownTimer.Start();
		}
		public override void CancelExecution()
		{
			//noap
		}
	}
}