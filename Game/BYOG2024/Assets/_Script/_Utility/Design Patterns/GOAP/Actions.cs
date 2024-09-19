using System.Collections.Generic;
namespace AstekUtility.DesignPattern.GOAP
{
	public class AgentAction
	{
		public string Name { get; }
		public float Cost { get; private set; }

		public HashSet<Belief> Precondition { get; } = new HashSet<Belief>();
		public HashSet<Belief> Effects { get; } = new HashSet<Belief>();

		private Strategies _strategy;
		public bool Complete => _strategy.Complete;

		AgentAction(string name)
		{
			Name = name;
		}

		public void Start() => _strategy.Start();

		public void Update(float deltaTime)
		{
			//Check if action can be performed and update the strategy
			if (_strategy.CanPerform)
			{
				_strategy.Update(deltaTime);
			}

			//Bail out if the strategy is still executing
			if (!_strategy.Complete) return;

			//Apply Effects
			foreach (Belief effect in Effects)
			{
				effect.Evaluate();
			}
		}

		public void Stop() => _strategy.Stop();

		public class Builder
		{
			private readonly AgentAction _action;

			public Builder(string name)
			{
				_action = new AgentAction(name)
				{
					Cost = 1
				};
			}

			public Builder WithCost(float cost)
			{
				_action.Cost = cost;
				return this;
			}

			public Builder WithStrategy(Strategies strategy)
			{
				_action._strategy = strategy;
				return this;
			}

			public Builder AddPreCondition(Belief precondition)
			{
				_action.Precondition.Add(precondition);
				return this;
			}

			public Builder AddEffect(Belief effect)
			{
				_action.Effects.Add(effect);
				return this;
			}

			public AgentAction Build()
			{
				return _action;
			}
		}
	}
}