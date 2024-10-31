using System;
using System.Collections;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Observer.Unmanaged;
using Combat.Player;
using Entity.Abilities;
using Entity.Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.UI
{
	using AbilityChangedObserver = AstekUtility.Observer.Unmanaged.IObserver<(int index, AbilityBase changeTo)>;
	using AbilityStateChangedObserver = AstekUtility.Observer.Unmanaged.IObserver<(int index, AbilityBase.State state)>;
	using AbilityProgressionChangedObserver = AstekUtility.Observer.Unmanaged.IObserver<(int index, float amount)>;
	using ActiveAbilitySwitchedObserver = AstekUtility.Observer.Unmanaged.IObserver<int>;

	public class AbilityTile : MonoBehaviour, AbilityChangedObserver, AbilityStateChangedObserver, AbilityProgressionChangedObserver, ActiveAbilitySwitchedObserver
	{
		[SerializeField] private Image iconImage;
		[SerializeField] private Image cooldownImage;

		[Header("UI Animation")]
		[SerializeField, Range(0f, 1f)] private float stepSize;
		[SerializeField] private float timePeriod;
		[SerializeField] private Vector2 initialSize;
		[SerializeField] private Vector2 finalSize;

		private RectTransform _rectTransform;

		private AbilityBase.State _currentState;
		private bool _isActiveAbility = false;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			enabled = false;
		}

		private void OnDisable()
		{
			Reset();
		}

		//AbilityChangedObserver
		public void OnNotify(ISubject<(int index, AbilityBase changeTo)> subject)
		{
			if (ServiceLocator.For(this).Get<AbilityHUDManager>().GetTileOrder(this) != subject.Data.index)
				return;

			iconImage.sprite = subject.Data.changeTo.AbilityIcon;
		}
		//AbilityStateChangedObserver
		public void OnNotify(ISubject<(int index, AbilityBase.State state)> subject)
		{
			if (ServiceLocator.For(this).Get<AbilityHUDManager>().GetTileOrder(this) != subject.Data.index)
				return;

			cooldownImage.gameObject.SetActive(subject.Data.state != AbilityBase.State.Usable);
			_currentState = subject.Data.state;
		}
		//AbilityProgressionChangedObserver
		public void OnNotify(ISubject<(int index, float amount)> subject)
		{
			if (ServiceLocator.For(this).Get<AbilityHUDManager>().GetTileOrder(this) != subject.Data.index)
				return;

			if (_currentState == AbilityBase.State.Usable)
				cooldownImage.fillAmount = 0;
			else
				//because progression send in increasing value from other script that from 0 to 1 while we work here with 1 to 0
				cooldownImage.fillAmount = 1 - subject.Data.amount;
		}
		//ActiveAbilitySwitchedObserver
		public void OnNotify(ISubject<int> subject)
		{
			int tileOrder = ServiceLocator.For(this).Get<AbilityHUDManager>().GetTileOrder(this);

			if (tileOrder != subject.Data && _isActiveAbility)
			{
				_isActiveAbility = false;
				_rectTransform.localScale = initialSize;
			}

			if (tileOrder == subject.Data && !_isActiveAbility)
			{
				_isActiveAbility = true;
				_rectTransform.localScale = finalSize;
			}
		}

		private void ScaleUIObject(Vector3 size)
		{
			(_rectTransform ?? GetComponent<RectTransform>()).localScale = size;
		}

		//Reset class on disable
		private void Reset()
		{
			_rectTransform.sizeDelta = initialSize;
		}

		public class Builder
		{
			private int _activeAbilityIndex;

			public Builder SetActiveAbilityIndex(int index)
			{
				_activeAbilityIndex = index;
				return this;
			}

			public AbilityTile Build(AbilityTile instance)
			{
				if (ServiceLocator.For(instance).Get<AbilityHUDManager>().GetTileOrder(instance) == _activeAbilityIndex)
				{
					instance._isActiveAbility = true;
					instance.ScaleUIObject(instance.finalSize);
				}
				instance.enabled = true;
				return instance;
			}
		}
	}
}