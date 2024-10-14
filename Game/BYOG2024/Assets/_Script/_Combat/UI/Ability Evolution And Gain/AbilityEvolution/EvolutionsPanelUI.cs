using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using AstekUtility;
using Combat.AbilityEvolution;
using Entity.Abilities;
using Global;
using UnityEngine;

namespace Combat.UI
{
	public class EvolutionsPanelUI : MonoBehaviour
	{
		[SerializeField] private AbilityCard toEvolveAbilityCard;
		[SerializeField] private Transform evolutionHolder;
		[SerializeField] private AbilityCard abilityCardPrefab;
		[SerializeField] private PrefabCollection abilityPrefabCollection;
		

		public void Reset()
		{
			toEvolveAbilityCard.Reset();
			evolutionHolder.DestroyChildren();
		}
		
		public class Builder
		{
			private AbilityBase _toEvolveAbility;
			private AbilityTreeNode[] _availableEvolutions;

			public Builder SetAbilityToEvolve(AbilityBase ability)
			{
				_toEvolveAbility = ability;
				return this;
			}

			public Builder SetAvailableEvolutions(AbilityTreeNode[] availableEvolutions)
			{
				_availableEvolutions = availableEvolutions;
				return this;
			}

			public EvolutionsPanelUI Build(EvolutionsPanelUI panelUI)
			{
				new AbilityCard.Builder().SetDescriptionFromAbility(_toEvolveAbility).Build(panelUI.toEvolveAbilityCard);

				foreach (AbilityTreeNode abilityNode in _availableEvolutions)
				{
					AbilityCard card = Instantiate(panelUI.abilityCardPrefab,panelUI.evolutionHolder);
					new AbilityCard.Builder()
						.SetBaseTypeOfThisAbility(_toEvolveAbility)
						.SetReferencedAbility(panelUI.abilityPrefabCollection[abilityNode.AbilityBaseType]
							.GetComponentInChildren<AbilityBase>())
						.Build(card);
				}
				
				return panelUI;
			}
		}
	}
}