using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;

namespace Combat
{
	public class AbilityEvolveSystem : MonoBehaviour
	{
		private Dictionary<EnemyTypes, int> gluttonyCountOfRespectiveTypes = new Dictionary<EnemyTypes, int>();

		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.ForSceneOf(this).Deregister(this);
		}

		public void AbsorbKill(EnemyTypes type)
		{
			if (Enum.IsDefined(typeof(EnemyTypes), type))
			{
				if (!gluttonyCountOfRespectiveTypes.ContainsKey(type))
					gluttonyCountOfRespectiveTypes.Add(type, 0);
				gluttonyCountOfRespectiveTypes[type]+=1;
			}
		}
	}

	public enum EnemyTypes
	{
		NonElemental,
		Fire,
		Decay,
		Lightning
	}
}