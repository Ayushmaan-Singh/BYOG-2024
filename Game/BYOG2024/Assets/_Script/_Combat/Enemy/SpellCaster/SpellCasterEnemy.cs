using AstekUtility.DesignPattern.ServiceLocatorTool;
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
			_stateMachine.AddTransition(spellCastAttackState, chaseState, new Predicate(() =>  _playerTransform 
			                                                                               && (_playerTransform.position - transform.position).sqrMagnitude > meleeAttackRange));
			_stateMachine.AddTransition(chaseState, spellCastAttackState, new Predicate(() => _playerTransform 
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