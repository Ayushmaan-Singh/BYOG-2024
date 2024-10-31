using System;
using System.Linq;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Input;
using Entity.Abilities;
using UnityEngine;

namespace Entity.Player
{
	public class PlayerAbilitySystem : EntityAbilitySystem
	{
		[Header("Player Ability System")]
		[SerializeField] private Gluttony gluttonyAbility;
		public AbilityBase[] AbilityOwned => abilities;

		private PlayerMediator _playerMediator;

		protected void Awake()
		{
			_activeAbility = abilities[_activeAbilityIndex];
		}

		private void Start()
		{
			_playerMediator = ServiceLocator.For(this).Get<PlayerMediator>();
			_playerMediator.ActiveAbilitySwitchedSubject.Notify(0);
		}

		private void Update()
		{
			AimAt = ServiceLocator.Global.Get<InputUtils.MousePosition>()?.Invoke() ?? Vector3.zero;
			int count = abilities.Length;
			for (int i = 0; i < count; i++)
			{
				_playerMediator.AbilityStateChangedSubject.Notify((i, abilities[i].CurrentState));
				_playerMediator.AbilityProgressChangedSubject.Notify((i, abilities[i].Progress));
			}
		}

		public void Attack() => _activeAbility.Execute();
		public void CancelAttack() => _activeAbility.CancelExecution();

		public void SwitchActiveAbility(float dir)
		{
			_activeAbility = abilities[_activeAbilityIndex = Mathf.Abs((dir > 0 ? _activeAbilityIndex + 1 : _activeAbilityIndex - 1) % abilities.Length)];
			ServiceLocator.For(this).Get<PlayerMediator>().ActiveAbilitySwitchedSubject.Notify(_activeAbilityIndex);
		}

		public void StartChannelingGluttony()
		{
			_activeAbility = gluttonyAbility;
			gluttonyAbility.Execute();
		}

		public void StopChannelingGluttony()
		{
			_activeAbility = abilities[_activeAbilityIndex];
			gluttonyAbility.CancelExecution();
		}

		/// <summary>
		/// Used by evolution
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public bool TryChangeAbility(AbilityBase from, AbilityBase to) => TryChangeAbility(AbilityOfTypeAtIndex(from), to);

		/// <summary>
		/// Used by ability unlock
		/// </summary>
		/// <param name="index"></param>
		/// <param name="to"></param>
		public bool TryChangeAbility(int index, AbilityBase to)
		{
			if (index > abilities.Length)
			{
				Debug.LogError($"index:{index} passed is greater than length of collection i.e {abilities.Length}");
				return false;
			}

			Destroy(abilities[index].gameObject);
			abilities[index] = Instantiate(to, abilityHolder);
			if (index == _activeAbilityIndex)
				_activeAbility = abilities[index];

			return true;
		}

		public int AbilityOfTypeAtIndex(AbilityBase ability)
		{
			int count = abilities.Length;
			for (int i = 0; i < count; i++)
			{
				if (abilities[i].GetType() != ability.GetType())
					continue;
				return i;
			}
			return -1;
		}
	}
}