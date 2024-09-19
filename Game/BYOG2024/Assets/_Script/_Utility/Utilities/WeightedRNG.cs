using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

namespace AstekUtility
{
	/// <summary>
	///     Create a object that can be randomly selected from a set of other with a set chance of getting selected
	/// </summary>
	[Serializable]
	public class WeightedRNG<T>
	{
		[field:SerializeField] public T Value { get; private set; }
		[field:SerializeField] public int Probability { get; private set; }

		public WeightedRNG()
		{
			
		}
		
		public WeightedRNG(T value, int probability)
		{
			Value = value;
			Probability = probability;
		}
	}

	/// <summary>
	///     Get a Random object from a list depending on the probability of selection
	/// </summary>
	public static class CalcWeightedRNG
	{
		public static T GetRandomValue<T>(List<WeightedRNG<T>> collection)
		{
			int totalProbability = 0;
			foreach (WeightedRNG<T> item in collection)
			{
				totalProbability += item.Probability;
			}

			int rand = Random.Range(0, totalProbability);
			int currentProb = 0;

			foreach (WeightedRNG<T> selection in collection)
			{
				currentProb += selection.Probability;
				if (rand <= currentProb)
					return selection.Value;
			}
			return default(T);
		}
	}
}