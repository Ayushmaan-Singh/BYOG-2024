using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	public class OnParticleCollisionEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<GameObject> onParticleCollisionEvent;

		private void OnParticleCollision(GameObject other)
		{
			onParticleCollisionEvent?.Invoke(other);
		}

		public void Register(UnityAction<GameObject> action)
		{
			onParticleCollisionEvent.AddListener(action);
		}

		public void Deregister(UnityAction<GameObject> action)
		{
			onParticleCollisionEvent.RemoveListener(action);
		}
	}
}
