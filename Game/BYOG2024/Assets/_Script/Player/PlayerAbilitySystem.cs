using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Entity.Abilities;
using UnityEngine;

namespace Entity.Player
{
	public class PlayerAbilitySystem : EntityAbilitySystem
	{
		[Header("Player Ability System")]
		[SerializeField] private Gluttony gluttonyAbility;
		[SerializeField] private List<AbilityBase> abilitiyCollection;
		private AbilityBase _activeAbility;
		private int _activeAbilityIndex = 0;

		public AbilityBase GetActiveAbility => _activeAbility;
		public List<AbilityBase> AbilityOwned => abilitySystem;

		protected new void Awake()
		{
			ServiceLocator.For(this).Register(this);
			_activeAbility = abilitySystem[_activeAbilityIndex];
		}
		protected new void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		public void Attack() => _activeAbility.Execute();
		public void CancelAttack() => _activeAbility.CancelExecution();

		public void SwitchActiveAbility(float dir)
		{
			if (_activeAbilityIndex + Mathf.Clamp(dir, -1, 1) >= abilitySystem.Count)
				_activeAbilityIndex = 0;
			else if (_activeAbilityIndex + Mathf.Clamp(dir, -1, 1) <= 0)
				_activeAbilityIndex = abilitySystem.Count - 1;

			_activeAbility = abilitySystem[_activeAbilityIndex];
		}

		public void StartChannelingGluttony() => gluttonyAbility.Execute();

		public void StopChannelingGluttony() => gluttonyAbility.CancelExecution();

		public void SwitchUsableAbility(AbilityBase insteadOf,AbilityBase abilityToSwitch)
		{
			foreach (AbilityBase ability in abilitiyCollection)
			{
				if (ability.GetType() == abilityToSwitch.GetType())
				{
					//TODO remove this magic number
					if (abilitySystem[0].GetType() == insteadOf.GetType())
					{
						if (_activeAbility == abilitySystem[0])
							_activeAbility = ability;
						abilitySystem[0] = ability;
					}
					else
					{
						if (_activeAbility == abilitySystem[1])
							_activeAbility = ability;
						abilitySystem[1] = ability;
					}
				}
			}
		}
	}
}