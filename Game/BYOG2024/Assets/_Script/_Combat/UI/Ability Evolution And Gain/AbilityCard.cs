using System;
using Entity.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Combat.UI
{
	public class AbilityCard : MonoBehaviour
	{
		[SerializeField] private Image icon;
		[SerializeField] private TextMeshProUGUI abilityDescription;


		public AbilityBase BaseFormOfAbility { get; private set; }
		public AbilityBase ReferencedAbility { get; private set; }

		public void Reset()
		{
			icon.sprite = null;
			abilityDescription.text = "";
			ReferencedAbility = null;
		}

		public class Builder
		{
			private AbilityBase _baseType;
			private AbilityBase _abilityBase;
			private AbilityBase _readDescriptionFrom;

			public Builder SetBaseTypeOfThisAbility(AbilityBase baseType)
			{
				_baseType = baseType;
				return this;
			}

			public Builder SetReferencedAbility(AbilityBase ability)
			{
				_abilityBase = ability;
				return this;
			}

			public Builder SetDescriptionFromAbility(AbilityBase abilityBase)
			{
				_readDescriptionFrom = abilityBase;
				return this;
			}

			public AbilityCard Build(AbilityCard card)
			{
				card.icon.sprite = (_abilityBase??_readDescriptionFrom).AbilityTileImage;
				card.abilityDescription.text = (_abilityBase??_readDescriptionFrom).AbilityDescription;
				card.BaseFormOfAbility = _baseType ?? null;
				card.ReferencedAbility = _abilityBase ?? null;

				return card;
			}
		}
	}
}