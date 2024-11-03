using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class TargetDetector : Detector
	{
		//gizmo parameters
		[SerializeField] private bool _showIfTargetIsInVisualRange;

		#if UNITY_EDITOR
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

		public override void Detect()
		{
			if (!_mainModel)
				return;

			_aiData.Targets.Clear();
			_aiData.Targets.AddRange(Physics.OverlapSphere(_mainModel.position, detectionRadius, detectObjectInLayer).ToList());
			;
		}
	}
}