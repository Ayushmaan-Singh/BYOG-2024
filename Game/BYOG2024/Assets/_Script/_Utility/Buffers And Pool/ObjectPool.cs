#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility
{
	public class ObjectPool
	{
		private readonly Type _type;
		private readonly List<object?> _objectQueue;

		private readonly bool _isInfinite = false;
		public int Count => _objectQueue.Count;

		public ObjectPool(uint maxPoolSize, Type type)
		{
			_type = type;
			_objectQueue = new List<object?>((int)maxPoolSize);
			_isInfinite = false;
		}

		public ObjectPool(Type type)
		{
			_type = type;
			_objectQueue = new List<object?>();
			_isInfinite = true;
		}

		public bool Contains(object? obj) => _objectQueue.Contains(obj);

		public bool CanAddItemToPool(object? obj)
		{
			return obj?.GetType() == _type && (_isInfinite || _objectQueue.Count < _objectQueue.Capacity) && !_objectQueue.Contains(obj);
		}

		public void AddItemToPool(object? obj)
		{
			if (CanAddItemToPool(obj))
			{
				_objectQueue.Add(obj);
			}
			else
			{
				if (obj?.GetType() != _type)
					Debug.Log($"ObjectPool: Type {obj?.GetType().Name} does not match collection's type {_type.Name}");

				else
					Debug.Log("ObjectPool : Amount Exceeded Cannot Add More Item");
			}
		}


		public T? GetItemFromPool<T>() where T : class?
		{
			if (typeof(T) != _type && !typeof(T).IsAssignableFrom(_type))
			{
				Debug.Log($"ObjectPool: Type {typeof(T).Name} does not match collection's type {_type.Name}");
				return null;
			}

			if (_objectQueue.Count > 0)
			{
				object? val = _objectQueue[0];
				_objectQueue.RemoveAt(0);
				return val as T;
			}
			return null;
		}

		public object? GetItemFromPool(Type type)
		{
			if (type != _type && !type.IsAssignableFrom(_type))
			{
				Debug.Log($"ObjectPool: Type {type.Name} does not match collection's type {_type.Name}");
				return null;
			}

			if (_objectQueue.Count > 0)
			{
				object? val = _objectQueue[0];
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
				object? value = _objectQueue[0];
				_objectQueue.RemoveAt(0);
				val = value as T;
				return true;
			}
			return false;
		}

		public object? GetSpecificObjectFromPool(object objectToGet)
		{
			if (_objectQueue.Contains(objectToGet))
			{
				object? obj = _objectQueue.Find(x => x == objectToGet);
				_objectQueue.Remove(objectToGet);
				return obj;
			}
			return null;
		}
	}
}