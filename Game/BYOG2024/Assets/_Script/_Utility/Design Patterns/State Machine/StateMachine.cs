using System;
using System.Collections.Generic;
namespace AstekUtility.DesignPattern.StateMachine
{
	public class StateMachine
	{
		private readonly HashSet<ITransition> _anyTransitions = new HashSet<ITransition>();
		private readonly Dictionary<Type, StateNode> _nodes = new Dictionary<Type, StateNode>();
		public StateNode CurrentState { get; private set; }

		public void FrameUpdate()
		{
			ITransition transition = GetTransition();
			if (transition != null)
				ChangeState(transition.To);

			CurrentState.State?.FrameUpdate();
		}

		public void PhysicsUpdate()
		{
			CurrentState.State?.PhysicsUpdate();
		}

		public void ChangeState(IState state)
		{
			if (state == CurrentState?.State) return;

			IState previousState = CurrentState?.State;
			IState nextState = _nodes[state.GetType()].State;

			previousState?.OnStateExit();
			nextState?.OnStateEnter();
			CurrentState = _nodes[state.GetType()];
		}

		private ITransition GetTransition()
		{
			foreach (ITransition transition in _anyTransitions)
				if (transition.Condition.Evaluate())
					return transition;

			foreach (ITransition transition in CurrentState.Transitions)
				if (transition.Condition.Evaluate())
					return transition;

			return null;
		}

		public void AddTransition(IState from, IState to, IPredicate condition)
		{
			GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
		}

		public void AddAnyTransition(IState to, IPredicate condition)
		{
			_anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
		}

		private StateNode GetOrAddNode(IState state)
		{
			StateNode node = _nodes.GetValueOrDefault(state.GetType());

			if (node == null)
			{
				node = new StateNode(state);
				_nodes.Add(state.GetType(), node);
			}

			return node;
		}

		/// <summary>
		/// Node containing a state and its transitions
		/// </summary>
		public class StateNode
		{
			public StateNode(IState state)
			{
				State = state;
				Transitions = new HashSet<ITransition>();
			}
			public IState State { get; }
			public HashSet<ITransition> Transitions { get; }

			public void AddTransition(IState to, IPredicate condition)
			{
				Transitions.Add(new Transition(to, condition));
			}
		}
	}
}