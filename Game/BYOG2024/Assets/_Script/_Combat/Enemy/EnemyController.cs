using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.DesignPattern.StateMachine;
using AstekUtility.Gameplay;
using Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat.Enemy
{
	public class EnemyController : MonoBehaviour, IDamageable
	{
		[SerializeField] private Rigidbody rb;

		[Header("Sub Components")]
		[SerializeField] private EntityHealthManager healthManager;
		[SerializeField] private EntityStatSystem statSystem;

		protected StateMachine _stateMachine = new StateMachine();

		protected void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		protected void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		protected virtual void InitializeStateMachine()
		{
			//noap
		}

		#region Health Manager

		public float MaxHp => statSystem.GetInstanceStats(Stats.Hp);
		public float CurrentHp => healthManager.CurrentHP;

		public void Damage(float amount)
		{
			healthManager.Damage(amount);
		}
		public void Heal(float amount)
		{
			healthManager.Heal(amount);
		}

		#endregion

	}
}