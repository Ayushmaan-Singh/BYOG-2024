using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.Gameplay;
using UnityEngine;
using UnityEngine.Rendering;

namespace Entity.Abilities
{
	public class HammerDropSpell : SpellBase
	{
		private float _hammerDropDamage;
		private float _shockWaveDamage;
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
			_hammerDropDamage = 0;
			_shockWaveDamage = 0;
		}

		public HammerDropSpell Execute()
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

		public void OnParticleCollisionEnter(Collider collision)
		{
			if (!_collisionLayerFilter.CanCollide(collision.gameObject))
				return;

			IDamageable damageable = collision.gameObject.GetComponentInParent<IDamageable>();
			damageable.Damage(_shockWaveDamage);
		}

		public void OnCollisionEnter(Collision collision)
		{
			if (!_collisionLayerFilter.CanCollide(collision.gameObject))
				return;

			IDamageable damageable = collision.gameObject.GetComponentInParent<IDamageable>();
			damageable.Damage(_hammerDropDamage);
		}

		public class Builder
		{
			private LayerMask _includeLayers;
			private float _hammerDropDamage;
			private float _shockwaveDamage;
			private Transform _owner;

			public Builder InitCollisionLayers(LayerMask layers)
			{
				_includeLayers = layers;
				return this;
			}

			public Builder InitHammerDropDamage(float damage)
			{
				_hammerDropDamage = damage;
				return this;
			}
			
			public Builder InitShockwaveDamage(float damage)
			{
				_shockwaveDamage = damage;
				return this;
			}

			public Builder InitOwner(Transform owner)
			{
				_owner = owner;
				return this;
			}

			public HammerDropSpell Build(HammerDropSpell instance)
			{
				instance._collisionLayerFilter.SetLayers(_includeLayers);
				instance._hammerDropDamage = _hammerDropDamage;
				instance._shockWaveDamage = _shockwaveDamage;
				instance.GetComponent<OwnerOfThisObject>().SetOwner(_owner);

				instance.enabled = true;

				return instance;
			}
		}
	}
}
