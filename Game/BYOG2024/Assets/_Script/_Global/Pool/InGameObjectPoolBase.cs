using System;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;

namespace Global.Pool
{
	public class InGameObjectPoolBase : MonoBehaviour
	{
		protected Dictionary<Type, ObjectPool> _pool = new Dictionary<Type, ObjectPool>();

		public ObjectPool this[Type type] => _pool.GetValueOrDefault(type);

		public Dictionary<Type, ObjectPool>.KeyCollection Keys => _pool.Keys;
		public Dictionary<Type, ObjectPool>.ValueCollection Values => _pool.Values;
		public int Count => _pool.Count;


		public virtual bool TryGetPooledObject<T>(out T pooledObject) where T : class
		{
			if (_pool.TryGetValue(typeof(T), out ObjectPool pool))
			{
				pooledObject = pool.GetItemFromPool<T>();
				return pooledObject != null;
			}
			pooledObject = null;
			return false;
		}
		public virtual void PoolThisObject<T>(T objectToPool) where T : class
		{
			Type type = objectToPool.GetType();
			ObjectPool pool;
			if (_pool.TryGetValue(type, out ObjectPool value))
				pool = value;
			else
			{
				pool = new ObjectPool(type);
				_pool.Add(type, pool);
			}

			if (pool.CanAddItemToPool(objectToPool))
				pool.AddItemToPool(objectToPool);
		}

		public void DestroyAllObjectOfType<T>() where T : MonoBehaviour
		{
			if (_pool.TryGetValue(typeof(T), out ObjectPool pool))
			{
				int count = pool.Count;
				for (int i = 0; i < count; i++)
				{
					Destroy(ServiceLocator.For(pool.GetItemFromPool<T>()).gameObject);
				}
			}
			_pool.Remove(typeof(T));
		}
		public void DestroyObject<T>(T objectToGet) where T : MonoBehaviour
		{
			if (!_pool.TryGetValue(typeof(T), out ObjectPool pool))
				return;

			T objectPooled = pool.GetSpecificObjectFromPool(objectToGet) as T;
			Destroy(ServiceLocator.For(objectPooled).gameObject);
		}
	}
}