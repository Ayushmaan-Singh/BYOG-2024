using System;
using System.Collections.Generic;
namespace AstekUtility.DesignPattern.Blackboard
{
	public class Arbiter
	{
		private readonly List<IExpert> experts;

		public void RegisterExpert(IExpert expert)
		{
			Preconditions.CheckNotNull(expert);
			experts.Add(expert);
		}

		public List<Action> BlackboardInteraction(Blackboard blackboard)
		{
			IExpert bestExpert = null;
			int highestInsistence = 0;

			foreach (IExpert expert in experts)
			{
				int insistence = expert.GetInsistence(blackboard);
				if (insistence > highestInsistence)
				{
					highestInsistence = insistence;
					bestExpert = expert;
				}
			}
			
			bestExpert?.Execute(blackboard);
			List<Action> actions = blackboard.PassedActions;
			blackboard.ClearActions();

			//Return or execute actions here
			return actions;
		}
	}
}