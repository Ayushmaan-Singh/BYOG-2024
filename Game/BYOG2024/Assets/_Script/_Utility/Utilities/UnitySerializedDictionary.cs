using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility
{
	// public abstract class UnitySerializedDictionary<TKey, TValue> : ISerializationCallbackReceiver
	// {
	// 	[SerializeField] private List<DictData> dictData = new List<DictData>();
	// 	private Dictionary<TKey, TValue> _dictionary;
	//
	// 	public TValue this[TKey key] { get { return _dictionary[key]; } set { _dictionary[key] = value; } }
	//
	// 	[Serializable]
	// 	private class DictData
	// 	{
	// 		public TKey Key;
	// 		public TValue Value;
	// 	}
	//
	// 	void ISerializationCallbackReceiver.OnAfterDeserialize()
	// 	{
	// 		_dictionary.Clear();
	// 		foreach (DictData t in dictData)
	// 		{
	// 			this[t.Key] = t.Value;
	// 		}
	// 	}
	//
	// 	void ISerializationCallbackReceiver.OnBeforeSerialize()
	// 	{
	// 		dictData.Clear();
	// 		foreach (KeyValuePair<TKey, TValue> item in _dictionary)
	// 		{
	// 			dictData.Add(new DictData
	// 			{
	// 				Key = item.Key,
	// 				Value = item.Value
	// 			});
	// 		}
	// 	}
	//}
}