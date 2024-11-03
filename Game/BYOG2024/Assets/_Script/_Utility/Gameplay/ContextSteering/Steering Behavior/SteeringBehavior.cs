using System;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public abstract class SteeringBehaviour : MonoBehaviour
	{
		protected AIData _aiData;
		protected Vector3[] _directionXZ;
		protected Transform _mainModel;

		private void Awake()
		{
			enabled = false;
		}
		public abstract (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest);

		[SerializeField] protected bool _showGizmo = true;
		[SerializeField] protected Color _gizmoColor = Color.yellow;

		public class Builder
		{
			private Vector3[] _directionXZ;
			private Transform _mainModel;
			private AIData _aiData;

			public Builder InitDirectionXZ(Vector3[] directionXZ)
			{
				_directionXZ = directionXZ;
				return this;
			}
			
			public Builder InitMainModel(Transform model)
			{
				_mainModel = model;
				return this;
			}
			
			public Builder InitAIData(AIData aiData)
			{
				_aiData = aiData;
				return this;
			}
			
			public SteeringBehaviour Build(SteeringBehaviour instance)
			{
				instance._aiData = _aiData;
				instance._mainModel = _mainModel;
				instance._directionXZ = _directionXZ;

				instance.enabled = true;
				return instance;
			}
		}
	}
}