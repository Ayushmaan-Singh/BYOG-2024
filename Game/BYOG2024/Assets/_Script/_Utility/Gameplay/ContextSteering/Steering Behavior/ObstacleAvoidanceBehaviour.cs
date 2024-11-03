using UnityEngine;
namespace AstekUtility.Gameplay
{
	/// <summary>
	///     Used by objects that are treated to be avoided by the AI to a certain degree
	///     Object like obstacles are considered automatically at max weight of avoidance i.e 1
	///     Object like other enemies are considered at a lower weight of avoidance i.e less than 1
	/// </summary>
	public class ObstacleAvoidanceBehaviour : SteeringBehaviour
	{
		[SerializeField] protected Detector detector;
		[SerializeField] protected float radius = 2f;
		[SerializeField] protected float agentColliderSize = 0.6f;
		[SerializeField] [Range(0, 1f)] protected float maxAvoidanceWeight = 1f;

		//gizmo parameters
		private float[] _dangersResultTemp;

		public void OnDrawGizmos()
		{
			if (_showGizmo == false)
				return;

			//Gizmos.color = Color.red;
			//Gizmos.DrawSphere(mainModel.position, _agentColliderSize);

			if (Application.isPlaying && _dangersResultTemp != null)
			{
				if (_dangersResultTemp != null)
				{
					Gizmos.color = _gizmoColor;
					int size = _dangersResultTemp.Length;
					for (int i = 0; i < size; i++)
					{
						Gizmos.DrawRay(
							_mainModel.position,
							_directionXZ[i] * _dangersResultTemp[i] * 2
						);
					}
				}
			}

		}

		public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest)
		{
			if (!_aiData.AvoidedObjectCollection.ContainsKey(detector))
				return (danger, interest);
			foreach (Collider obstacleCollider in _aiData.AvoidedObjectCollection[detector])
			{
				Vector3 directionToObstacle
					= obstacleCollider.ClosestPoint(_mainModel.position) - _mainModel.position;
				float distanceToObstacle = directionToObstacle.magnitude;

				//calculate weight based on the distance Enemy<--->Obstacle
				float calculatedWeight = Mathf.Clamp01((radius - distanceToObstacle) / radius);
				float weight
					= distanceToObstacle <= agentColliderSize && calculatedWeight > maxAvoidanceWeight
						? maxAvoidanceWeight
						: calculatedWeight;

				Vector3 directionToObstacleNormalized = directionToObstacle.normalized;

				int size = _directionXZ.Length;
				//Add obstacle parameters to the danger array
				for (int i = 0; i < size; i++)
				{
					float result = Vector3.Dot(directionToObstacleNormalized, _directionXZ[i]);

					float valueToPutIn = result * weight;

					//override value only if it is higher than the current one stored in the danger array
					if (valueToPutIn > danger[i])
					{
						danger[i] = valueToPutIn;
					}
				}
			}
			_dangersResultTemp = danger;
			return (danger, interest);
		}
	}
}