using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.DesignPattern.StateMachine;
using Combat.Player;
using Entity.Player;
using UnityEngine;

namespace Combat.Enemy.Shooter
{
	public class ShooterEnemy : EnemyController
	{
		[SerializeField] private PlayerRuntimeSet playerRTSet;
		[SerializeField] private ChaseState chaseState;
		[SerializeField] private RangedAttackState rangedAttackState;

		[SerializeField] private float meleeAttackRange;

		private void Start()
		{
			//InitializeStateMachine();
		}

		protected override void InitializeStateMachine()
		{
			Transform _playerTransform = playerRTSet.Owner.MainMovementBody;
			
			_masterStateMachine.AddTransition(rangedAttackState, chaseState, new Predicate(() =>  _playerTransform 
			                                                                               && (_playerTransform.position - transform.position).sqrMagnitude > meleeAttackRange));
			_masterStateMachine.AddTransition(chaseState, rangedAttackState, new Predicate(() => _playerTransform 
			                                                                              && (_playerTransform.position - transform.position).sqrMagnitude <= meleeAttackRange));
			
			_masterStateMachine.ChangeState(chaseState);
		}

		private void Update()
		{
			//_masterStateMachine.FrameUpdate();
		}

		private void FixedUpdate()
		{
			//_masterStateMachine.PhysicsUpdate();
		}
	}
}