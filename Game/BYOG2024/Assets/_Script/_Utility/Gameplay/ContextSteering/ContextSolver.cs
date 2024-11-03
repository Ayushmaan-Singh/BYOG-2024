using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	[Serializable]
	public class ContextSolver
	{
		private Vector3[] _direction;
		protected Transform _mainModel;

		public ContextSolver(Vector3[] direction, Transform mainModel)
		{
			_direction = direction;
			_mainModel = mainModel;
		}

		public Vector3 GetDirectionToMove(SteeringBehaviour[] behaviours)
		{
			float[] danger = new float[_direction.Length];
			float[] interest = new float[_direction.Length];

			//Loop through each behaviour
			foreach (SteeringBehaviour behaviour in behaviours)
			{
				(danger, interest) = behaviour.GetSteering(danger, interest);
			}

			//subtract danger _values from interest array
			int directionCount = _direction.Length;
			for (int i = 0; i < directionCount; i++)
			{
				interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
			}

#if UNITY_EDITOR

			_interestGizmo = interest;

#endif

			//get the average _direction
			Vector3 outputDirection = Vector3.zero;
			for (int i = 0; i < directionCount; i++)
			{
				outputDirection += _direction[i] * interest[i];
			}

			outputDirection.Normalize();

# if UNITY_EDITOR

			_resultDirection = outputDirection;

#endif
			//return the selected movement _direction
			return outputDirection;
		}
# if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			if (Application.isPlaying && _showGizmos)
			{
				Gizmos.color = _gizmoColor;
				Gizmos.DrawRay(_mainModel.position, _resultDirection * _rayLength);
			}
		}
		//gizmo parameters
		[SerializeField] private Color _gizmoColor = Color.yellow;
		[SerializeField] private bool _showGizmos = true;
		private float[] _interestGizmo;
		private Vector3 _resultDirection = Vector3.zero;
		[SerializeField] private float _rayLength = 2;
#endif
	}
}