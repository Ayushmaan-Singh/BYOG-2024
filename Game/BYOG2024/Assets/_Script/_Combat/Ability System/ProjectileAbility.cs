using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using UnityEngine;

namespace Entity.Abilities
{
	public class ProjectileAbility : AbilityBase
	{
		[SerializeField] private Transform muzzle;

		[Space]
		[SerializeField] private float damage;
		[SerializeField] private float projectileSpeed;
		[SerializeField] private float cooldownTime;

		[Space]
		[SerializeField] private LayerMask includeLayers;
		[SerializeField] private LayerMask excludeLayers;

		private CountdownTimer _cooldownTimer;

		private void Awake()
		{
			_cooldownTimer = new CountdownTimer(cooldownTime)
			{
				OnTimerStop = () =>
				{
					CurrentState = State.Usable;
				}
			};
		}

		private void Update()
		{
			_cooldownTimer.Tick(Time.deltaTime);

			if (_cooldownTimer.IsRunning)
				Progress = _cooldownTimer.Progress;
		}

		public override void Execute()
		{
			if (CurrentState != State.Usable)
				return;

			// ProjectileBase ammo =
			// 	ServiceLocator.ForSceneOf(this).Get<ProjectileFactory>().Instantiate<ProjectileBase>();
			//
			// if (!ammo.OrNull())
			// 	return;
			//
			// new ProjectileBase.Builder()
			// 	.InitPosition(muzzle.position)
			// 	.InitForwardDirection(muzzle.forward)
			// 	.InitIncludeLayers(includeLayers)
			// 	.InitExcludeLayers(excludeLayers)
			// 	.InitDmgAmount(damage)
			// 	.InitDmgScaleStat(ServiceLocator.For(this).Get<EntityStatSystem>().GetInstanceStats(Stats.DamageScale))
			// 	.InitProjectileSpeed(projectileSpeed)
			// 	.Build(ammo).Execute();
			//
			// CurrentState = State.Unusable;
			// _cooldownTimer.Start();
		}

		public override void CancelExecution()
		{
			//noap
		}
	}
}