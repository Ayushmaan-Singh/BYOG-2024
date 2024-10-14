using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.DesignPattern.StateMachine;
using AstekUtility.Gameplay;
using Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat.Enemy
{
	[System.Serializable]
	public class ChaseState : BaseState
	{
		[Header("State Property")]
		[SerializeField] private Transform mainBody;
		[SerializeField] private EntityStatSystem entityStats;
		[SerializeField] private float rotationSpeed;

		[Header("Player Detection And Navigation")]
		[SerializeField, InlineProperty] private List<Detector> detectors = new List<Detector>();
		[SerializeField, InlineProperty] private List<SteeringBehaviour> steeringBehaviours = new List<SteeringBehaviour>();

		private ChaseBehavior _playerDirection;
		private Rigidbody _rb;

		public override void OnStateEnter()
		{
			if (!_rb)
				_rb = mainBody.GetComponent<Rigidbody>();

			_playerDirection ??= new ChaseBehavior(steeringBehaviours, detectors, mainBody);
		}
		public override void FrameUpdate()
		{
			//noap
		}
		public override void PhysicsUpdate()
		{
			Vector3 dir = _playerDirection.UpdateDirection();

			_rb.MovePosition(_rb.position + dir * (entityStats.GetInstanceStats(Stats.MovementSpeed) * Time.fixedDeltaTime));
			_rb.MoveRotation(Quaternion.Slerp(_rb.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed));
		}
		public override void OnStateExit()
		{
			//noap
		}
	}

	public class ChaseBehavior : ContextSteering
	{
		public ChaseBehavior(List<SteeringBehaviour> steeringBehaviours, List<Detector> detectors, Transform mainModel) : base(steeringBehaviours, detectors, mainModel) { }
	}
}