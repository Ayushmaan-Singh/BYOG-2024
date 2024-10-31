using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using AstekUtility.Gameplay;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility.Gameplay.Collision
{
	/// <summary>
	/// Use this class when the particle isn't affected by physics in particle collision or you only need 1 collision per particle system
	/// It required the enabling and setting of particle system's collision module
	/// </summary>
	[RequireComponent(typeof(ParticleSystem))]
	public class OnParticleCollisionEvent : MonoBehaviour
	{
		[Title("Get Collision GameObject")]
		[SerializeField] private UnityEvent<ParticleCollisionEvent> onParticleCollisionEnter;
		[SerializeField] private UnityEvent<GameObject> onParticleCollisionExit;

		private ParticleSystem _ps;
		private readonly HashSet<GameObject> _objectInCollisionWithThisPS = new HashSet<GameObject>();

		private void Awake()
		{
			_ps = GetComponent<ParticleSystem>();
		}

		public void Register_OnEnter(UnityAction<ParticleCollisionEvent> action) => onParticleCollisionEnter.AddListener(action);
		public void Deregister_OnEnter(UnityAction<ParticleCollisionEvent> action) => onParticleCollisionEnter.RemoveListener(action);

		public void Register_OnExit(UnityAction<GameObject> action) => onParticleCollisionExit.AddListener(action);
		public void Deregister_OnExit(UnityAction<GameObject> action) => onParticleCollisionExit.RemoveListener(action);

		private void OnParticleCollision(GameObject other)
		{
			List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
			int numCollisionEvents = _ps.GetCollisionEvents(other, collisionEvents);
			
			for (int i = 0; i < numCollisionEvents; i++)
			{
				GameObject collisionObj = collisionEvents[i].colliderComponent.gameObject;

				if (_objectInCollisionWithThisPS.Contains(collisionObj))
					continue;
				
				_objectInCollisionWithThisPS.Add(collisionObj);
				OnParticleEnter(collisionEvents[i]);
			}

			_objectInCollisionWithThisPS.RemoveWhere(collisionObj =>
			{
				bool stillInCollision = false;
				for (int i = 0; i < numCollisionEvents; i++)
				{
					if (collisionEvents[i].colliderComponent.gameObject != collisionObj)
						continue;

					stillInCollision = true;
					break;
				}
				if (!stillInCollision)
				{
					OnParticleExit(collisionObj);
				}
				return !stillInCollision;
			});
		}

		private void OnParticleEnter(ParticleCollisionEvent collision) => onParticleCollisionEnter?.Invoke(collision);
		private void OnParticleExit(GameObject other) => onParticleCollisionExit?.Invoke(other);

	}
}