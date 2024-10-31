using System;
using AstekUtility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	/// <summary>
	/// Use this class when we need particle to collider in kinematic sense and only detect collision without other physics
	/// It generates collision per spawned particle so can be used for effects that depends on how many collision happens
	/// </summary>
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleCollisionComponent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collider> onCollisionEnter;
		[SerializeField] private UnityEvent<Collider> onCollisionStay;
		[SerializeField] private UnityEvent<Collider> onCollisionExit;

		#if UNITY_EDITOR
		[SerializeField] private bool _showGizmo = false;
		#endif

		private ParticleSystem _ps;
		private readonly Dictionary<uint, ParticleSystem.Particle> _cacheParticle = new Dictionary<uint, ParticleSystem.Particle>();
		private readonly Dictionary<uint, List<Collider>> _particleInCollisionWith = new Dictionary<uint, List<Collider>>();

		private void Awake()
		{
			_ps = GetComponent<ParticleSystem>();
		}

		private void OnDisable()
		{
			if (_cacheParticle.Count <= 0)
				return;

			_cacheParticle.Keys.ForEach(particleID =>
			{
				_particleInCollisionWith[particleID].ForEach(objCollider =>
				{
					onCollisionExit?.Invoke(objCollider);
				});
			});

			_cacheParticle.Clear();
			_particleInCollisionWith.Clear();
		}

		private void OnParticleUpdateJobScheduled()
		{
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_ps.particleCount];
			_ps.GetParticles(particles);

			if (_cacheParticle.Count > 0)
			{
				List<uint> particleIDList = _cacheParticle.Keys.ToList();
				particleIDList.ForEach(particleID =>
				{
					if (_cacheParticle[particleID].remainingLifetime <= 0 || !particles.Any(particle => particle.randomSeed == particleID))
					{
						_cacheParticle.Remove(particleID);
						_particleInCollisionWith[particleID].ForEach(objCollider =>
						{
							onCollisionExit?.Invoke(objCollider);
						});
						_particleInCollisionWith.Remove(particleID);
					}
				});
			}


			particles.ForEach(particle =>
			{
				//If true then means a new particle 
				uint particleID = particle.randomSeed;
				if (_cacheParticle.TryAdd(particleID, particle))
				{
					_particleInCollisionWith.Add(particleID, new List<Collider>());
				}
				else
					_cacheParticle[particleID] = particle;

				Collider[] hitColliders = Physics.OverlapSphere(_ps.transform.TransformPoint(particle.position), particle.GetCurrentSize(_ps) * _ps.collision.radiusScale);

				HandleExitingCollider(particleID, hitColliders);
				HandleEnteringOrStayingColliders(particleID, hitColliders);
			});
		}

		/// <summary>
		/// Perform operation on the ones in collision
		/// </summary>
		/// <param name="hitColliders"></param>
		/// <param name="particleID"></param>
		private void HandleEnteringOrStayingColliders(uint particleID, Collider[] hitColliders)
		{
			hitColliders.ForEach(castCollider =>
			{
				if (!_particleInCollisionWith[particleID].Contains(castCollider))
				{
					_particleInCollisionWith[particleID].Add(castCollider);
					onCollisionEnter?.Invoke(castCollider);
				}
				else
				{
					onCollisionStay?.Invoke(castCollider);
				}
			});
		}

		/// <summary>
		/// Remove the colliders that are no longer in collision
		/// </summary>
		/// <param name="particleID"></param>
		/// <param name="hitColliders"></param>
		private void HandleExitingCollider(uint particleID, Collider[] hitColliders)
		{
			List<Collider> toRemove = _particleInCollisionWith[particleID].Except(hitColliders).ToList();
			toRemove.ForEach(objCollider =>
			{
				_particleInCollisionWith[particleID].Remove(objCollider);
				onCollisionExit?.Invoke(objCollider);
			});
		}

		private void OnDrawGizmos()
		{
			if (!_showGizmo)
				return;

			Gizmos.color = Color.red;
			_cacheParticle.ForEach(particleKeyValue =>
			{
				Gizmos.DrawSphere(_ps.transform.TransformPoint(particleKeyValue.Value.position), particleKeyValue.Value.GetCurrentSize(_ps) * _ps.collision.radiusScale);
			});
		}
	}
}