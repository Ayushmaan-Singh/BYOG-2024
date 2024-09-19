using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;
namespace AstekUtility.DesignPattern.Blackboard
{
	[Serializable]
	public readonly struct BlackboardKey : IEquatable<BlackboardKey>
	{
		private readonly string _name;
		private readonly int _hashKey;

		public BlackboardKey(string name)
		{
			_name = name;
			_hashKey = name.ComputeFNV1aHash();
		}

		public bool Equals(BlackboardKey other) => _hashKey == other._hashKey;
		public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
		public override int GetHashCode() => _hashKey;
		public override string ToString() => _name;

		public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs._hashKey == rhs._hashKey;
		public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => lhs._hashKey != rhs._hashKey;

	}

	[Serializable]
	public class BlackboardEntry<T>
	{
		public BlackboardKey Key { get; }
		public T Value { get; }
		public Type ValueType { get; }

		public BlackboardEntry(BlackboardKey key, T value)
		{
			Key = key;
			Value = value;
			ValueType = typeof(T);
		}

		public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
		public override int GetHashCode() => Key.GetHashCode();
	}

	[Serializable]
	public class Blackboard
	{
		private Dictionary<string, BlackboardKey> _keyRegistry = new Dictionary<string, BlackboardKey>();
		private Dictionary<BlackboardKey, object> _entries = new Dictionary<BlackboardKey, object>();

		public List<Action> PassedActions { get; } = new List<Action>();

		#if UNITY_EDITOR

		public void Debug()
		{
			foreach (KeyValuePair<BlackboardKey, object> entry in _entries)
			{
				Type entryType = entry.Value.GetType();

				if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
				{
					PropertyInfo valueProperty = entryType.GetProperty("Value");
					if (valueProperty == null) continue;
					object value = valueProperty.GetValue(entry.Value);
					UnityEngine.Debug.Log($"Key:{entry.Key}, Value:{entry.Value}");
				}
			}
		}

		#endif

		public void AddAction(Action action)
		{
			Preconditions.CheckNotNull(action);
			PassedActions.Add(action);
		}

		public void ClearActions() => PassedActions.Clear();

		public bool TryGetValue<T>(BlackboardKey key, out T value)
		{
			if (_entries.TryGetValue(key, out object entry) && entry is BlackboardEntry<T> castedEntry)
			{
				value = castedEntry.Value;
				return true;
			}

			value = default;
			return false;
		}

		public void SetValue<T>(BlackboardKey key, T value)
		{
			_entries[key] = new BlackboardEntry<T>(key, value);
		}

		public BlackboardKey GetOrRegisterKey(string keyName)
		{
			Preconditions.CheckNotNull(keyName);
			if (!_keyRegistry.TryGetValue(keyName, out BlackboardKey key))
			{
				key = new BlackboardKey(keyName);
				_keyRegistry[keyName] = key;
			}

			return key;
		}

		public bool ContainsKey(BlackboardKey key) => _entries.ContainsKey(key);
		public void Remove(BlackboardKey key) => _entries.Remove(key);
	}
}