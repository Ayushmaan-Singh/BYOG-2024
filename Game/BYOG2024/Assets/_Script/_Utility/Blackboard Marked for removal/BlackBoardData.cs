using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace _Scripts._Utility.Blackboard
{
	public enum DataType { Int, Float, Bool }

	[Serializable]
	public class SerializedDict<T>:ISerializationCallbackReceiver
	{
		public List<DictElement> Collection;
		private Dictionary<T, DataValue> dict;

		public void OnBeforeSerialize()
		{
			throw new NotImplementedException();
		}
		public void OnAfterDeserialize()
		{
			throw new NotImplementedException();
		}
	}
	
	[Serializable]
	public class DictElement
	{
		public string Key;
		public DataType ValueType = DataType.Int;
		public DataValue Value;
	}

	public class DataValue
	{
		public int IntField = 0;
		public float FloatField = 0;
		public bool BoolField = false;
	}
}