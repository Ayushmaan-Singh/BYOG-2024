using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	public class OnCollisionEnterEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collision> _onCollisionEnter;
			
		private void OnCollisionEnter(Collision other)
		{
			_onCollisionEnter?.Invoke(other);
		}

		public void Register(UnityAction<Collision> action)
		{
			_onCollisionEnter.AddListener(action);
		}
		public void Deregister(UnityAction<Collision> action)
		{
			_onCollisionEnter.AddListener(action);
		}
	}
}
