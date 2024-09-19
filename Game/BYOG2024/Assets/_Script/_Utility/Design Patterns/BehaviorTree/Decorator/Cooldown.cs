using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.BehaviorTree
{
	public class Cooldown : Node
	{
		private readonly float _cooldown;
		private double _timeCounter;

		public Cooldown(string name, float cooldown) : base(name)
		{
			_cooldown = cooldown;
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
				Debug.LogError($"Cooldown Node {Name}: More than one child node");
			}
		}

		public override Status Process()
		{
			if (Time.time >= _timeCounter)
			{
				_timeCounter = Time.time + _cooldown;
				return Children[0].Process();
			}
			return Status.Failure;
		}
	}
}