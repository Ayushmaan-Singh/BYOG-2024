using AstekUtility.DesignPattern.ServiceLocatorTool;
using Entity.Abilities;
using UnityEngine;

namespace Entity.Player
{
	public class PlayerAbilitySystem : EntityAbilitySystem
	{
		[Header("Player Ability System")]
		[SerializeField] private Gluttony gluttonyAbility;
		private AbilityBase _activeAbility;
		private int _activeAbilityIndex = 0;

		public AbilityBase GetActiveAbility => _activeAbility;

		protected new void Awake()
		{
			ServiceLocator.For(this).Register(this);
			_activeAbility = abilitySystem[_activeAbilityIndex];
		}
		protected new void OnDestroy()
		{
			ServiceLocator.For(this)?.Register(this);
		}

		public void Attack() => _activeAbility.Execute();
		public void CancelAttack() => _activeAbility.CancelExecution();
		
		public void SwitchActiveAbility(int dir)
		{
			if (_activeAbilityIndex + Mathf.Clamp(dir, -1, 1) >= abilitySystem.Count)
				_activeAbilityIndex = 0;
			else if (_activeAbilityIndex + Mathf.Clamp(dir, -1, 1) <= 0)
				_activeAbilityIndex = abilitySystem.Count - 1;

			_activeAbility = abilitySystem[_activeAbilityIndex];
		}

		public void StartChannelingGluttony() => gluttonyAbility.Execute();

		public void StopChannelingGluttony() => gluttonyAbility.CancelExecution();
	}
}