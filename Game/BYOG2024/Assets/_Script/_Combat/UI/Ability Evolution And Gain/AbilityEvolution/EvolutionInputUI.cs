using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using AstekUtility.Input;
using Entity.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Combat.UI
{
	public class EvolutionInputUI : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField] private Image progressImage;

		[Header("Input System")]
		[SerializeField] private float holdTime;
		[SerializeField] private InputActionReference leftClickInput;

		private bool _isLeftClickPressed;
		private CountdownTimer _holdTimer;

		private void Awake()
		{
			_holdTimer = new CountdownTimer(holdTime)
			{
				OnTimerStop = () =>
				{
					if (!_holdTimer.IsFinished)
						return;
					ServiceLocator.ForSceneOf(this).Get<PlayerMediator>().ChangeAbility(ServiceLocator.For(this).Get<EvolutionUI>().SelectedAbilityBaseForm,
						ServiceLocator.For(this).Get<EvolutionUI>().SelectedAbilityEvolvedForm);
					ServiceLocator.For(this).Get<EvolutionUI>().Deactivate();
				}
			};
		}

		private void Update()
		{
			if (!gameObject.activeInHierarchy)
				return;

			_holdTimer.Tick(Time.unscaledDeltaTime);

			if (!_holdTimer.IsRunning)
				transform.position = Mouse.current.position.ReadValue();

			if (_isLeftClickPressed)
				progressImage.fillAmount = _holdTimer.Progress;
		}

		private void OnEnable()
		{
			leftClickInput.action.performed += OnLeftClick_Performed;
			leftClickInput.action.canceled += OnLeftClick_Canceled;
		}

		private void OnDisable()
		{
			leftClickInput.action.performed -= OnLeftClick_Performed;
			leftClickInput.action.canceled -= OnLeftClick_Canceled;
			
			_holdTimer.Stop();
			_holdTimer.Reset();
			progressImage.fillAmount = 0;
		}

		private void OnLeftClick_Performed(InputAction.CallbackContext ctx)
		{
			_isLeftClickPressed = true;
			_holdTimer.Start();
		}

		private void OnLeftClick_Canceled(InputAction.CallbackContext ctx)
		{
			_isLeftClickPressed = false;
			_holdTimer.Stop();
			progressImage.fillAmount = 0;
		}
	}
}