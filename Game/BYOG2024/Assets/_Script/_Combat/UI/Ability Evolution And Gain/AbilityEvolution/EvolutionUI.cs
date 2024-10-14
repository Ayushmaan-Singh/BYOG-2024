using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Odin.Utility;
using Combat.AbilityEvolution;
using Entity.Abilities;
using Global;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Combat.UI
{
	public class EvolutionUI : MonoBehaviour
	{
		[SerializeField, Tooltip("This hold the functional part of UI,used to activate or deactivate panel")]
		private GameObject panel;
		[SerializeField] private EvolutionsPanelUI[] availableEvolutionPanelUI;
		[SerializeField] private UnityTag abilityTileTag;

		[Header("Input")]
		[SerializeField,ValueDropdown("@Global.InputManager.ActionMapsName")] private string actionMap;
		[SerializeField] private EvolutionInputUI evolutionInputUI;

		private PointerEventData _clickData;
		private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

		public AbilityBase SelectedAbilityBaseForm { get; private set; }
		public AbilityBase SelectedAbilityEvolvedForm { get; private set; }

		public void Activate()
		{
			panel.SetActive(true);
			Time.timeScale = 0;
			ServiceLocator.Global.Get<InputManager>().SwitchActiveActionMap(actionMap);
		}
		public void Deactivate()
		{
			panel.SetActive(false);
			Time.timeScale = 1;
			ServiceLocator.Global.Get<InputManager>().SwitchActiveActionMapToPrev();

		}
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
			if (_raycastResults.Count <= 0)
				return;

			AbilityCard abilityCard = null;
			int count = _raycastResults.Count;
			for (int i = 0; i < count; i++)
			{
				AbilityCard card = _raycastResults[i].gameObject.GetComponent<AbilityCard>();
				if (!card.OrNull() || !card.BaseFormOfAbility || !card.ReferencedAbility)
					continue;
				abilityCard = card;
				break;
			}
			if (abilityCard)
			{
				evolutionInputUI.gameObject.SetActive(true);
				SelectedAbilityBaseForm = abilityCard.BaseFormOfAbility;
				SelectedAbilityEvolvedForm = abilityCard.ReferencedAbility;
			}
			else
			{
				evolutionInputUI.gameObject.SetActive(false);
			}
		}

		public void EvolutionAbility(Dictionary<AbilityBase, AbilityTreeNode[]> availableEvolution)
		{
			int i = 0;
			foreach (AbilityBase toEvolveAbility in availableEvolution.Keys)
			{
				if (i >= 0 && i < availableEvolutionPanelUI.Length)
				{
					availableEvolutionPanelUI[i].gameObject.SetActive(true);
					new EvolutionsPanelUI.Builder()
						.SetAbilityToEvolve(toEvolveAbility)
						.SetAvailableEvolutions(availableEvolution[toEvolveAbility])
						.Build(availableEvolutionPanelUI[i]);
				}
				else
					break;
				i++;
			}
		}
		public void Reset()
		{
			foreach (EvolutionsPanelUI evolutionPanel in availableEvolutionPanelUI)
			{
				evolutionPanel.Reset();
				evolutionPanel.gameObject.SetActive(false);
			}
		}
	}
}