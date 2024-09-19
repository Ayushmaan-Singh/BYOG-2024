//To use dependency sequence 
//Make a behavior tree for dependecy behavior add all the behavior to this dependency tree
//Add child nodes to this node
//With above setup every frame 

namespace AstekUtility.BehaviorTree
{
    /// <summary>
    ///     This is a dependency sequence used when we want to check a condition while a particular sequence
    /// </summary>
    public class DepSequence : Node
	{
		private readonly BehaviorTree _dependancy;

		public DepSequence(string name, BehaviorTree dependency)
		{
			Name = name;
			_dependancy = dependency;
		}

		public override Status Process()
		{
			if (_dependancy.Process() == Status.Failure)
			{
				//Reset All Children
				foreach (Node n in Children)
					n.Reset();
				return Status.Failure;
			}

			Status childStatus = Children[_currentChild].Process();

			if (childStatus == Status.Running)
				return Status.Running;
			if (childStatus == Status.Failure)
				return Status.Failure;

			_currentChild++;
			if (_currentChild >= Children.Count)
			{
				_currentChild = 0;
				return Status.Success;
			}

			return Status.Running;
		}
	}
}