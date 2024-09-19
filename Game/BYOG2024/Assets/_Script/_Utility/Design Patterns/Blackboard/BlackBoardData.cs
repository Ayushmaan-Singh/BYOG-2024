using System;
using System.Collections.Generic;
using UnityEngine;
using AstekUtility.DesignPattern.Blackboard;

namespace AstekUtility
{
	[CreateAssetMenu(menuName = "AstekUtility/Blackboard Data", order = 0)]
	public class BlackBoardData : ScriptableObject
	{
		public List<BlackboardEntryData> Entries = new List<BlackboardEntryData>();

		public void SetValueOnBlackboard(Blackboard blackboard)
		{
			foreach (BlackboardEntryData entry in Entries)
			{
				entry.SetValueOnBlackboard(blackboard);
			}
		}
	}

	[Serializable]
	public class BlackboardEntryData : ISerializationCallbackReceiver
	{
		public string KeyName;
		public AnyValue.ValueType ValueType;
		public AnyValue Value;

		public void SetValueOnBlackboard(Blackboard blackboard)
		{
			BlackboardKey key = blackboard.GetOrRegisterKey(KeyName);
			_setValueDispatchTable[Value.Type](blackboard, key, Value);
		}

		//Dispatch table to set different types of value on the blackboard
		private static Dictionary<AnyValue.ValueType, Action<Blackboard, BlackboardKey, AnyValue>> _setValueDispatchTable 
			= new Dictionary<AnyValue.ValueType, Action<Blackboard, BlackboardKey, AnyValue>>
		{
			{
				AnyValue.ValueType.Int, (blackboard, key, anyValue) => blackboard.SetValue<int>(key, anyValue)
			},
			{
				AnyValue.ValueType.Float, (blackboard, key, anyValue) => blackboard.SetValue<float>(key, anyValue)
			},
			{
				AnyValue.ValueType.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key, anyValue)
			},
			{
				AnyValue.ValueType.String, (blackboard, key, anyValue) => blackboard.SetValue<string>(key, anyValue)
			},
			{
				AnyValue.ValueType.Vector3, (blackboard, key, anyValue) => blackboard.SetValue<Vector3>(key, anyValue)
			},
		};

		public void OnBeforeSerialize() { }
		public void OnAfterDeserialize() => Value.Type = ValueType;
	}

	[Serializable]
	public struct AnyValue
	{
		public enum ValueType { Int, Float, Bool, String, Vector3 }
		public ValueType Type;

		//Storage for different types of values
		public int IntValue;
		public float FloatValue;
		public bool BoolValue;
		public string StringValue;
		public Vector3 Vector3Value;
		// Add more types as needed, but remember to add them to the dispatch table above and the custom Editor

		// Implicit conversion operators to convert AnyValue to different types
		public static implicit operator int(AnyValue value) => value.ConvertValue<int>();
		public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
		public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
		public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
		public static implicit operator Vector3(AnyValue value) => value.ConvertValue<Vector3>();

		private T ConvertValue<T>()
		{
			return Type switch
			{
				ValueType.Int => AsInt<T>(IntValue),
				ValueType.Float => AsFloat<T>(FloatValue),
				ValueType.Bool => AsBool<T>(BoolValue),
				ValueType.String => (T)(object)StringValue,
				ValueType.Vector3 => AsVector3<T>(Vector3Value),
				_ => throw new NotSupportedException($"Not supported value type: {typeof(T)}")
			};
		}

		//Method to convert primitive types to generic types with type safety and without boxing
		public T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default(T);
		public T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default(T);
		public T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default(T);
		public T AsVector3<T>(Vector3 value) => typeof(T) == typeof(Vector3) && value is T correctType ? correctType : default(T);
	}
}