using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay;
using Global;
using UnityEngine;
using UnityEngine.Events;

namespace Entity.Abilities
{
	[RequireComponent(typeof(ParticleSystem))]
	public class MineExplosion : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collider, Explosion> onCollisionEnter;
		[SerializeField] private ParticleSystem explosion;

		#if UNITY_EDITOR
		[SerializeField] private bool _showGizmo = false;
		#endif

		private ParticleSystem _ps;
		private LayerMask _includeLayers;
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

			_cacheParticle.Clear();
			_particleInCollisionWith.Clear();
			_includeLayers = 0;
		}

		private void OnParticleUpdateJobScheduled()
		{
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_ps.particleCount];
			_ps.GetParticles(particles);

			if (_cacheParticle.Count > 0)
			{
				List<uint> particleIDList = EnumerableExtensions.ToList(_cacheParticle.Keys);
				particleIDList.ForEach(particleID =>
				{
					if (_cacheParticle[particleID].remainingLifetime <= 0 || !EnumerableExtensions.Any(particles, particle => particle.randomSeed == particleID))
					{
						_cacheParticle.Remove(particleID);
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
				
				_ps.SetParticles(_cacheParticle.Values.ToArray());
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
				if (!_particleInCollisionWith[particleID].Contains(castCollider) && castCollider.gameObject.IsInLayer(_includeLayers))
				{
					_particleInCollisionWith[particleID].Add(castCollider);
					Explosion explosionFX = Instantiate(explosion
						, _ps.transform.TransformPoint(_cacheParticle[particleID].position)
						, Quaternion.identity
						, ServiceLocator.ForSceneOf(this).Get<VFXHolder>().transform).GetComponent<Explosion>();

					_cacheParticle.Remove(particleID);
					onCollisionEnter?.Invoke(castCollider, explosionFX);
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
			List<Collider> toRemove = EnumerableExtensions.ToList(_particleInCollisionWith[particleID].Except(hitColliders));
			toRemove.ForEach(objCollider =>
			{
				_particleInCollisionWith[particleID].Remove(objCollider);
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

		public class Builder
		{
			private LayerMask _includeLayers;

			public Builder InitCollisionLayers(LayerMask layers)
			{
				_includeLayers = layers;
				return this;
			}

			public MineExplosion Build(MineExplosion instance)
			{
				instance._includeLayers = _includeLayers;
				return instance;
			}
		}
	}
}