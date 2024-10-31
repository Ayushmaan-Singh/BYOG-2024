using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.Gameplay;
using UnityEngine;

namespace Entity.Abilities
{
	[RequireComponent(typeof(CollisionLayerFilter))]
	public class Explosion : MonoBehaviour
	{
		private float _damage;
		private CollisionLayerFilter _collisionLayerFilter;
		
		private void Awake()
		{
			_collisionLayerFilter = GetComponent<CollisionLayerFilter>();
			enabled = false;
		}

		public void OnParticleCollisionEnter(Collider collision)
		{
			if (!_collisionLayerFilter.CanCollide(collision.gameObject))
				return;
			
			IDamageable damageable = collision.gameObject.GetComponentInParent<IDamageable>();
			damageable.Damage(_damage);
		}

		public class Builder
		{
			private float _damage;
			private LayerMask _includeLayers;
			
			public Builder InitCollisionLayers(LayerMask layers)
			{
				_includeLayers = layers;
				return this;
			}

			public Builder InitExplosionDamage(float damage)
			{
				_damage = damage;
				return this;
			}

			public Explosion Build(Explosion instance)
			{
				instance._damage = _damage;
				instance._collisionLayerFilter.SetLayers(_includeLayers);

				instance.enabled = true;
				instance.gameObject.SetActive(true);

				return instance;
			}
		}
	}
}
