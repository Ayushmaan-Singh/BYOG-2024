using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace AstekUtility
{
	[Serializable]
	public class ObjectPool<T>
	{
		private readonly List<T> _collection = new List<T>();
		[SerializeField, Min(1)] private int maxSize;
		[SerializeField] private bool canExpand = true;

		public bool CanBePooled => _collection.Count < maxSize || canExpand;
		public bool CanRelease => _collection.Count > 0;

		public event Action<T> OnPooled = delegate { };
		public event Action<T> OnRelease = delegate { };
		public event Action<T> OnClear = delegate { };

		public ObjectPool() { }
		public ObjectPool(int maxSize) => this.maxSize = maxSize;
		public ObjectPool(bool canExpand) => this.canExpand = canExpand;

		public void PoolObject(T obj)
		{
			if (!CanBePooled)
				return;

			_collection.Add(obj);
			OnPooled.Invoke(obj);
		}
		public T? ReleaseObject()
		{
			if (!CanRelease)
				return default(T);

			T obj = _collection[0];
			_collection.Remove(obj);
			OnRelease.Invoke(obj);
			return obj;
		}
		public void Clear()
		{
			_collection.ForEach(obj => OnClear.Invoke(obj));
			_collection.Clear();
		}
	}
}