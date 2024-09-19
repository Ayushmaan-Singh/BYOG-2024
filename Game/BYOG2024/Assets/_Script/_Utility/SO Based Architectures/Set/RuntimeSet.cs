using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AstekUtility.RuntimeSetArchitecture
{
	public abstract class RuntimeSet<T> : ScriptableObject
	{
		private Dictionary<string, T> Items = new Dictionary<string, T>();

		public T this[string name]
		{
			get => Items[name];
			set => Items[name] = value;
		}
		public int Count => Items.Count;

		public Dictionary<string, T>.KeyCollection Keys => Items.Keys;
		public Dictionary<string, T>.ValueCollection Values => Items.Values;

		public bool ContainsKey(string key) => Items.ContainsKey(key);
		public bool ContainsValue(T value) => Items.ContainsValue(value);
		public void Add(string itemName, T item) => Items.Add(itemName, item);
		public void Remove(string itemName) => Items.Remove(itemName);
		public void Clear() => Items.Clear();
	}
}