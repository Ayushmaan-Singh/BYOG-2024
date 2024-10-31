using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.Gameplay;
using UnityEngine;

namespace Entity.Abilities
{
	public class OrbitalLaserBarrageSpell : SpellBase
	{
		private float _laserDamage;
		private float _shockWaveDamage;
		private float _executeBelowHealth;
		private CollisionLayerFilter _collisionLayerFilter;

		private void Awake()
		{
			_collisionLayerFilter = GetComponent<CollisionLayerFilter>();
			enabled = false;
		}

		private void OnEnable()
		{
			GetComponent<ParticleFX>().OnParticleEffectFinished += OnParticleEffectFinished;
		}

		private void OnDisable()
		{
			GetComponent<ParticleFX>().OnParticleEffectFinished -= OnParticleEffectFinished;
			_collisionLayerFilter.SetLayers(0);
			_laserDamage = 0;
			_shockWaveDamage = 0;
			_executeBelowHealth = 0;
		}

		public OrbitalLaserBarrageSpell Execute()
		{
			GetComponent<ParticleFX>().Play();
			return this;
		}

		private void OnParticleEffectFinished(ParticleFX particleFX)
		{
			gameObject.SetActive(false);
			HammerDropAbility ability = GetComponent<OwnerOfThisObject>().Owner?.GetComponent<HammerDropAbility>();
			if (ability.OrNull())
				ability?.PoolObject(gameObject);
			else
				Destroy(gameObject);
		}

		public void OnParticleCollisionEnter_Laser(Collider collision)
		{
			if (!_collisionLayerFilter.CanCollide(collision.gameObject))
				return;

			IDamageable damageable = collision.gameObject.GetComponentInParent<IDamageable>();

			damageable.Damage(damageable.CurrentHp / damageable.MaxHp < _executeBelowHealth ? damageable.CurrentHp : _laserDamage);
		}

		public void OnParticleCollisionEnter_ShockWave(Collider collision)
		{
			if (!_collisionLayerFilter.CanCollide(collision.gameObject))
				return;

			IDamageable damageable = collision.gameObject.GetComponentInParent<IDamageable>();
			damageable.Damage(_shockWaveDamage);
		}

		public class Builder
		{
			private LayerMask _includeLayers;
			private float _laserDamage;
			private float _shockwaveDamage;
			private float _executeBelowHealth;
			private Transform _owner;

			public Builder InitCollisionLayers(LayerMask layers)
			{
				_includeLayers.value = layers;
				return this;
			}

			public Builder InitLaserDamage(float damage)
			{
				_laserDamage = damage;
				return this;
			}

			public Builder InitShockwaveDamage(float damage)
			{
				_shockwaveDamage = damage;
				return this;
			}

			public Builder InitExecuteBelowHealthPercent(float health)
			{
				_executeBelowHealth = health;
				return this;
			}

			public Builder InitOwner(Transform owner)
			{
				_owner = owner;
				return this;
			}

			public OrbitalLaserBarrageSpell Build(OrbitalLaserBarrageSpell instance)
			{
				instance._collisionLayerFilter.SetLayers(_includeLayers);
				instance._laserDamage = _laserDamage;
				instance._shockWaveDamage = _shockwaveDamage;
				instance._executeBelowHealth = _executeBelowHealth;
				instance.GetComponent<OwnerOfThisObject>().SetOwner(_owner);

				instance.enabled = true;

				return instance;
			}
		}
	}
}