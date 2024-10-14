using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Entity.Abilities
{
	public abstract class AbilityBase : SerializedMonoBehaviour
	{
		[Title("UI Data")]
		[field:SerializeField] public Sprite AbilityIcon { get; private set; }
		[field:SerializeField] public Sprite AbilityTileImage { get; private set; }
		[field:SerializeField, TextArea] public string AbilityDescription { get; private set; }

		[Title("Spawn Data")]
		[OdinSerialize] protected Dictionary<Type, TransformValues> localOffsetFromModel;

		public enum State
		{
			Usable,
			Unusable,
			InProgress
		}

		public State CurrentState { get; protected set; }
		public float Progress { get; protected set; }

		public abstract void Execute();
		public abstract void CancelExecution();

		[Serializable]
		public class TransformValues
		{
			public Vector3 Position;
			public Quaternion Rotation;
			public Vector3 Scale;
		}
	}
}