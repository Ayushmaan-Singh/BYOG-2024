using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	internal class OnCollisionStayEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collision> onCollisionStay;

		private void OnCollisionStay(Collision other)
		{
			onCollisionStay?.Invoke(other);
		}

		public void Register(UnityAction<Collision> action)
		{
			onCollisionStay.AddListener(action);
		}
		public void Deregister(UnityAction<Collision> action)
		{
			onCollisionStay.AddListener(action);
		}
	}
}
