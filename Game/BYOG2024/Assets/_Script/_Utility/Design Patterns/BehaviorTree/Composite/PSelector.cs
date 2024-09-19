using System.Collections.Generic;
namespace AstekUtility.BehaviorTree
{
	public class PSelector : Node
	{
		public PSelector(string name) : base(name) { }

		private void OrderNodes()
		{
			Sort(Children, 0, Children.Count - 1);
		}

		public override Status Process()
		{
			OrderNodes();

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

		//=========================Quick Sort=============================//
		private int Partition(List<Node> array, int low,
			int high)
		{
			Node pivot = array[high];

			int lowIndex = low - 1;

			//2. Reorder the collection.
			for (int j = low; j < high; j++)
			{
				if (array[j].SortOrder <= pivot.SortOrder)
				{
					lowIndex++;

					Node temp = array[lowIndex];
					array[lowIndex] = array[j];
					array[j] = temp;
				}
			}

			Node temp1 = array[lowIndex + 1];
			array[lowIndex + 1] = array[high];
			array[high] = temp1;

			return lowIndex + 1;
		}

		private void Sort(List<Node> array, int low, int high)
		{
			if (low < high)
			{
				int partitionIndex = Partition(array, low, high);
				Sort(array, low, partitionIndex - 1);
				Sort(array, partitionIndex + 1, high);
			}
		}
	}
}