using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.StateMachine;
using AstekUtility.Gameplay;
using Entity;
using Entity.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Combat.Enemy.SpellCaster
{
	[System.Serializable]
	public class SpellCastingAttackState : BaseState
	{
		[Header("State Property")]
		[SerializeField] private Transform mainBody;
		[SerializeField] private EntityStatSystem entityStats;
		[SerializeField] private EntityAbilitySystem abilitySystem;
		[SerializeField] private float rotationSpeed;

		[Header("Target Detection")]
		[SerializeField, InlineProperty] private ContextSteering contextSteering;
		private Rigidbody _rb;
		private AbilityBase _abilityBeingUsed;

		private bool _canAttack = false;
		public bool IsAttacking => _abilityBeingUsed != null;

		public override void OnStateEnter()
		{
			if (!_rb)
				_rb = mainBody.GetComponent<Rigidbody>();
		}
		public override void FrameUpdate()
		{
			if (_canAttack)
			{
				foreach (AbilityBase ability in abilitySystem.GetAbilities)
				{
					if (ability.CurrentState != AbilityBase.State.Usable)
						return;

					_abilityBeingUsed = ability;
					ability.Execute();
				}
			}


			if (_abilityBeingUsed && _abilityBeingUsed.CurrentState == AbilityBase.State.Unusable)
			{
				_canAttack = false;
				_abilityBeingUsed = null;
			}
		}
		public override void PhysicsUpdate()
		{
			if (_abilityBeingUsed)
				return;

			Vector3 dir = contextSteering.Direction;
			Quaternion targetRotation = Quaternion.LookRotation(dir);
			if (!_abilityBeingUsed || _abilityBeingUsed.CurrentState != AbilityBase.State.InProgress)
				_rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed));

			if (_rb.rotation.IsRotationApproximatelySame(targetRotation, 0.05f) && !_canAttack)
				_canAttack = true;
		}
		public override void OnStateExit()
		{
			_canAttack = false;
			_abilityBeingUsed = null;
		}
	}
}