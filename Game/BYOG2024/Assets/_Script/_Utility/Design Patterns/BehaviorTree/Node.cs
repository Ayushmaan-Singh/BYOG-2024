using System.Collections.Generic;
namespace AstekUtility.BehaviorTree
{
	public class Node
	{

		public enum Status
		{
			Running,
			Success,
			Failure
		}
		protected int _currentChild;

		public Node() { }
		public Node(string name)
		{
			Name = name;
		}

		public Node(string name, int order)
		{
			Name = name;
			SortOrder = order;
		}
		public string Name { get; protected set; }
		public int SortOrder { get; protected set; }

		public Status NodeStatus { get; protected set; }

		//Put in order they are to be executed if its a sequence type Node
		public List<Node> Children { get; protected set; } = new List<Node>();

		public void AddChild(Node n)
		{
			Children.Add(n);
		}

		public virtual Status Process()
		{
			return Children[_currentChild].Process();
		}

		public void Reset()
		{
			foreach (Node n in Children)
				n.Reset();
			_currentChild = 0;
		}
	}
}