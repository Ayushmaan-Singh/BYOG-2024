namespace AstekUtility.BehaviorTree
{
    /// <summary>
    ///     Act as if, else if and else
    ///     stops as soon as one of its child return success
    /// </summary>
    public class Selector : Node
	{
		public Selector(string name) : base(name) { }

		public override Status Process()
		{
			Status childStatus = Children[_currentChild].Process();

			if (childStatus == Status.Running)
				return Status.Running;
			if (childStatus == Status.Success)
			{
				_currentChild = 0;
				return Status.Success;
			}

			_currentChild++;
			if (_currentChild >= Children.Count)
			{
				_currentChild = 0;
				return Status.Failure;
			}

			return Status.Running;
		}
	}
}