using System.Collections.Generic;
using AstekUtility.DesignPattern.StateMachine;
using AstekUtility.Gameplay;
using AstekUtility.Gameplay.Timer;
using Entity;
using Entity.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat.Enemy
{
	[System.Serializable]
	public class MeleeAttackState : BaseState
	{
		[Header("State Property")]
		[SerializeField] private Transform mainBody;
		[SerializeField] private EntityStatSystem entityStats;
		[SerializeField] private EntityAbilitySystem abilitySystem;
		[SerializeField] private float rotationSpeed;

		[Header("Player Detection And Navigation")]
		[SerializeField, InlineProperty] private ContextSteering contextSteering;
		private AbilityBase meleeAbility;
		private Rigidbody _rb;

		public override void OnStateEnter()
		{
			if (!_rb)
				_rb = mainBody.GetComponent<Rigidbody>();
		}
		public override void FrameUpdate()
		{
			AbilityBase ability = abilitySystem.GetAbilities[0];
			if (ability.CurrentState != AbilityBase.State.Usable)
				return;

			ability.Execute();
			meleeAbility = abilitySystem.GetAbilities[0];
		}
		
		public override void PhysicsUpdate()
		{
			Vector3 dir = contextSteering.Direction;

			if (meleeAbility.CurrentState != AbilityBase.State.InProgress)
				_rb.MoveRotation(Quaternion.Slerp(_rb.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed));
		}
		public override void OnStateExit()
		{
			//noap
		}
	}
}