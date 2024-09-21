using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	public class OnParticleSystemStoppedEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<ParticleSystem> _onParticleSystemStopped;

		private void OnParticleSystemStopped()
		{
			_onParticleSystemStopped?.Invoke(GetComponent<ParticleSystem>());
		}

		public void Register(UnityAction<ParticleSystem> action)
		{
			_onParticleSystemStopped.AddListener(action);
		}
		public void Deregister(UnityAction<ParticleSystem> action)
		{
			_onParticleSystemStopped.AddListener(action);
		}
	}
}
