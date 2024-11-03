using System.Linq;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class SeekBehaviour : SteeringBehaviour
	{
		[SerializeField] private float _targetReachedThreshold = 0.5f;
		private bool _reachedLastTarget = true;

		public void OnDrawGizmos()
		{

			if (_showGizmo == false)
				return;
			Gizmos.DrawSphere(_targetPositionCached, 0.2f);

			if (Application.isPlaying && _interestsTemp != null)
			{
				if (_interestsTemp != null)
				{
					Gizmos.color = Color.green;
					for (int i = 0; i < _interestsTemp.Length; i++)
					{
						Gizmos.DrawRay(_mainModel.position, _directionXZ[i] * _interestsTemp[i] * 2);
					}
					if (_reachedLastTarget == false)
					{
						Gizmos.color = _gizmoColor == Color.green ? new Color(_gizmoColor.r, 0, 0) : _gizmoColor;
						Gizmos.DrawSphere(_targetPositionCached, 0.2f);
					}
				}
			}
		}

		public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest)
		{
			//if we don't have a target stop seeking
			//else set a new target
			if (_reachedLastTarget)
			{
				if (_aiData.Targets == null || _aiData.GetTargetsCount() <= 0)
				{
					_aiData.CurrentTarget = null;
					return (danger, interest);
				}
				_reachedLastTarget = false;
				_aiData.CurrentTarget = _aiData.Targets.OrderBy
					(target => Mathf.Sqrt((_mainModel.position - target.transform.position).sqrMagnitude)).FirstOrDefault();

			}

			//cache the last position only if we still see the target (if the targets collection is not empty)
			if (_aiData.CurrentTarget && _aiData.Targets != null && _aiData.Targets.Contains(_aiData.CurrentTarget))
				_targetPositionCached = _aiData.CurrentTarget.transform.position;

			//First check if we have reached the target
			if (Mathf.Sqrt((_mainModel.position - _targetPositionCached).sqrMagnitude) < _targetReachedThreshold)
			{
				_reachedLastTarget = true;
				_aiData.CurrentTarget = null;
				return (danger, interest);
			}

			//If we havent yet reached the target do the main logic of finding the interest directions
			Vector3 directionToTarget = _targetPositionCached - _mainModel.position;
			for (int i = 0; i < interest.Length; i++)
			{
				float result = Vector3.Dot(directionToTarget.normalized, _directionXZ[i]);

				//accept only directions at the less than 90 degrees to the target direction
				if (result > 0)
				{
					float valueToPutIn = result;
					if (valueToPutIn > interest[i])
					{
						interest[i] = valueToPutIn;
					}

				}
			}
			
			_interestsTemp = interest;
			return (danger, interest);
		}

		//gizmo parameters
		private Vector3 _targetPositionCached;
		private float[] _interestsTemp;
	}
}