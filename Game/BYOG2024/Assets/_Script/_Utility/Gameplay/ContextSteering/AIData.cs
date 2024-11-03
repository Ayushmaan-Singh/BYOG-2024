using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class AIData
	{
		public Dictionary<Detector, Collider[]> AvoidedObjectCollection = new Dictionary<Detector, Collider[]>();

		public Collider CurrentTarget;
		public readonly List<Collider> Targets = new List<Collider>();

		public AIData() { }

		public int GetTargetsCount()
		{
			return Targets?.Count ?? 0;
		}
	}
}