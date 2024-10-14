using System;
using System.Collections.Generic;
using System.Linq;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Odin.Utility;
using Combat.AbilityEvolution;
using Entity.Abilities;
using Global;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Combat.UI
{
	public class AbilityUnlockableUI : MonoBehaviour
	{
		[SerializeField, Tooltip("This hold the functional part of UI,used to activate or deactivate panel")]
		private GameObject panel;
		[SerializeField] private GameObject abilityInputUnlockUI;
		[SerializeField] private AbilityUnlockPanelUI[] unlockableAbilityPanelUI;
		[SerializeField,InlineProperty] private UnityTag abilityTileTag;

		[Header("Input System")]
		[SerializeField, ValueDropdown("@Global.InputManager.ActionMapsName")]
		private string actionMap;
		[SerializeField] private GraphicRaycaster raycaster;

		private PointerEventData _clickData;
		private List<RaycastResult> _raycastResults = new List<RaycastResult>();

		public AbilityBase AbilitySelected { get; private set; }

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
			_clickData = new PointerEventData(EventSystem.current);
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		private void Update()
		{
			_raycastResults.Clear();
			_clickData.position = Mouse.current.position.ReadValue();
			EventSystem.current.RaycastAll(_clickData, _raycastResults);
			if (_raycastResults.Count > 0)
			{
				AbilityCard abilityCard = null;
				int count = _raycastResults.Count;
				for (int i = 0; i < count; i++)
				{
					AbilityCard card = _raycastResults[i].gameObject.GetComponent<AbilityCard>();
					if (card && !card.BaseFormOfAbility && card.ReferencedAbility)
					{
						abilityCard = card;
						break;
					}
				}
				if (abilityCard)
				{
					abilityInputUnlockUI.SetActive(true);
					AbilitySelected = abilityCard.GetComponent<AbilityCard>().ReferencedAbility;
				}
				else
				{
					abilityInputUnlockUI.SetActive(false);
				}
			}
		}

		public void Activate()
		{
			panel.SetActive(true);
			Time.timeScale = 0;
			ServiceLocator.Global.Get<InputManager>().SwitchActiveActionMap(actionMap);
		}
		public void Deactivate()
		{
			Time.timeScale = 1;
			ServiceLocator.Global.Get<InputManager>().SwitchActiveActionMapToPrev();
			abilityInputUnlockUI.SetActive(false);
			panel.SetActive(false);
		}

		public void UnlockableAbility(AbilityTreeNode[] unlockableAbility)
		{
			int count = Math.Clamp(unlockableAbilityPanelUI.Length, 0, 6);

			//This part splits the unlockableAbility array into arrays of max size 3 
			IEnumerable<AbilityTreeNode[]> result = unlockableAbility
				.Select((value, index) => new
				{
					value,
					index
				})
				.GroupBy(x => x.index / 3)
				.Select(g => g.Select(x => x.value).ToArray());

			int i = 0;
			foreach (AbilityTreeNode[] nodeCollection in result)
			{
				if (i >= 0 && i < unlockableAbilityPanelUI.Length)
				{
					unlockableAbilityPanelUI[i].gameObject.SetActive(true);
					new AbilityUnlockPanelUI.Builder().SetUnlockableAbility(nodeCollection).Build(unlockableAbilityPanelUI[i]);
				}
				else
					break;
				i++;
			}
		}
		public void Reset()
		{
			foreach (AbilityUnlockPanelUI unlockPanelUI in unlockableAbilityPanelUI)
			{
				unlockPanelUI.Reset();
				unlockPanelUI.gameObject.SetActive(false);
			}
		}
	}
}