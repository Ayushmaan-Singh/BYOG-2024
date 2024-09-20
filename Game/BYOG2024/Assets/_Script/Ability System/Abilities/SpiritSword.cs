using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
namespace Entity.Abilities
{
	public class SpiritSword : AbilityBase
	{
		[SerializeField] private GameObject sword;
		[SerializeField] private SplineContainer attackSpline;


		public override void Execute()
		{
			
		}
		public override void CancelExecution()
		{
			throw new System.NotImplementedException();
		}

		// private IEnumerator SwordSlash()
		// {
		// 	for(int i=0;i<)
		// }
	}
}