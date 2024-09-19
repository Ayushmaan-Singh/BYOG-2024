#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility
{
	public interface IObjectPool { }

	public class ObjectPool
	{
		private readonly Type _type;
		private readonly List<object> _objectQueue;

		private bool _isInfinite = false;

		public ObjectPool(uint maxPoolSize, Type type)
		{
			_type = type;
			_objectQueue = new List<object>((int)maxPoolSize);
			_isInfinite = false;
		}

		public ObjectPool(Type type)
		{
			_type = type;
			_objectQueue = new List<object>();
			_isInfinite = true;
		}

		public bool CanAddItemToPool(object obj)
		{
			return obj.GetType().Equals(_type) && (_isInfinite || _objectQueue.Count < _objectQueue.Capacity) && !_objectQueue.Contains(obj);
		}

		public void AddItemToPool(object obj)
		{
			if (CanAddItemToPool(obj))
			{
				_objectQueue.Add(obj);
			}
			else
			{
				if (!obj.GetType().Equals(_type))
					Debug.Log($"ObjectPool: Type {obj.GetType().Name} does not match collection's type {_type.Name}");

				else
					Debug.Log("ObjectPool : Amount Exceeded Cannot Add More Item");
			}
		}

		public int GetPooledItemCount()
		{
			return _objectQueue.Count;
		}

		public T? GetItemFromPool<T>() where T : class?
		{
			if (!typeof(T).Equals(_type) && !typeof(T).IsAssignableFrom(_type))
			{
				Debug.Log($"ObjectPool: Type {typeof(T).Name} does not match collection's type {_type.Name}");
				return null;
			}

			if (_objectQueue.Count > 0)
			{
				object val = _objectQueue[0];
				_objectQueue.RemoveAt(0);
				return val as T;
			}
			return null;
		}
		
		public object GetItemFromPool(Type type)
		{
			if (!type.Equals(_type) && !type.IsAssignableFrom(_type))
			{
				Debug.Log($"ObjectPool: Type {type.Name} does not match collection's type {_type.Name}");
				return null;
			}

			if (_objectQueue.Count > 0)
			{
				object val = _objectQueue[0];
				_objectQueue.RemoveAt(0);
				return val;
			}
			return null;
		}

		public bool TryGetItemFromPool<T>(out T? val) where T : class?
		{
			val = null;
			if (!typeof(T).Equals(_type))
			{
				Debug.Log($"ObjectPool: Type {typeof(T).Name} does not match collection's type {_type.Name}");
				return false;
			}

			if (_objectQueue.Count > 0)
			{
				object value = _objectQueue[0];
				_objectQueue.RemoveAt(0);
				val = value as T;
				return true;
			}
			return false;
		}
	}
}