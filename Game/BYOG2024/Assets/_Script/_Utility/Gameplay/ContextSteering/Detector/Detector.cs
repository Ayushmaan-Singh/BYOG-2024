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

		public abstract void Detect();

		#if UNITY_EDITOR
		[SerializeField] protected bool _showGizmos = true;
		[SerializeField] protected Color _gizmoColor = Color.magenta;
		public abstract void OnDrawGizmos();
		#endif

		public class Builder
		{
			private AIData _aiData;
			private Transform _mainModel;

			public Builder InitAIData(AIData aiData)
			{
				_aiData = aiData;
				return this;
			}
			
			public Builder InitMainModel(Transform model)
			{
				_mainModel = model;
				return this;
			}
			
			public Detector Build(Detector instance)
			{
				instance._aiData = _aiData;
				instance._mainModel = _mainModel;

				instance.enabled = true;
				return instance;
			}
		}
	}
}