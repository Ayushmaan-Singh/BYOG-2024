namespace AstekUtility.BehaviorTree
{
	public class RSequence : Node
	{
		//this bool is for making the AI decisive ,keep it false for a more unique behavior
		private bool _shuffleOnce;
		private readonly Utils suffling = new Utils();

		public RSequence(string name, bool shuffleOnce = false) : base(name)
		{
			_shuffleOnce = shuffleOnce;
		}

		public override Status Process()
		{
			if (_shuffleOnce)
			{
				Children = suffling.Shuffle(Children);
				_shuffleOnce = true;
			}
			else
			{
				Children = suffling.Shuffle(Children);
			}

			Status childStatus = Children[_currentChild].Process();

			if (childStatus == Status.Running)
				return Status.Running;
			if (childStatus == Status.Failure)
			{
				_currentChild = 0;
				_shuffleOnce = false;
				return Status.Failure;
			}

			_currentChild++;
			if (_currentChild >= Children.Count)
			{
				_currentChild = 0;
				_shuffleOnce = false;
				return Status.Success;
			}

			return Status.Running;
		}
	}
}