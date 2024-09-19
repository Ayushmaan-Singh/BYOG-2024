using UnityEngine;
namespace AstekUtility.Gameplay
{
	public abstract class SteeringBehaviour : MonoBehaviour
	{
		protected AIData _aiData;
		protected Vector3[] _direction8SidesXZ;
		protected Transform _mainModel;

		public void Init(Transform mainModel, Vector3[] direction8SidesXZ, AIData aIData)
		{
			_direction8SidesXZ = direction8SidesXZ;
			_mainModel = mainModel;
			_aiData = aIData;
		}
		public abstract (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest);

		[SerializeField] protected bool _showGizmo = true;
		[SerializeField] protected Color _gizmoColor = Color.yellow;
	}
}