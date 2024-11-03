using System.Collections.Generic;
using AstekUtility;
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
		[SerializeField] private ContextSteering _contextSteering;
		
		private Rigidbody _rb;

		public override void OnStateEnter()
		{
			if (!_rb)
				_rb = mainBody.GetComponent<Rigidbody>();
		}
		public override void FrameUpdate()
		{
			//noap
		}
		public override void PhysicsUpdate()
		{
			Vector3 dir = _contextSteering.Direction;
			_rb.MoveRotation(Quaternion.Slerp(_rb.rotation, Quaternion.LookRotation(dir), Time.fixedDeltaTime * rotationSpeed));
			_rb.MovePosition(_rb.position + dir * (entityStats.GetInstanceStats(Stats.MovementSpeed) * Time.fixedDeltaTime));
		}
		public override void OnStateExit()
		{
			//noap
		}
	}
}