using System.Threading.Tasks;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.DesignPattern.StateMachine;
using AstekUtility.Gameplay;
using Entity;
using Entity.Abilities;
using Global;
using UnityEngine;

namespace Combat.Enemy
{
	public abstract class EnemyController : MonoBehaviour, IDamageable
	{
		[SerializeField] protected Rigidbody rb;
		[SerializeField] protected Renderer[] renderers;

		[Header("Sub Components")]
		[SerializeField] protected EntityHealthManager healthManager;
		[SerializeField] protected EntityStatSystem statSystem;
		[SerializeField] protected EntityAbilitySystem abilitySystem;
		[SerializeField] protected EntityAnimationController animationController;

		[Header("Gluttony")]
		[SerializeField] protected GluttonyConsumeSequence consumeSequencePrefab;
		[SerializeField] protected Transform gluttonyFXPositionTransform;
		[SerializeField] protected float gluttonyFXMaxSize;

		protected StateMachine _masterStateMachine = new StateMachine();

		protected void OnEnable()
		{
			ServiceLocator.For(this).Register(this);
		}

		protected void OnDisable()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		protected abstract void InitializeStateMachine();

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

		public void OnDeath()
		{
			Destroy(ServiceLocator.For(this).gameObject);
		}

		#endregion

		public void OnConsume(ConsumableEntity consumableEntity)
		{
			//Do things like stopping all the scripts and processing
			statSystem.enabled = false;
			healthManager.enabled = false;
			animationController.enabled = false;
			abilitySystem.enabled = false;
			this.enabled = false;
		}

		public async void OnConsume_Gluttony(ConsumableEntity consumableEntity)
		{
			renderers?.ForEach(meshRenderer => meshRenderer.enabled = false);
			GluttonyConsumeSequence sequence = new GluttonyConsumeSequence.Builder()
				.SetPosition(gluttonyFXPositionTransform.position)
				.SetMaxSize(gluttonyFXMaxSize)
				.Build(Instantiate(consumeSequencePrefab, ServiceLocator.ForSceneOf(this)?.Get<VFXHolder>()?.transform, true)).Execute();

			while (!sequence.EntityReadyToBeDestroyed)
				await Task.Delay(Random.Range(16, 33));

			//This is the last thing to happen
			ServiceLocator.ForSceneOf(this)?.Get<AbilityEvolveSystem>()?.ConsumeEntity(consumableEntity);
			Destroy(ServiceLocator.For(this).gameObject);
		}
	}
}