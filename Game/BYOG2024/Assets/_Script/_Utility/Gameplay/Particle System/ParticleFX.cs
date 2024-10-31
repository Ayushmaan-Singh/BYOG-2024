using System;
using System.Collections.Generic;
using System.Linq;
using AstekUtility.Gameplay;
using UnityEngine;

namespace AstekUtility
{
	[RequireComponent(typeof(OwnerOfThisObject))]
	public class ParticleFX : MonoBehaviour
	{
		[SerializeField] private ParticleSystem[] _particleSystems;
		[SerializeField] private bool _destroyOnFinish = false;

		private readonly List<ParticleSystem> _mainPS = new List<ParticleSystem>();
		private readonly List<ParticleSystem> _subEmitters = new List<ParticleSystem>();

		public ParticleSystem[] ParticleSystems => _particleSystems;
		public ParticleSystem[] MainParticleSystem => _mainPS.ToArray();
		public ParticleSystem[] SubEmitters => _subEmitters.ToArray();

		public event Action<ParticleFX> OnParticleEffectStopped = delegate { };
		public event Action<ParticleFX> OnParticleEffectFinished = delegate { };

		private bool isRunning = false;

		private void Awake()
		{
			//First Store SubEmitters
			_particleSystems.ForEach(ps =>
			{
				int subEmitterCount = ps.GetComponent<ParticleSystem>()?.subEmitters.subEmittersCount ?? 0;
				if (!ps.subEmitters.enabled || subEmitterCount <= 0)
					return;

				for (int i = 0; i < subEmitterCount; i++)
					_subEmitters.Add(ps.subEmitters.GetSubEmitterSystem(i));
			});

			//Store all that are not in _subEmitters
			_particleSystems.ForEach(ps =>
			{
				if (!_subEmitters.Contains(ps))
					_mainPS.Add(ps);
			});
		}

		private void Update()
		{
			if (!isRunning || _particleSystems.Any(ps => ps.IsAlive()))
				return;

			isRunning = false;
			OnParticleEffectFinished.Invoke(this);
			if (_destroyOnFinish)
				Destroy(gameObject);
		}

		public void Play()
		{
			isRunning = true;
			_particleSystems.ForEach(ps =>
			{
				if (!ps.gameObject.activeInHierarchy)
					ps.gameObject.SetActive(true);

				if (!ps.main.playOnAwake)
					ps.Play();
			});
		}
		public void Stop()
		{
			isRunning = false;
			OnParticleEffectStopped.Invoke(this);
			_mainPS.ForEach(ps =>
			{
				if (ps.main.playOnAwake)
					ps.gameObject.SetActive(false);
				else
					ps.Stop();
			});
			_subEmitters.ForEach(ps => ps.Stop());
		}
		public void Restart()
		{
			Stop();
			Play();
		}
	}
}