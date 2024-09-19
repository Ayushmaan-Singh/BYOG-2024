using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.DesignPattern.GOAP
{
	public class BeliefFactory
	{
		private readonly GoapAgent _agent;
		private readonly Dictionary<string, Belief> _beliefs;

		public BeliefFactory(GoapAgent agent, Dictionary<string, Belief> beliefs)
		{
			_agent = agent;
			_beliefs = beliefs;
		}

		public void AddBelief(string key, Func<bool> condition)
		{
			_beliefs.Add(key, new Belief.Builder(key)
				.WithCondition(condition)
				.Build());
		}

		public void AddSensorBelief(string key, ISensor sensor)
		{
			_beliefs.Add(key, new Belief.Builder(key)
				.WithCondition(() => sensor.IsTargetInRange)
				.WithLocation(() => sensor.TargetPosition)
				.Build());
		}

		public void AddLocationBelief(string key, float distance, Transform locationCondition)
		{
			AddLocationBelief(key, distance, locationCondition.position);
		}

		public void AddLocationBelief(string key, float distance, Vector3 locationCondition)
		{
			_beliefs.Add(key, new Belief.Builder(key)
				.WithLocation(() => locationCondition)
				.Build());
		}

		public bool InRangeOf(Vector3 pos, float range)
		{
			return Vector3.Distance(_agent.transform.position, pos) < range;
		}
	}

	public class Belief
	{
		public string Name { get; }

		private Func<bool> _condition = () => false;
		private Func<Vector3> _observedLocation = () => Vector3.zero;

		public Vector3 Location => _observedLocation();

		private Belief(string name)
		{
			Name = name;
		}

		public bool Evaluate()
		{
			return _condition();
		}

		public class Builder
		{
			private readonly Belief belief;

			public Builder(string name)
			{
				belief = new Belief(name);
			}

			public Builder WithCondition(Func<bool> condition)
			{
				belief._condition = condition;
				return this;
			}

			public Builder WithLocation(Func<Vector3> observedLocation)
			{
				belief._observedLocation = observedLocation;
				return this;
			}

			public Belief Build()
			{
				return belief;
			}
		}
	}
}