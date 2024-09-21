using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
	[RequireComponent(
		 typeof(EntityStatSystem),
		 typeof(EntityHealthManager),
		 typeof(PlayerAbilitySystem))
	 , RequireComponent(typeof(PlayerMovement))]
	public class PlayerController : MonoBehaviour
	{
		private PlayerInput _playerInput;
		private ServiceLocator _serviceLocator;

		private void Awake()
		{
			_serviceLocator = ServiceLocator.For(this).Register(this);
			_playerInput = new PlayerInput();
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		private void OnEnable()
		{
			_playerInput.Enable();

			_playerInput.BasicAction.Movement.performed += MovementPerformed;
			_playerInput.BasicAction.Movement.canceled += MovementCancelled;
			_playerInput.BasicAction.Attack.performed += Attack;
			_playerInput.BasicAction.Attack.canceled += CancelAttack;
			_playerInput.BasicAction.ChangeAttack.performed += CurrentAbilityChanged;
			_playerInput.BasicAction.Gluttony.performed += GluttonyPerformed;
			_playerInput.BasicAction.Gluttony.canceled += GluttonyCancelled;
		}

		private void OnDisable()
		{
			_playerInput.Disable();

			_playerInput.BasicAction.Movement.performed -= MovementPerformed;
			_playerInput.BasicAction.Movement.canceled -= MovementCancelled;
			_playerInput.BasicAction.Attack.performed -= Attack;
			_playerInput.BasicAction.Attack.canceled -= CancelAttack;
			_playerInput.BasicAction.ChangeAttack.performed -= CurrentAbilityChanged;
			_playerInput.BasicAction.Gluttony.performed -= GluttonyPerformed;
			_playerInput.BasicAction.Gluttony.canceled -= GluttonyCancelled;
		}


		//Movement
		private void MovementPerformed(InputAction.CallbackContext ctx) => _serviceLocator.Get<PlayerMovement>().Movement(ctx.ReadValue<Vector2>());
		private void MovementCancelled(InputAction.CallbackContext ctx) => _serviceLocator.Get<PlayerMovement>().Movement(Vector2.zero);

		//Attack
		private void Attack(InputAction.CallbackContext ctx) =>
			_serviceLocator.Get<PlayerAbilitySystem>().Attack();

		private void CancelAttack(InputAction.CallbackContext ctx) =>
			_serviceLocator.Get<PlayerAbilitySystem>().CancelAttack();


		//Ability
		private void CurrentAbilityChanged(InputAction.CallbackContext ctx) =>
			_serviceLocator.Get<PlayerAbilitySystem>().SwitchActiveAbility(Math.Clamp(ctx.ReadValue<float>(), -1f, 1f));


		private void GluttonyPerformed(InputAction.CallbackContext ctx) =>
			_serviceLocator.Get<PlayerAbilitySystem>().StartChannelingGluttony();


		private void GluttonyCancelled(InputAction.CallbackContext ctx) =>
			_serviceLocator.Get<PlayerAbilitySystem>().StopChannelingGluttony();

	}
}