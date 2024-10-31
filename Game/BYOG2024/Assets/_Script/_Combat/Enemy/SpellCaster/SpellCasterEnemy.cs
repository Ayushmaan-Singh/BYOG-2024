﻿using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.DesignPattern.StateMachine;
using Entity.Player;
using UnityEngine;
namespace Combat.Enemy.SpellCaster
{
	public class SpellCasterEnemy : EnemyController
	{
		[SerializeField] private ChaseState chaseState;
		[SerializeField] private SpellCastingAttackState spellCastAttackState;

		[SerializeField] private float meleeAttackRange;

		[SerializeField] private Transform playerHolder;
		private Transform _playerTransform;

		private void Start()
		{
			InitializeStateMachine();
		}

		protected override void InitializeStateMachine()
		{
			_masterStateMachine.AddTransition(spellCastAttackState, chaseState, new Predicate(() =>  _playerTransform 
			                                                                               && (_playerTransform.position - transform.position).sqrMagnitude > meleeAttackRange));
			_masterStateMachine.AddTransition(chaseState, spellCastAttackState, new Predicate(() => _playerTransform 
			                                                                              && (_playerTransform.position - transform.position).sqrMagnitude <= meleeAttackRange));
			
			_masterStateMachine.ChangeState(chaseState);
		}

		private void Update()
		{
			_playerTransform ??= playerHolder.GetChild(0).GetComponentInChildren<PlayerController>().transform;
			if (!_playerTransform)
				return;
			_masterStateMachine.FrameUpdate();
		}

		private void FixedUpdate()
		{
			if (!_playerTransform)
				return;
			_masterStateMachine.PhysicsUpdate();
		}
	}
}