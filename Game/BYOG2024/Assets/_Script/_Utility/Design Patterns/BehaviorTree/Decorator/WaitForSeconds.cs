using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.BehaviorTree
{
	public class WaitForSeconds : Node
	{
		private readonly float _delay;
		private float _timeCounter;

		public WaitForSeconds(string name, float delay) : base(name)
		{
			_delay = delay;
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
				Debug.LogError($"WaitForSeconds Node {Name}: More than one child node");
			}
		}

		public override Status Process()
		{
			if (_timeCounter >= _delay)
			{
				_timeCounter = 0;
				return Children[0].Process();
			}
			_timeCounter += Time.deltaTime;
			return Status.Running;
		}
	}
}