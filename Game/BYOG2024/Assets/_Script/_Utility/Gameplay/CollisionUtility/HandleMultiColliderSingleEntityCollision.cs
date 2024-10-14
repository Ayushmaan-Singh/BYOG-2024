using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.Gameplay.Collision
{
	/// <summary>
	/// This is script is used in the case where a single rigidbody gets collision event from multiple colliders, but object that is in collision with any
	/// one of the collider is considered to be in collision with the whole body i.e multiple colliders combine to form a single
	/// big collider
	/// </summary>
	public class HandleMultiColliderSingleEntityCollision
	{
		private readonly Dictionary<GameObject, List<Collider>> _gameObjectInCollision = new Dictionary<GameObject, List<Collider>>();

		/// <summary>
		/// if collision passed requires the registration of key then its a new collision else an old collision
		/// </summary>
		/// <param name="collision"></param>
		/// <param name="isObjectInCollision"></param>
		/// <returns></returns>
		public HandleMultiColliderSingleEntityCollision ProcessCollisionEnter(UnityEngine.Collision collision ,out bool isObjectInCollision)
		{
			isObjectInCollision = true;
			foreach (ContactPoint contact in collision.contacts)
			{
				if (!_gameObjectInCollision.ContainsKey(collision.gameObject))
				{
					_gameObjectInCollision.Add(collision.gameObject, new List<Collider>(){contact.thisCollider});
					isObjectInCollision = false;
				}
				else
				{
					_gameObjectInCollision[collision.gameObject].Add(contact.thisCollider);
				}
			}
			return this;
		}
		
		/// <summary>
		/// if said gameobject has no collider with which it is colliding then that means this object is no longer in collision
		/// </summary>
		/// <param name="collision"></param>
		/// <returns></returns>
		public HandleMultiColliderSingleEntityCollision ProcessCollisionExit(UnityEngine.Collision collision)
		{
			foreach (ContactPoint contact in collision.contacts)
			{
				_gameObjectInCollision[collision.gameObject].Remove(contact.thisCollider);
			}
			if (_gameObjectInCollision[collision.gameObject].Count <= 0)
				_gameObjectInCollision.Remove(collision.gameObject);
			return this;
		}

		public void Reset() => _gameObjectInCollision.Clear();
	}
}