using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Entity.Abilities;
using Entity.Player;
using UnityEngine;
namespace Combat.UI
{
	public class AbilityHUDManager : MonoBehaviour
	{
		[SerializeField] private AbilityTile[] abilityTiles;

		public int GetTileOrder(AbilityTile tile)
		{
			int count = abilityTiles.Length;
			for (int i = 0; i < count; i++)
			{
				if (abilityTiles[i] != tile)
					continue;
				return i;
			}
			return -1;
		}

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
			
			PlayerMediator mediator = ServiceLocator.ForSceneOf(this)?.Get<PlayerMediator>();
			if (!mediator)
				return;
			
			foreach (AbilityTile tile in abilityTiles)
			{
				mediator.AbilityChangedSubject.Detach(tile);
				mediator.AbilityStateChangedSubject.Detach(tile);
				mediator.AbilityProgressChangedSubject.Detach(tile);
				mediator.ActiveAbilitySwitchedSubject.Detach(tile);
			}
		}

		private IEnumerator Start()
		{
			yield return new WaitWhile(() =>
			{
				ServiceLocator forSceneOf = ServiceLocator.ForSceneOf(this);
				return forSceneOf?.Get<PlayerMediator>() == null;
			});
			
			PlayerMediator mediator = ServiceLocator.ForSceneOf(this).Get<PlayerMediator>();
			foreach (AbilityTile tile in abilityTiles)
			{
				mediator.AbilityChangedSubject.Attach(tile);
				mediator.AbilityStateChangedSubject.Attach(tile);
				mediator.AbilityProgressChangedSubject.Attach(tile);
				mediator.ActiveAbilitySwitchedSubject.Attach(tile);
			}
		}
	}
}