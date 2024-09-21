using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using AstekUtility.Input;
using Combat;
using Entity.Abilities;
using Entity.Player;
using UnityEngine;

namespace Entity.Abilities
{
	public class Minefield : AbilityBase
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
				_currentState = State.Usable;
			};
		}

		private void Update()
		{
			switch (_currentState)
			{

				case State.Usable:

					indicatorHolder.position = ServiceLocator.Global.Get<InputUtils.MousePosition>().Invoke();
					if (!_indicatorOn && ServiceLocator.For(this).Get<PlayerAbilitySystem>().GetActiveAbility == this)
					{
						_indicatorOn = true;
						foreach (ParticleSystem indicator in indicators)
						{
							indicator.gameObject.SetActive(true);
							indicator.Play();
						}
					}

					if (_indicatorOn && ServiceLocator.For(this).Get<PlayerAbilitySystem>().GetActiveAbility != this)
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
			if (_currentState != State.Usable)
				return;

			_currentState = State.Unusable;
			Vector3 mousePos = ServiceLocator.Global.Get<InputUtils.MousePosition>().Invoke();
			Transform holder = ServiceLocator.ForSceneOf(this).Get<ParticleEffectsHolder>().transform;
			GameObject spawn=Instantiate(attackPrefab, mousePos, Quaternion.identity, holder);
			spawn.GetComponentInChildren<ParticleEffectMediator>().SetAbility(this,damagePerHit);
			spawn.GetComponentInChildren<ParticleEffectMediator>().ParticleEffectOn();
			_cooldownTimer.Start();
		}
		public override void CancelExecution()
		{
			//noap
		}
	}
}
