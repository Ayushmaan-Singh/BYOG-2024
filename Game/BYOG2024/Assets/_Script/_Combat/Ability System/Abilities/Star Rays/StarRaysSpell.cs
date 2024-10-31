using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.Gameplay;
using UnityEngine;

namespace Entity.Abilities
{
	public class StarRaysSpell : SpellBase
	{
		private float _rayDamage;
		private float _damageIncrement;
		private int _damageIncrementCount;
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
			_rayDamage = 0;
			_damageIncrement = 0;
			_damageIncrementCount = 0;
		}

		public StarRaysSpell Execute()
		{
			GetComponent<ParticleFX>().Play();
			return this;
		}

		private void OnParticleEffectFinished(ParticleFX particleFX)
		{
			gameObject.SetActive(false);
			StarRaysAbility ability = GetComponent<OwnerOfThisObject>().Owner?.GetComponent<StarRaysAbility>();
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
			damageable.Damage(_rayDamage*_damageIncrementCount);
			_damageIncrementCount++;
		}

		public class Builder
		{
			private LayerMask _includeLayers;
			private float _rayDamage;
			private float _damageIncrement;
			private Transform _owner;

			public Builder InitCollisionLayers(LayerMask layers)
			{
				_includeLayers = layers;
				return this;
			}

			public Builder InitRayDamage(float damage)
			{
				_rayDamage = damage;
				return this;
			}
			
			public Builder InitDamageIncrementPerHit(float damage)
			{
				_rayDamage = damage;
				return this;
			}

			public Builder InitOwner(Transform owner)
			{
				_owner = owner;
				return this;
			}

			public StarRaysSpell Build(StarRaysSpell instance)
			{
				instance._collisionLayerFilter.SetLayers(_includeLayers);
				instance._rayDamage = _rayDamage;
				instance.GetComponent<OwnerOfThisObject>().SetOwner(_owner);

				instance.enabled = true;

				return instance;
			}
		}
	}
}
