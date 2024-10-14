using System.Collections.Generic;
using System.Linq;
using AstekUtility;
using AstekUtility.Gameplay;
using Combat.Player;
using Entity.Player;
using UnityEngine;

namespace Combat.Enemy
{
	public class PlayerDetector : Detector
	{
		[SerializeField] private PlayerRuntimeSet playerRTSet;

		public override void Detect()
		{
			//Cleans the list in case player is destroyed
			_aiData.Targets = _aiData.Targets.Where(target => target.OrNull()).ToList();
			
			if (_aiData.Targets.Count > 0)
				return;
			
			foreach (PlayerMediator child in playerRTSet.PlayersInGame)
			{
				if (!_aiData.Targets.Contains(child.ColliderDetectedByContextSteering))
					_aiData.Targets.Add(child.ColliderDetectedByContextSteering);
			}
		}

		#if UNITY_EDITOR
		//gizmo parameters
		[SerializeField] private bool _showIfTargetIsInVisualRange;

		public override void OnDrawGizmos()
		{
			if (_showGizmos == false || _aiData == null)
				return;

			Gizmos.color = _gizmoColor;
			if (_aiData.Targets == null)
				return;

			foreach (Collider item in _aiData.Targets)
			{
				Gizmos.DrawSphere(item.transform.position, 0.4f);
			}
		}
		#endif
	}
}