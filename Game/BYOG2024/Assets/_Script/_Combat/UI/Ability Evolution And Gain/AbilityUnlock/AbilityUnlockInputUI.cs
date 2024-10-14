using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using Entity.Player;
using Global;
using Global.UI;
using Sirenix.OdinInspector;

namespace Combat.UI
{
	public class AbilityUnlockInputUI : MonoBehaviour
	{
		[Header("Visual FX")]
		[SerializeField] private UIFadeInFX rightFadeIn;
		[SerializeField] private UIFadeOutFX rightFadeOut;

		[SerializeField] private UIFadeInFX leftFadeIn;
		[SerializeField] private UIFadeOutFX leftFadeOut;

		[Header("Left Click Mouse Input")]
		[SerializeField] private GameObject leftClick;
		[SerializeField] private Image leftClickProgress;

		[Header("Right Click Mouse Input")]
		[SerializeField] private GameObject rightClick;
		[SerializeField] private Image rightClickProgress;

		[Header("Input System")]
		[SerializeField] private float holdTime;
		[SerializeField] private InputActionReference rightClickInput;
		[SerializeField] private InputActionReference leftClickInput;

		private bool _rightClickPressed;
		private bool _leftClickPressed;

		private float _leftDefaultUIPosX;
		private float _rightDefaultUIPosX;
		private CoroutineTask _onPress;
		private CountdownTimer _holdTimer;

		private void Awake()
		{
			_leftDefaultUIPosX = leftClick.GetComponent<RectTransform>().anchoredPosition.x;
			_rightDefaultUIPosX = rightClick.GetComponent<RectTransform>().anchoredPosition.x;
			_holdTimer = new CountdownTimer(holdTime)
			{
				OnTimerStop = () =>
				{
					if (!_holdTimer.IsFinished)
						return;
					ServiceLocator.ForSceneOf(this).Get<PlayerMediator>().ChangeAbility(_rightClickPressed ? 1 : 0, ServiceLocator.For(this).Get<AbilityUnlockableUI>().AbilitySelected);
					ServiceLocator.For(this).Get<AbilityUnlockableUI>().Deactivate();
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

			if (_rightClickPressed)
			{
				rightClickProgress.fillAmount = _holdTimer.Progress;
			}
			else if (_leftClickPressed)
			{
				leftClickProgress.fillAmount = _holdTimer.Progress;
			}
		}

		private void OnEnable()
		{
			rightClickInput.action.performed += RightClickPressed;
			rightClickInput.action.canceled += RightClickReleased;

			leftClickInput.action.performed += LeftClickPressed;
			leftClickInput.action.canceled += LeftClickReleased;
		}
		private void OnDisable()
		{
			rightClickInput.action.performed -= RightClickPressed;
			rightClickInput.action.canceled -= RightClickReleased;

			leftClickInput.action.performed -= LeftClickPressed;
			leftClickInput.action.canceled -= LeftClickReleased;

			//This thing prevent unknown behavior if the object is disabled while coroutines were in process
			CompleteReset();
		}

		private void RightClickPressed(InputAction.CallbackContext ctx)
		{
			if (_leftClickPressed)
				return;

			_rightClickPressed = true;
			_holdTimer.Start();
			StartCoroutine(ClickPressedAnimSequence(leftFadeOut, leftFadeIn));
		}
		private void RightClickReleased(InputAction.CallbackContext ctx)
		{
			if (_rightClickPressed)
			{
				ResetRightClick();
				StartCoroutine(ClickReleasedAnimSequence(leftFadeOut, leftFadeIn));
			}
			_rightClickPressed = false;
		}

		private void LeftClickPressed(InputAction.CallbackContext ctx)
		{
			if (_rightClickPressed)
				return;

			_leftClickPressed = true;
			_holdTimer.Start();
			StartCoroutine(ClickPressedAnimSequence(rightFadeOut, rightFadeIn));
		}
		private void LeftClickReleased(InputAction.CallbackContext ctx)
		{
			if (_leftClickPressed)
			{
				ResetLeftClick();
				StartCoroutine(ClickReleasedAnimSequence(rightFadeOut, rightFadeIn));
			}
			_leftClickPressed = false;
		}


		private IEnumerator ClickPressedAnimSequence(UIFadeOutFX fadeOut, UIFadeInFX fadeIn)
		{
			yield return new WaitWhile(() => fadeIn.Progress < 1);

			fadeOut.Play();

			yield return new WaitWhile(() =>
			{
				leftClick.GetComponent<RectTransform>().anchoredPosition =
					leftClick.GetComponent<RectTransform>().anchoredPosition.With(x:Mathf.Lerp(_leftDefaultUIPosX, 0, fadeOut.Progress));

				rightClick.GetComponent<RectTransform>().anchoredPosition =
					rightClick.GetComponent<RectTransform>().anchoredPosition.With(x:Mathf.Lerp(_rightDefaultUIPosX, 0, fadeOut.Progress));

				return fadeOut.Progress < 1;
			});

			leftClick.GetComponent<RectTransform>().anchoredPosition =
				leftClick.GetComponent<RectTransform>().anchoredPosition.With(x:0);
			rightClick.GetComponent<RectTransform>().anchoredPosition =
				rightClick.GetComponent<RectTransform>().anchoredPosition.With(x:0);
		}

		private IEnumerator ClickReleasedAnimSequence(UIFadeOutFX fadeOut, UIFadeInFX fadeIn)
		{
			yield return new WaitWhile(() => fadeOut.Progress < 1);
			fadeIn.Play();

			yield return new WaitWhile(() =>
			{
				leftClick.GetComponent<RectTransform>().anchoredPosition =
					leftClick.GetComponent<RectTransform>().anchoredPosition.With(x:Mathf.Lerp(0, _leftDefaultUIPosX, fadeIn.Progress));

				rightClick.GetComponent<RectTransform>().anchoredPosition =
					rightClick.GetComponent<RectTransform>().anchoredPosition.With(x:Mathf.Lerp(0, _rightDefaultUIPosX, fadeIn.Progress));

				return fadeIn.Progress < 1;
			});

			leftClick.GetComponent<RectTransform>().anchoredPosition =
				leftClick.GetComponent<RectTransform>().anchoredPosition.With(x:_leftDefaultUIPosX);
			rightClick.GetComponent<RectTransform>().anchoredPosition =
				rightClick.GetComponent<RectTransform>().anchoredPosition.With(x:_rightDefaultUIPosX);
		}

		private void CompleteReset()
		{
			StopAllCoroutines();

			_leftClickPressed = false;
			_rightClickPressed = false;

			leftClick.GetComponent<RectTransform>().anchoredPosition =
				leftClick.GetComponent<RectTransform>().anchoredPosition.With(x:_leftDefaultUIPosX);
			rightClick.GetComponent<RectTransform>().anchoredPosition =
				rightClick.GetComponent<RectTransform>().anchoredPosition.With(x:_rightDefaultUIPosX);

			leftFadeIn.Stop();
			leftFadeOut.Stop();
			leftFadeOut.ResetFade();


			rightFadeIn.Stop();
			rightFadeOut.Stop();
			rightFadeOut.ResetFade();
			ResetLeftClick();
			ResetRightClick();
		}

		private void ResetLeftClick()
		{
			_holdTimer.Stop();
			_holdTimer.Reset();
			leftClickProgress.fillAmount = 0;
		}

		private void ResetRightClick()
		{
			_holdTimer.Stop();
			_holdTimer.Reset();
			rightClickProgress.fillAmount = 0;
		}
	}
}