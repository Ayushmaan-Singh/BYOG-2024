using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.BehaviorTree
{
	public class BehaviorTree : Node
	{
		public BehaviorTree()
		{
			Name = "Tree";
		}

		public BehaviorTree(string name) : base(name) { }

		public override Status Process()
		{
			if (Children.Count == 0)
			{
				return Status.Success;
			}
			return Children[_currentChild].Process();
		}


#if UNITY_EDITOR
		private struct NodeLevel
		{
			public int level;
			public Node node;
		}

		[Tooltip("Only For Debug Purpose")]
		public void PrintTree()
		{
			string printTree = "";
			Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
			Node currentNode = this;
			nodeStack.Push(new NodeLevel
			{
				level = 0,
				node = currentNode
			});

			int stackCount = nodeStack.Count;
			while (stackCount != 0)
			{
				NodeLevel nextNode = nodeStack.Pop();
				printTree += new string('-', nextNode.level) + nextNode.node.Name + "\n";

				for (int i = nextNode.node.Children.Count - 1; i >= 0; i--)
				{
					nodeStack.Push(new NodeLevel
					{
						level = nextNode.level + 1,
						node = nextNode.node.Children[i]
					});
				}
			}
			Debug.Log(printTree);
		}
#endif
	}
}