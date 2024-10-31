using System;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using Global;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entity.Abilities
{
	public class HammerDropAbility : AbilityBase
	{
		[Header("Ability Data")]
		[SerializeField] private float hammerDropDamage;
		[SerializeField,MaxValue("@hammerDropDamage")] private float shockwaveDamage;
		[SerializeField] private float cooldown;
		[SerializeField] private GameObject indicator;
		[SerializeField] private LayerMask _includeLayers;

		[Header("Particle FX")]
		[SerializeField] private GameObject spellPrefab;

		private ObjectPool<GameObject> _particleObjectPool;
		private CountdownTimer _cooldownTimer;

		private void Awake()
		{
			_cooldownTimer = new CountdownTimer(cooldown)
			{
				OnTimerStop = () =>
				{
					_cooldownTimer.Reset();
					CurrentState = State.Usable;
				}
			};

			//Object Pool
			_particleObjectPool = new ObjectPool<GameObject>();
			_particleObjectPool.OnPooled += (obj) =>
			{
				obj.SetActive(false);
			};
			_particleObjectPool.OnRelease += (obj) =>
			{
				obj.SetActive(true);
			};
			_particleObjectPool.OnClear += Destroy;
		}

		private void Update()
		{
			switch (CurrentState)
			{

				case State.Usable:

					indicator.transform.position = ServiceLocator.For(this).Get<EntityAbilitySystem>().AimAt ?? Vector3.zero;
					if (!indicator.activeInHierarchy && ServiceLocator.For(this).Get<EntityAbilitySystem>().GetActiveAbility == this)
						indicator.SetActive(true);

					if (indicator.activeInHierarchy && ServiceLocator.For(this).Get<EntityAbilitySystem>().GetActiveAbility != this)
						indicator.SetActive(false);

					break;

				case State.Unusable:

					_cooldownTimer.Tick(Time.deltaTime);
					Progress = _cooldownTimer.Progress;
					
					if (indicator.activeInHierarchy)
						indicator.SetActive(false);

					break;

				case State.InProgress:

					if (indicator.activeInHierarchy)
						indicator.SetActive(false);

					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void Execute()
		{
			if (CurrentState != State.Usable)
				return;

			CurrentState = State.Unusable;
			GameObject spawn = _particleObjectPool.ReleaseObject() ?? Instantiate(spellPrefab, ServiceLocator.ForSceneOf(this).Get<VFXHolder>().transform);
			spawn.transform.position = ServiceLocator.For(this).Get<EntityAbilitySystem>().AimAt ?? Vector3.zero;
			new HammerDropSpell.Builder()
				.InitCollisionLayers(_includeLayers)
				.InitHammerDropDamage(hammerDropDamage)
				.InitShockwaveDamage(shockwaveDamage)
				.InitOwner(transform)
				.Build(spawn.GetComponent<HammerDropSpell>())
				.Execute();

			_cooldownTimer.Start();
		}
		public override void CancelExecution()
		{
			//noap
		}

		public void PoolObject(GameObject obj)
		{
			_particleObjectPool.PoolObject(obj);
		}

		public class Builder
		{
			private LayerMask _includeLayers;

			public Builder InitIncludeCollisionLayers(LayerMask layers)
			{
				_includeLayers = layers;
				return this;
			}

			public HammerDropAbility Build(HammerDropAbility instance)
			{
				instance._includeLayers = _includeLayers;
				return instance;
			}
		}
	}
}
