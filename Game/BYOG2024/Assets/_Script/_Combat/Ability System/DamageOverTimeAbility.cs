using System;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using AstekUtility.Input;
using Entity.Player;
using UnityEngine;

namespace Entity.Abilities
{
	public class DamageOverTimeAbility : AbilityBase
	{
		[Header("Visuals")]
		[SerializeField] private GameObject particleEffect;
		[SerializeField] private ParticleSystem[] indicators;
		[SerializeField] private Transform indicatorHolder;

		[Header("Ability Data")]
		[SerializeField] private float damagePerTick;
		[SerializeField] private int tickPerSec;
		[SerializeField] private float totalDamagePeriod;
		[SerializeField] private float cooldown;

		private List<Collision> inCollision=new List<Collision>();
		private CountdownTimer _cooldown;
		private CountdownTimer _damageTick;
		private CountdownTimer _totalTimePeriod;
		private bool _indicatorOn;

		private void Awake()
		{
			_cooldown = new CountdownTimer(cooldown)
			{
				OnTimerStop = () =>
				{
					CurrentState = State.Usable;
				}
			};
			_damageTick = new CountdownTimer(1 / tickPerSec)
			{
				OnTimerStop = Damage
			};
			_totalTimePeriod = new CountdownTimer(totalDamagePeriod)
			{
				OnTimerStop = _damageTick.Stop
			};
		}

		private void Update()
		{
			switch (CurrentState)
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

					_damageTick.Stop();
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

			CurrentState = State.InProgress;
			Vector3 mousePos = ServiceLocator.Global.Get<InputUtils.MousePosition>().Invoke();
			// Transform holder = ServiceLocator.ForSceneOf(this).Get<ParticleEffectsInGameObjectPool>().transform;
			// GameObject spawn = Instantiate(particleEffect, mousePos, Quaternion.identity, holder);
			// spawn.GetComponentInChildren<OnCollisionStayEvent_Alternate>().Register(ObjectInCollision);
			_damageTick.Start();
		}

		public override void CancelExecution()
		{
			throw new System.NotImplementedException();
		}

		private void ObjectInCollision(List<Collision> collisions)
		{
			inCollision = collisions;
		}

		private void Damage()
		{
			foreach (Collision collision in inCollision)
			{
				collision.collider.GetComponentInParent<EntityHealthManager>().Damage(damagePerTick);
			}
			_damageTick.Reset();
			_damageTick.Start();
		}
	}
}