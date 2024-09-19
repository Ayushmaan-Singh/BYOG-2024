using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.BehaviorTree
{
    /// <summary>
    ///     Runs the child node if the provided condition is satisfied else fails
    ///     Critical P1: This class can NEVER have more than 1 CHILD NODE
    ///     Child node can be anything composite, decorator, service or task
    /// </summary>
    public class Conditional : Node
	{
		private readonly Func<bool> _condition;

		public Conditional(string name, Func<bool> condition) : base(name)
		{
			_condition = condition;
			Children = new List<Node>(1);
		}

		public new void AddChild(Node n)
		{
			if (Children.Count == 0)
			{
				base.AddChild(n);
			}
			else
			{
				Debug.LogError($"Conditional Node {Name}: More than one child node");
			}
		}

		public override Status Process()
		{
			if (_condition.Invoke())
			{
				return Children[0].Process();
			}
			return Status.Failure;
		}
	}
}