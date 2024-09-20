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

		public static OnCollisionEnterEvent operator +(OnCollisionEnterEvent collision, UnityAction<Collision> action)
		{
			collision._onCollisionEnter.AddListener(action);
			return collision;
		}
		public static OnCollisionEnterEvent operator -(OnCollisionEnterEvent collision, UnityAction<Collision> action)
		{
			collision._onCollisionEnter.AddListener(action);
			return collision;
		}
	}
}
