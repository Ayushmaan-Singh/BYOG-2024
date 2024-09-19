using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class AIData
	{
		public Dictionary<Detector, Collider[]> AvoidedObjectCollection;

		public Collider CurrentTarget;
		public Collider[] Targets;

		public AIData()
		{
			AvoidedObjectCollection = new Dictionary<Detector, Collider[]>();
		}
		
		public int GetTargetsCount()
		{
			return Targets == null ? 0 : Targets.Length;
		}
	}
}