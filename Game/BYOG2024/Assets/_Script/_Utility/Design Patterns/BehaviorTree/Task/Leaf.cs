namespace AstekUtility.BehaviorTree
{
	public class Leaf : Node
	{
		public delegate Status Tick();

		public delegate Status TickM(int val);

		public int index;
		public TickM multiProcessMethod;
		public Tick processMethod;

		public Leaf() { }

		public Leaf(string name, Tick pm)
		{
			Name = name;
			processMethod = pm;
		}
		public Leaf(string name, Tick pm, int order)
		{
			Name = name;
			processMethod = pm;
			SortOrder = order;
		}

		public Leaf(string name, TickM pm, int index)
		{
			Name = name;
			multiProcessMethod = pm;
			this.index = index;
		}

		public override Status Process()
		{
			if (processMethod != null)
				return processMethod();
			if (multiProcessMethod != null)
				return multiProcessMethod(index);

			return Status.Failure;
		}
	}
}