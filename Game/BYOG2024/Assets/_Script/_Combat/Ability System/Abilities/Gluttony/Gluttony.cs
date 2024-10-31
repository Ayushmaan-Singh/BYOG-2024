using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay;
using AstekUtility.Gameplay.Timer;
using Combat;
using Entity.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entity.Abilities
{
	public class Gluttony : AbilityBase
	{
		[Title("Gluttony")]
		[SerializeField] private Rigidbody gluttonyRb;
		[SerializeField] private GameObject shieldGameObject;
		[SerializeField] private Collider shieldCollider;

		[SerializeField, Range(0f, 1f)] private float consumeEntityBelowHealth = 0.3f;
		[SerializeField] private float parryWindowTime;

		[SerializeField] private CrossfadeAnimState gluttonyCrossFadeAnimState;
		[SerializeField] private CrossfadeAnimState nullAnimState;

		[Header("Sub Ability")]
		[SerializeField] private Beelzebub beelzebubAbility;
		[SerializeField] private int consumeEntityCountToUseBeelzebub;

		private CollisionLayerFilter includeLayerInCollision;
		private int _entitiesConsumed;
		private AbilityBase _activeAbility;

		private CountdownTimer _parryWindowTimer;

		private void Awake()
		{
			_activeAbility = this;
			shieldGameObject.SetActive(false);
			shieldCollider.enabled = false;
			includeLayerInCollision = shieldCollider.GetComponent<CollisionLayerFilter>();

			_parryWindowTimer = new CountdownTimer(parryWindowTime);
		}

		private void Start()
		{
			ServiceLocator.For(this).Get<EntityAnimationController>().RegisterAnimEvent("Gluttony", GluttonyAnimationEvent);
		}

		private void OnDestroy()
		{
			if (ServiceLocator.For(this)?.TryGetService(typeof(EntityAnimationController), out EntityAnimationController animController) is true)
				animController.DeregisterAnimEvent("Gluttony", GluttonyAnimationEvent);
		}

		private void Update()
		{
			_parryWindowTimer.Tick(Time.deltaTime);
			if (_entitiesConsumed >= consumeEntityCountToUseBeelzebub && _activeAbility.CurrentState != State.InProgress)
			{
				_activeAbility = beelzebubAbility;
				_entitiesConsumed = 0;
			}
		}

		public override void Execute()
		{
			//Beelzebub
			if (_activeAbility != this)
				_activeAbility.Execute();
			//Gluttony
			else
				ExecuteImplementation();
		}
		public override void CancelExecution()
		{
			//Beelzebub
			if (_activeAbility != this)
				_activeAbility.CancelExecution();
			//Gluttony
			else
				CancelExecutionImplementation();
		}

		private void ExecuteImplementation()
		{
			if (CurrentState != State.Usable)
				return;

			CurrentState = State.InProgress;
			//Run some animation with animation event to launch gluttony
			_parryWindowTimer.Start();
			gluttonyCrossFadeAnimState.Play(ServiceLocator.For(this).Get<EntityAnimationController>());
		}

		private void CancelExecutionImplementation()
		{
			CurrentState = State.Usable;
			//Here stop animation
			nullAnimState.Play(ServiceLocator.For(this).Get<EntityAnimationController>());
			_parryWindowTimer.Stop();
			shieldGameObject.SetActive(false);
			shieldCollider.enabled = false;
		}

		private void GluttonyAnimationEvent()
		{
			shieldGameObject.SetActive(true);
			Invoke(nameof(ActivateShieldCollider), 0.3f);
		}

		private void ActivateShieldCollider()
		{
			if (CurrentState != State.InProgress)
				return;
			_parryWindowTimer.Start();
			shieldCollider.enabled = true;
		}

		private bool CanParry()
		{
			return _parryWindowTimer.IsRunning;
		}

		public void OnCollisionEnter(Collision collision)
		{
			if (includeLayerInCollision && !includeLayerInCollision.CanCollide(collision.gameObject))
				return;

			//This is to parry
			if (CanParry())
			{
				IDamageable damageableEntity = collision.gameObject.GetComponentInParent<IDamageable>() ?? collision.gameObject.GetComponent<OwnerOfThisObject>().Owner.GetComponent<IDamageable>();
				float healthPercent = damageableEntity == null ? 0 : damageableEntity.CurrentHp / damageableEntity.MaxHp;

				if (healthPercent > consumeEntityBelowHealth)
					return;

				_entitiesConsumed++;
				(collision.gameObject.GetComponentInChildren<ConsumableEntity>() ?? collision.gameObject.GetComponent<OwnerOfThisObject>().Owner.GetComponent<ConsumableEntity>())?.Consume(this);
				return;
			}
			ServiceLocator.For(this).Get<PlayerMovement>().ApplyImpulseForce(-ServiceLocator.For(this).Get<PlayerMovement>().PlayerFacingInDirection, 1f);
		}
		
		public void OnCollisionEnter(GameObject collision)
		{
			//This is to parry
			if (CanParry())
			{
				IDamageable damageableEntity = collision.GetComponentInParent<IDamageable>() ?? collision.GetComponent<OwnerOfThisObject>().Owner.GetComponent<IDamageable>();
				float healthPercent = damageableEntity == null ? 0 : damageableEntity.CurrentHp / damageableEntity.MaxHp;

				if (healthPercent > consumeEntityBelowHealth)
					return;

				_entitiesConsumed++;
				(collision.GetComponentInChildren<ConsumableEntity>() ?? collision.GetComponent<OwnerOfThisObject>().Owner.GetComponent<ConsumableEntity>())?.Consume(this);
				return;
			}
			ServiceLocator.For(this).Get<PlayerMovement>().ApplyImpulseForce(-ServiceLocator.For(this).Get<PlayerMovement>().PlayerFacingInDirection, 1f);
		}

		private void OnParticleCollision(GameObject other)
		{
			if (CanParry()) { }
		}
	}
}