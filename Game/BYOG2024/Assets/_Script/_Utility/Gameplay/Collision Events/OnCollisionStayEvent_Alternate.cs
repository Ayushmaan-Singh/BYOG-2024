using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	/// <summary>
	/// This is the alternate of OnCollisionStayEvent. It uses a combination of OnCollisionEnter and OnCollisionExit instead of OnCollisionStay.
	/// Recommended especially for kinematic and static(not in motion) body
	/// </summary>
	public class OnCollisionStayEvent_Alternate : MonoBehaviour
	{
		[SerializeField] private UnityEvent<List<Collision>> collisionUpdate;
		[SerializeField] private UnityEvent<List<Collision>> collisionFixedUpdate;

		private List<Collision> collisionCollection = new List<Collision>();

		private void Update()
		{
			collisionUpdate?.Invoke(collisionCollection);
		}

		private void FixedUpdate()
		{
			collisionCollection = collisionCollection.Where(x => x != null).ToList();
			collisionFixedUpdate?.Invoke(collisionCollection);
		}

		private void OnCollisionEnter(Collision colliding)
		{
			collisionCollection.Add(colliding);
		}
		private void OnCollisionExit(Collision colliding)
		{
			collisionCollection.Remove(colliding);
		}

		public void Register(UnityAction<List<Collision>> collisionAction, bool runInFixedUpdate = false)
		{
			if (runInFixedUpdate)
				collisionFixedUpdate.AddListener(collisionAction);
			else
				collisionUpdate.AddListener(collisionAction);
		}

		public void Deregister(UnityAction<List<Collision>> collisionAction)
		{
			collisionUpdate.RemoveListener(collisionAction);
			collisionFixedUpdate.RemoveListener(collisionAction);
		}
	}
}