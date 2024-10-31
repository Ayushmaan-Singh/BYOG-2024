using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.DesignPattern.StateMachine;
using AstekUtility.Gameplay;
using Combat.Player;
using Entity.Player;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
namespace Combat.Enemy
{
	public class MeleeEnemy : EnemyController
	{
		[SerializeField] private ChaseState chaseState;
		[SerializeField] private MeleeAttackState meleeAttackState;

		[SerializeField] private float meleeAttackRange;

		[SerializeField] private PlayerRuntimeSet playerRTSet;

		private void Start()
		{
			InitializeStateMachine();
		}

		protected override void InitializeStateMachine()
		{
			_masterStateMachine.AddTransition(meleeAttackState, chaseState, new Predicate(() =>  playerRTSet.Owner.OrNull() 
			                                                                               && (playerRTSet.Owner.MainBodyRb.transform.position - transform.position).sqrMagnitude > meleeAttackRange));
			_masterStateMachine.AddTransition(chaseState, meleeAttackState, new Predicate(() => playerRTSet.Owner.OrNull()
			                                                                              && (playerRTSet.Owner.MainBodyRb.transform.position - transform.position).sqrMagnitude <= meleeAttackRange));
			
			_masterStateMachine.ChangeState(chaseState);
		}

		private void Update()
		{
			_masterStateMachine.FrameUpdate();
		}

		private void FixedUpdate()
		{
			_masterStateMachine.PhysicsUpdate();
		}
	}
}