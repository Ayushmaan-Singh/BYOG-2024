using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Abilities
{
	public abstract class AbilityBase : MonoBehaviour
	{
		public enum State
		{
			Usable,
			Unusable,
			InProgress
		}

		public abstract void Execute();
		public abstract void CancelExecution();
	}
}