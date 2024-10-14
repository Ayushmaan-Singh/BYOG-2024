using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using Combat.AbilityEvolution;
using Entity.Abilities;
using Global;
using UnityEngine;

namespace Combat.UI
{
	public class AbilityUnlockPanelUI : MonoBehaviour
	{
		[SerializeField] private Transform evolutionHolder;
		[SerializeField] private AbilityCard abilityCardPrefab;		
		[SerializeField] private PrefabCollection abilityPrefabCollection;


		public void Reset()
		{
			evolutionHolder.DestroyChildren();
		}
		
		public class Builder
		{
			private AbilityTreeNode[] _unlockableAbilities;

			public Builder SetUnlockableAbility(AbilityTreeNode[] unlockableAbility)
			{
				_unlockableAbilities = unlockableAbility;
				return this;
			}

			public AbilityUnlockPanelUI Build(AbilityUnlockPanelUI panelUI)
			{
				foreach (AbilityTreeNode abilityNode in _unlockableAbilities)
				{
					AbilityCard card = Instantiate(panelUI.abilityCardPrefab,panelUI.evolutionHolder);
					new AbilityCard.Builder()
						.SetReferencedAbility(panelUI.abilityPrefabCollection[abilityNode.AbilityBaseType]
							.GetComponentInChildren<AbilityBase>())
						.Build(card);
				}
				
				return panelUI;
			}
		}
	}
}
