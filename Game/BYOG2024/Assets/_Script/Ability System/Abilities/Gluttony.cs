using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Entity.Player;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Entity.Abilities
{
	public class Gluttony : AbilityBase
	{
		[SerializeField] private GameObject gluttonyGO;
		[SerializeField] private ParticleSystem[] gluttonyVfx;
		[SerializeField] private Collider gluttonyCollider;
		[SerializeField] private float disableColliderAfter;
		[SerializeField] private float enableColliderAfter;

		private void Awake()
		{
			OnCollisionEnterEvent collisionEvent = GetComponentInChildren<OnCollisionEnterEvent>() + AbsorbDead;
			OnCollisionStayEvent collisionStayEvent = GetComponentInChildren<OnCollisionStayEvent>() + AbsorbDead;
			gluttonyCollider.enabled = false;
		}

		public override void Execute()
		{
			gluttonyGO.SetActive(true);
			Invoke(nameof(TriggerCollider),enableColliderAfter);
			transform.position = ServiceLocator.For(this).Get<PlayerController>().transform.position.With(y:gluttonyGO.transform.position.y);
			ServiceLocator.For(this).Get<EntityStatSystem>().ModifyInstanceStatValue(Stats.MovementSpeed, 0, Operation.Equate);
			foreach (ParticleSystem particle in gluttonyVfx)
			{
				particle.Play();
			}
		}
		public override void CancelExecution()
		{
			foreach (ParticleSystem particle in gluttonyVfx)
			{
				particle.Stop();
			}
			Invoke(nameof(TriggerCollider),disableColliderAfter);
			ServiceLocator.For(this).Get<EntityStatSystem>().ModifyInstanceStatValue(Stats.MovementSpeed,
				ServiceLocator.For(this).Get<EntityStatSystem>().GetDefaultStats(Stats.MovementSpeed), Operation.Equate);
		}

		public void AbsorbDead(Collision collision)
		{
			if (!collision.collider.CompareTag("Enemy"))
				return;

			EntityHealthManager healthManager = collision.collider.GetComponentInChildren<EntityHealthManager>();
			if (healthManager == null)
				healthManager = collision.collider.GetComponentInParent<EntityHealthManager>();

			healthManager.GettingEatenByGluttony();
		}

		public void TriggerCollider()
		{
			gluttonyCollider.enabled = !gluttonyCollider.enabled;
		}
	}
}