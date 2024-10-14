using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using AstekUtility.Odin.Utility;
using Entity.Player;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Entity.Abilities
{
	public class Gluttony : AbilityBase
	{
		[SerializeField] private Rigidbody rb;
		[SerializeField] private List<UnityTag> tagThatCanBeAbsorbed;
		
		[Header("Visuals")]
		[SerializeField] private GameObject gluttonyGO;
		[SerializeField] private ParticleSystem[] gluttonyVfx;
		[SerializeField] private Collider gluttonyCollider;

		[Header("Timers")]
		[SerializeField] private float gluttonyMaxRunTime;
		[SerializeField] private float cooldown;
		[SerializeField] private float stopDelay; 

        
		private CountdownTimer _gluttonyRunTimer;
		private CountdownTimer _cooldownTimer;

		private CoroutineTask _wakeupRbTask;

		private void Awake()
		{
			GetComponentInChildren<OnCollisionEnterEvent>().Register(AbsorbDead);
			GetComponentInChildren<OnCollisionStayEvent>().Register(AbsorbDead);

			_wakeupRbTask = new CoroutineTask(WakeupRigidbody(),this,false);
			
			_cooldownTimer = new CountdownTimer(cooldown);
			_gluttonyRunTimer = new CountdownTimer(gluttonyMaxRunTime);

			_cooldownTimer.OnTimerStop += OnCooldownFinished;
			_gluttonyRunTimer.OnTimerStop += StopGluttony;
		}

		private void Update()
		{
			switch (CurrentState)
			{

				case State.Usable:

					_gluttonyRunTimer.Reset();
					_cooldownTimer.Reset();

					break;

				case State.Unusable:

					_cooldownTimer.Tick(Time.deltaTime);

					break;

				case State.InProgress:

					_gluttonyRunTimer.Tick(Time.deltaTime);
					
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void FixedUpdate()
		{
			if (CurrentState == State.InProgress)
			{
				if (rb.IsSleeping() && !_wakeupRbTask.Running)
				{
					_wakeupRbTask.Start();
				}
			}
		}

		public override void Execute()
		{
			if (CurrentState != State.Usable)
				return;
			
			CurrentState = State.InProgress;
			transform.position = ServiceLocator.For(this).Get<PlayerController>().transform.position.With(y:0);
			ServiceLocator.For(this).Get<EntityStatSystem>().ModifyInstanceStatValue(Stats.MovementSpeed, 0, Operation.Equate);
			foreach (ParticleSystem particle in gluttonyVfx)
			{
				particle.Play();
			}
			_gluttonyRunTimer.Start();
		}

		public override void CancelExecution()
		{
			//noap
		}

		private void StopGluttony()
		{
			foreach (ParticleSystem particle in gluttonyVfx)
			{
				particle.Stop();
			}
			
			Invoke(nameof(StopGluttonyDelayed),stopDelay);
		}
		
		private void StopGluttonyDelayed()
		{
			ServiceLocator.For(this).Get<EntityStatSystem>().ModifyInstanceStatValue(Stats.MovementSpeed,
				ServiceLocator.For(this).Get<EntityStatSystem>().GetDefaultStats(Stats.MovementSpeed), Operation.Equate);
			CurrentState = State.Unusable;
			_cooldownTimer.Start();
		}

		private void OnCooldownFinished()
		{
			CurrentState = State.Usable;
		}

		private void AbsorbDead(Collision collision)
		{
			if (CurrentState == State.InProgress && !collision.collider.CompareTag("Enemy"))
				return;

			EntityHealthManager healthManager = collision.collider.GetComponentInChildren<EntityHealthManager>();
			if (healthManager == null)
				healthManager = collision.collider.GetComponentInParent<EntityHealthManager>();

			//healthManager.GettingEatenByGluttony();
		}

		private IEnumerable WakeupRigidbody()
		{
			rb.isKinematic = false;
			rb.WakeUp();
			yield return new WaitForFixedUpdate();
			rb.isKinematic = true;
		}
	}
}