using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using AstekUtility.Odin.Utility;
using Combat.Enemy;
using Global.Pool;
using Unity.Mathematics;
using UnityEngine;

namespace Entity.Abilities
{
	public class ProjectileAbility : AbilityBase
	{
		[SerializeField] private GameObject projectile;
		[SerializeField] private Transform muzzle;
		[SerializeField] private float cooldownTime;
		[SerializeField] private UnityTag shooterTag;
		[SerializeField] private float damage;

		private CountdownTimer _cooldownTimer;

		private void Awake()
		{
			_cooldownTimer = new CountdownTimer(cooldownTime);
			_cooldownTimer.OnTimerStop += () =>
			{
				CurrentState = State.Usable;
			};
		}

		private void Update()
		{
			switch (CurrentState)
			{
				case State.Usable:
					break;

				case State.Unusable:
					
					_cooldownTimer.Tick(Time.deltaTime);
					break;

				case State.InProgress:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void Execute()
		{
			if (CurrentState != State.Usable)
				return;

			GameObject ammo =
				Instantiate(projectile, muzzle.position, Quaternion.LookRotation(muzzle.forward), ServiceLocator.ForSceneOf(this).Get<ParticleEffectsInGameObjectPool>().transform);
			ammo.GetComponentInChildren<ProjectileAbility>().Execute();
			CurrentState = State.Unusable;
			_cooldownTimer.Start();
		}

		public override void CancelExecution()
		{
			//noap
		}
	}
}