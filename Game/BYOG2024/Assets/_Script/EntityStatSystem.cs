using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Entity
{
	public class EntityStatSystem : SerializedMonoBehaviour
	{
		[OdinSerialize, InlineProperty] private Dictionary<Stats, float> defaultValue = new Dictionary<Stats, float>();
		private Dictionary<Stats, float> _instanceValue = new Dictionary<Stats, float>();

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
			foreach (Stats stat in defaultValue.Keys)
			{
				_instanceValue.Add(stat, defaultValue[stat]);
			}
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		public float GetInstanceStats(Stats stats) => _instanceValue[stats];
		public float GetDefaultStats(Stats stats) => defaultValue[stats];

		public EntityStatSystem ModifyBaseStatValue(Stats stat, float amount, Operation operation)
		{
			switch (operation)
			{

				case Operation.Increment:

					defaultValue[stat] += amount;
					break;

				case Operation.Decrement:

					defaultValue[stat] -= amount;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
			}
			return this;
		}
		public EntityStatSystem ModifyInstanceStatValue(Stats stat, float amount, Operation operation)
		{
			switch (operation)
			{

				case Operation.Increment:

					_instanceValue[stat] += amount;
					break;

				case Operation.Decrement:

					_instanceValue[stat] -= amount;
					break;
				
				case Operation.Equate:

					_instanceValue[stat] = amount;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
			}
			return this;
		}
	}

	public enum Stats
	{
		MovementSpeed,
		DamageScale,
		Hp
	}

	public enum Operation
	{
		Increment,
		Decrement,
		Equate
	}
}