using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public abstract class Detector : MonoBehaviour
	{
		[SerializeField] protected float detectionRadius = 1000;
		[SerializeField] protected LayerMask detectObjectInLayer;
		protected AIData _aiData;
		protected Transform _mainModel;

		public void Init(Transform mainModel, AIData aIData)
		{
			_mainModel = mainModel;
			_aiData = aIData;
		}
		public abstract void Detect();

		#if UNITY_EDITOR
		[SerializeField] protected bool _showGizmos = true;
		[SerializeField] protected Color _gizmoColor = Color.magenta;
		public abstract void OnDrawGizmos();
		#endif
	}
}