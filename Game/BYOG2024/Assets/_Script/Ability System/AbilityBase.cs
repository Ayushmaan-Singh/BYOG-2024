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
		[field:SerializeField, TextArea] public string AbilityDescription { get; private set; }
		[field:OdinSerialize, InlineProperty] public Dictionary<EnemyTypes, int> enemyTypeForEvolution { get; protected set; } = new Dictionary<EnemyTypes, int>();

		public enum State
		{
			Usable,
			Unusable,
			InProgress
		}

		protected State _currentState = State.Usable;

		public abstract void Execute();
		public abstract void CancelExecution();
	}
}