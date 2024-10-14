using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
namespace AstekUtility.Gameplay
{
	public class ObstacleDetector : Detector
	{
		private void Start()
		{
			_aiData.AvoidedObjectCollection.Add(this, null);
		}

		#if UNITY_EDITOR
		public override void OnDrawGizmos()
		{
			if (_showGizmos == false || _aiData == null || !_aiData.AvoidedObjectCollection.ContainsKey(this))
				return;

			Gizmos.color = _gizmoColor;

			if (_aiData.AvoidedObjectCollection[this] == null)
				return;

			foreach (Collider obstacleCollider in _aiData.AvoidedObjectCollection[this])
			{
				if (obstacleCollider != null)
					Gizmos.DrawSphere(obstacleCollider.transform.position, 0.4f);
			}
		}
		#endif

		public override void Detect()
		{
			if (!_mainModel)
				return;
			_aiData.AvoidedObjectCollection[this] = Physics.OverlapSphere(_mainModel.position, detectionRadius, detectObjectInLayer);
		}
	}
}