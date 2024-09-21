using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Abilities
{
	public abstract class AbilityBase : MonoBehaviour
	{
		[Header("UI Data",order = 100)]
		[field:SerializeField] public Sprite AbilityIcon;
		[field:SerializeField, TextArea] public string AbilityDescription;
		
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