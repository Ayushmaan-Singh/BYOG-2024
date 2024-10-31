using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility.Gameplay.Collision
{
	[RequireComponent(typeof(ParticleSystem))]
	public class OnParticleTriggerEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<ParticleSystem.Particle> onParticleTriggerEnter;
		[SerializeField] private UnityEvent<ParticleSystem.Particle> onParticleTriggerStay;
		[SerializeField] private UnityEvent<ParticleSystem.Particle> onParticleTriggerExit;

		private ParticleSystem ps;

		private void Awake()
		{
			ps = GetComponent<ParticleSystem>();
		}

		public void Register_OnEnter(UnityAction<ParticleSystem.Particle> action) => onParticleTriggerEnter.AddListener(action);
		public void Deregister_OnEnter(UnityAction<ParticleSystem.Particle> action) => onParticleTriggerEnter.RemoveListener(action);

		public void Register_OnExit(UnityAction<ParticleSystem.Particle> action) => onParticleTriggerExit.AddListener(action);
		public void Deregister_OnExit(UnityAction<ParticleSystem.Particle> action) => onParticleTriggerExit.RemoveListener(action);


		private void OnParticleTrigger()
		{
			// Arrays to store particles
			List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
			List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();
			List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

			// Get particles that entered the trigger zone
			int enterCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
			// Get particles that exited the trigger zone
			int exitCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
			// Get particles that are still inside the trigger zone
			int insideCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

			// Update lists and sets
			enter.ForEach(OnParticleEnter);
			inside.ForEach(OnParticleStay);
			exit.ForEach(OnParticleExit);
		}

		private void OnParticleEnter(ParticleSystem.Particle particle) => onParticleTriggerEnter?.Invoke(particle);

		private void OnParticleStay(ParticleSystem.Particle particle) => onParticleTriggerStay?.Invoke(particle);

		private void OnParticleExit(ParticleSystem.Particle particle) => onParticleTriggerExit?.Invoke(particle);
	}
}