using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Combat.Player;
using Entity;
using Entity.Player;
using UnityEngine;
using PlayerController = Gamekit3D.PlayerController;

namespace Combat.Enemy
{
	public class EnemyAbilitySystem : EntityAbilitySystem
	{
		[SerializeField] private PlayerRuntimeSet playerRTSet;

		protected new void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		protected new void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		private void Update()
		{
			AimAt = playerRTSet.Owner?.MainBodyRb.transform.position;
		}
	}
}