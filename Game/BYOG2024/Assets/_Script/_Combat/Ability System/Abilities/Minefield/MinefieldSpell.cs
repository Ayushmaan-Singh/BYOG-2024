using AstekUtility;
using AstekUtility.Gameplay;
using UnityEngine;

namespace Entity.Abilities
{
	public class MinefieldSpell : SpellBase
	{
		[SerializeField] private MineExplosion mineExplosion;
		private float _explosionDamage;
		private LayerMask _includeLayers;

		private void Awake()
		{
			enabled = false;
		}

		private void OnEnable()
		{
			GetComponent<ParticleFX>().OnParticleEffectFinished += OnParticleEffectFinished;
			mineExplosion.enabled = true;
		}

		private void OnDisable()
		{
			GetComponent<ParticleFX>().OnParticleEffectFinished -= OnParticleEffectFinished;

			mineExplosion.enabled = false;
			_includeLayers = 0;
			_explosionDamage = 0;
		}

		public MinefieldSpell Execute()
		{
			GetComponent<ParticleFX>().Play();
			return this;
		}

		private void OnParticleEffectFinished(ParticleFX particleFX)
		{
			gameObject.SetActive(false);
			MinefieldAbility ability = GetComponent<OwnerOfThisObject>().Owner?.GetComponent<MinefieldAbility>();
			if (ability.OrNull())
				ability?.PoolObject(gameObject);
			else
				Destroy(gameObject);
		}

		public void OnParticleCollisionEnter_WalkingInToMine(Collider collision, Explosion explosion)
		{
			new Explosion.Builder()
				.InitCollisionLayers(_includeLayers)
				.InitExplosionDamage(_explosionDamage)
				.Build(explosion);
		}

		public class Builder
		{
			private LayerMask _includeLayers;
			private float _explosionDamage;
			private Transform _owner;

			public Builder InitCollisionLayers(LayerMask layers)
			{
				_includeLayers = layers;
				return this;
			}

			public Builder InitExplosionDamage(float damage)
			{
				_explosionDamage = damage;
				return this;
			}

			public Builder InitOwner(Transform owner)
			{
				_owner = owner;
				return this;
			}

			public MinefieldSpell Build(MinefieldSpell instance)
			{
				instance._includeLayers = _includeLayers;
				instance._explosionDamage = _explosionDamage;
				instance.GetComponent<OwnerOfThisObject>().SetOwner(_owner);

				new MineExplosion.Builder()
					.InitCollisionLayers(_includeLayers)
					.Build(instance.mineExplosion);

				instance.enabled = true;

				return instance;
			}
		}
	}
}