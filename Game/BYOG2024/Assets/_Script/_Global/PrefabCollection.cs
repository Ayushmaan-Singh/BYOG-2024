using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Global
{
	public class PrefabCollection : SerializedScriptableObject
	{
		[OdinSerialize, InlineProperty] private Dictionary<Type, GameObject> _prefabs = new Dictionary<Type, GameObject>();

		public GameObject this[Type type] => _prefabs.GetValueOrDefault(type);

		public Dictionary<Type, GameObject>.KeyCollection Keys => _prefabs.Keys;
		public Dictionary<Type, GameObject>.ValueCollection Values => _prefabs.Values;
		public int Count => _prefabs.Count;

		public bool TryGet(Type type, out GameObject prefab) => _prefabs.TryGetValue(type, out prefab);
	}
}