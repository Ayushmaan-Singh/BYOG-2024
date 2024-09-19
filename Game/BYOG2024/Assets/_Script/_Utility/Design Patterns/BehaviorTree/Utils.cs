using System;
using System.Collections.Generic;
namespace AstekUtility.BehaviorTree
{
	public class Utils
	{
		public Random r = new Random();

		public List<T> Shuffle<T>(List<T> list)
		{
			int n = list.Count;
			List<T> newList = list;
			while (n > 1)
			{
				int k = r.Next(n + 1);
				T val = newList[k];
				newList[k] = newList[n];
				newList[n] = val;
			}

			return newList;
		}
	}
}