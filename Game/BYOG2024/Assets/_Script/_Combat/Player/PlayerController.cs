using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay;
using Combat.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
	[RequireComponent(
		 typeof(EntityStatSystem),
		 typeof(EntityHealthManager),
		 typeof(PlayerAbilitySystem))
	 , RequireComponent(typeof(PlayerMovement))]
	public class PlayerController : MonoBehaviour, IDamageable
	{
		[Header("Input System")]
		[SerializeField] private InputActionReference movementInput;
		[SerializeField] private InputActionReference attackInput;
		[SerializeField] private InputActionReference changeActiveAttackInput;
		[SerializeField] private InputActionReference gluttonyAbility;
		
		private ServiceLocator _serviceLocator;

		private void Awake()
		{
			_serviceLocator = ServiceLocator.For(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		private void OnEnable()
		{
			movementInput.action.performed += MovementPerformed;
			movementInput.action.canceled += MovementCancelled;
			attackInput.action.performed += Attack;
			attackInput.action.canceled += CancelAttack;
			changeActiveAttackInput.action.performed += CurrentAbilityChanged;
			gluttonyAbility.action.performed += GluttonyPerformed;
			gluttonyAbility.action.canceled += GluttonyCancelled;
		}

		private void OnDisable()
		{
			movementInput.action.performed -= MovementPerformed;
			movementInput.action.canceled -= MovementCancelled;
			attackInput.action.performed -= Attack;
			attackInput.action.canceled -= CancelAttack;
			changeActiveAttackInput.action.performed -= CurrentAbilityChanged;
			changeActiveAttackInput.action.performed -= GluttonyPerformed;
			gluttonyAbility.action.canceled -= GluttonyCancelled;
		}


		//Movement
		private void MovementPerformed(InputAction.CallbackContext ctx) => _serviceLocator.Get<PlayerMovement>().Movement(ctx.ReadValue<Vector2>());
		private void MovementCancelled(InputAction.CallbackContext ctx) => _serviceLocator.Get<PlayerMovement>().Movement(Vector2.zero);

		//Attack
		private void Attack(InputAction.CallbackContext ctx) =>
			(_serviceLocator.Get<EntityAbilitySystem>() as PlayerAbilitySystem)?.Attack();

		private void CancelAttack(InputAction.CallbackContext ctx) =>
			(_serviceLocator.Get<EntityAbilitySystem>() as PlayerAbilitySystem)?.CancelAttack();


		//Ability
		private void CurrentAbilityChanged(InputAction.CallbackContext ctx) =>
			(_serviceLocator.Get<EntityAbilitySystem>() as PlayerAbilitySystem)?.SwitchActiveAbility(Math.Clamp(ctx.ReadValue<float>(), -1f, 1f));


		private void GluttonyPerformed(InputAction.CallbackContext ctx) =>
			(_serviceLocator.Get<EntityAbilitySystem>() as PlayerAbilitySystem)?.StartChannelingGluttony();


		private void GluttonyCancelled(InputAction.CallbackContext ctx) =>
			(_serviceLocator.Get<EntityAbilitySystem>() as PlayerAbilitySystem)?.StopChannelingGluttony();

		#region Health Management

		public float MaxHp => ServiceLocator.For(this).Get<EntityStatSystem>()?.GetInstanceStats(Stats.Hp) ?? 0;
		public float CurrentHp => ServiceLocator.For(this).Get<EntityHealthManager>()?.CurrentHP ?? 0;
		
		public void Damage(float amount)
		{
			ServiceLocator.For(this).Get<EntityHealthManager>()?.Damage(amount);
			ServiceLocator.ForSceneOf(this).Get<PlayerMediator>().healthSubject.Notify((MaxHp,CurrentHp));
		}
		public void Heal(float amount)
		{
			ServiceLocator.For(this).Get<EntityHealthManager>()?.Heal(amount);
			ServiceLocator.ForSceneOf(this).Get<PlayerMediator>().healthSubject.Notify((MaxHp,CurrentHp));
		}

		#endregion
	}
}