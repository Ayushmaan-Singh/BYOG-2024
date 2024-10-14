using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.DesignPattern.StateMachine;
using Entity.Player;
using UnityEngine;

namespace Combat.Enemy.Shooter
{
	public class ShooterEnemy : EnemyController
	{
		[SerializeField] private ChaseState chaseState;
		[SerializeField] private RangedAttackState rangedAttackState;

		[SerializeField] private float meleeAttackRange;

		[SerializeField] private Transform playerHolder;
		private Transform _playerTransform;
		
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
			_stateMachine.AddTransition(rangedAttackState, chaseState, new Predicate(() =>  _playerTransform 
			                                                                               && (_playerTransform.position - transform.position).sqrMagnitude > meleeAttackRange));
			_stateMachine.AddTransition(chaseState, rangedAttackState, new Predicate(() => _playerTransform 
			                                                                              && (_playerTransform.position - transform.position).sqrMagnitude <= meleeAttackRange));
			
			_stateMachine.ChangeState(chaseState);
		}

		private void Update()
		{
			_playerTransform ??= playerHolder.GetChild(0).GetComponentInChildren<PlayerController>().transform;
			if (!_playerTransform)
				return;
			_stateMachine.FrameUpdate();
		}

		private void FixedUpdate()
		{
			if (!_playerTransform)
				return;
			_stateMachine.PhysicsUpdate();
		}
	}
}