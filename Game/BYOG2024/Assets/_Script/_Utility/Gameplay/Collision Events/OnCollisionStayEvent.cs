using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	internal class OnCollisionStayEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collision> _onCollisionEnter;

		private void OnCollisionStay(Collision other)
		{
			_onCollisionEnter?.Invoke(other);
		}

		public static OnCollisionStayEvent operator +(OnCollisionStayEvent collision, UnityAction<Collision> action)
		{
			collision._onCollisionEnter.AddListener(action);
			return collision;
		}
		public static OnCollisionStayEvent operator -(OnCollisionStayEvent collision, UnityAction<Collision> action)
		{
			collision._onCollisionEnter.AddListener(action);
			return collision;
		}
	}
}
