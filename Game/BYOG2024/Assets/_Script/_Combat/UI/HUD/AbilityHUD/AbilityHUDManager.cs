using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Combat.Player;
using Entity.Abilities;
using Entity.Player;
using UnityEngine;
namespace Combat.UI
{
	public class AbilityHUDManager : MonoBehaviour
	{
		[SerializeField] private PlayerRuntimeSet playerRTSet;
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

		private void OnEnable()
		{
			ServiceLocator.For(this).Register(this);
			foreach (AbilityTile tile in abilityTiles)
				new AbilityTile.Builder().SetActiveAbilityIndex(playerRTSet.Owner?.ActiveAbilityIndex() ?? 0).Build(tile);
		}

		private void OnDisable()
		{

			if (!playerRTSet.Owner.OrNull())
				return;

			foreach (AbilityTile tile in abilityTiles)
			{
				playerRTSet.Owner?.AbilityChangedSubject.Detach(tile);
				playerRTSet.Owner?.AbilityStateChangedSubject.Detach(tile);
				playerRTSet.Owner?.AbilityProgressChangedSubject.Detach(tile);
				playerRTSet.Owner?.ActiveAbilitySwitchedSubject.Detach(tile);
			}
			ServiceLocator.For(this)?.Deregister(this);
		}

		private IEnumerator Start()
		{
			yield return new WaitWhile(() => !playerRTSet.Owner);

			foreach (AbilityTile tile in abilityTiles)
			{
				playerRTSet.Owner.AbilityChangedSubject.Attach(tile);
				playerRTSet.Owner.AbilityStateChangedSubject.Attach(tile);
				playerRTSet.Owner.AbilityProgressChangedSubject.Attach(tile);
				playerRTSet.Owner.ActiveAbilitySwitchedSubject.Attach(tile);
			}
		}
	}
}