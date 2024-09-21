using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.VisualFeedback;
using Unity.VisualScripting;
using UnityEngine;

namespace Entity.Abilities
{
	public class ParticleEffectMediator : MonoBehaviour
	{
		[SerializeField] private ParticleSystem[] particleSystems;
		public AbilityBase Ability { get; private set; }
		public float Damage { get; private set; }

		private bool _isRunning = false;
		private int finishedRegisteredBy = 0;

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		public ParticleEffectMediator SetAbility(AbilityBase ability,float damage)
		{
			Ability = ability;
			Damage = damage;
			return this;
		}

		private void Update()
		{
			if (finishedRegisteredBy >= particleSystems.Length)
				Destroy(gameObject);
			
			if (!Ability && _isRunning)
				ParticleEffectOff();
		}

		public void ParticleEffectOn()
		{
			if (Ability == null || _isRunning)
				return;

			_isRunning = true;
			foreach (ParticleSystem particle in particleSystems)
			{
				particle.Play();
			}
		}

		public void ParticleEffectOff()
		{
			_isRunning = false;
			foreach (ParticleSystem particle in particleSystems)
			{
				particle.Stop();
			}
		}
		public void ParticleFinished(ParticleSystem particleSystem)
		{
			finishedRegisteredBy++;
			Debug.Log(finishedRegisteredBy);
		}
	}
}