﻿using System.Collections.Generic;
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
	public class SpellCastingAttackState:BaseState
	{
		[Header("State Property")]
		[SerializeField] private Transform mainBody;
		[SerializeField] private EntityStatSystem entityStats;
		[SerializeField] private EntityAbilitySystem abilitySystem;
		[SerializeField] private float rotationSpeed;

		[Header("Player Detection And Navigation")]
		[SerializeField, InlineProperty] private List<Detector> detectors = new List<Detector>();
		[SerializeField, InlineProperty] private List<SteeringBehaviour> steeringBehaviours = new List<SteeringBehaviour>();

		private ChaseBehavior _playerDirection;
		private Rigidbody _rb;
		private AbilityBase _abilityBeingUsed;
		private bool _isInPositionToShoot = false;

		public override void OnStateEnter()
		{
			if (!_rb)
				_rb = mainBody.GetComponent<Rigidbody>();

			_playerDirection ??= new ChaseBehavior(steeringBehaviours, detectors, mainBody);

		}
		public override void FrameUpdate()
		{
			foreach (AbilityBase ability in abilitySystem.GetAbilities)
			{
				if (ability.CurrentState != AbilityBase.State.Usable || !_isInPositionToShoot)
					return;

				_abilityBeingUsed = ability;
				ability.Execute();
			}

			if (_abilityBeingUsed && _abilityBeingUsed.CurrentState == AbilityBase.State.Unusable)
				_abilityBeingUsed = null;
		}
		public override void PhysicsUpdate()
		{
			Vector3 dir = _playerDirection.UpdateDirection();
			Quaternion targetRotation = Quaternion.LookRotation(dir);
			if (!_abilityBeingUsed || _abilityBeingUsed.CurrentState != AbilityBase.State.InProgress)
				_rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));

			if (_rb.rotation.IsRotationApproximatelySame(targetRotation, 0.05f))
				_isInPositionToShoot = true;
		}
		public override void OnStateExit()
		{
			//noap
		}
	}
}