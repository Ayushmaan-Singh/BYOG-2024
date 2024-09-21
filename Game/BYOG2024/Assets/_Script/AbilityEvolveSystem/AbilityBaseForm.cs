using System;
using System.Collections.Generic;
using AstekUtility;
using Combat;
using Entity.Abilities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
//using Unity.VisualScripting;
using UnityEngine;
namespace _Script.AbilityEvolveSystem
{
	public class AbilityBaseForm : SerializedScriptableObject
	{
		[OdinSerialize] private Dictionary<AbilityBase, List<EnemyConsumptionCount>> AbilityGainRequirement;

		public AbilityBase GainableAbility()
		{
			return null;
		}
	}

	[Serializable]
	public struct EnemyConsumptionCount
	{
		public EnemyTypes Type;
		public int Count;

		public override bool Equals(object obj)
		{
			return obj is EnemyConsumptionCount enemyCount && this == enemyCount;
		}

		public override int GetHashCode()
		{
			return Type.ToString().ComputeFNV1aHash();
		}

		public static bool operator ==(EnemyConsumptionCount e1, EnemyConsumptionCount e2) => e1.Type == e2.Type && e1.Count == e2.Count;
		public static bool operator !=(EnemyConsumptionCount e1, EnemyConsumptionCount e2) => !(e1 == e2);
	}
}