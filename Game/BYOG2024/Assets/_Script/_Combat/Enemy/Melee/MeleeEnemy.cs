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
		
		public new void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		private void Start()
		{
			InitializeStateMachine();
		}

		public new void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		protected override void InitializeStateMachine()
		{
			_stateMachine.AddTransition(meleeAttackState, chaseState, new Predicate(() =>  playerRTSet.Owner.OrNull() 
			                                                                               && (playerRTSet.Owner.MainBodyRb.transform.position - transform.position).sqrMagnitude > meleeAttackRange));
			_stateMachine.AddTransition(chaseState, meleeAttackState, new Predicate(() => playerRTSet.Owner.OrNull()
			                                                                              && (playerRTSet.Owner.MainBodyRb.transform.position - transform.position).sqrMagnitude <= meleeAttackRange));
			
			_stateMachine.ChangeState(chaseState);
		}

		private void Update()
		{
			_stateMachine.FrameUpdate();
		}

		private void FixedUpdate()
		{
			_stateMachine.PhysicsUpdate();
		}
	}
}