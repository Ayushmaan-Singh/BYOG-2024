using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.Gameplay;
using UnityEngine;

namespace Entity.Abilities
{
	internal class DarkArrowVolleySpell : MonoBehaviour
	{
		private float _damagePerHit;
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
			_damagePerHit = 0;
		}

		public DarkArrowVolleySpell Execute()
		{
			GetComponent<ParticleFX>().Play();
			return this;
		}

		private void OnParticleEffectFinished(ParticleFX particleFX)
		{
			gameObject.SetActive(false);
			DarkArrowVolleyAbility ability = GetComponent<OwnerOfThisObject>().Owner?.GetComponent<DarkArrowVolleyAbility>();
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
			damageable.Damage(_damagePerHit);
		}

		public class Builder
		{
			private LayerMask _includeLayers;
			private float _damagePerHit;
			private Transform _owner;

			public Builder InitCollisionLayers(LayerMask layers)
			{
				_includeLayers.value = layers;
				return this;
			}

			public Builder InitDamagePerHit(float damagePerHit)
			{
				_damagePerHit = damagePerHit;
				return this;
			}

			public Builder InitOwner(Transform owner)
			{
				_owner = owner;
				return this;
			}

			public DarkArrowVolleySpell Build(DarkArrowVolleySpell instance)
			{
				instance._collisionLayerFilter.SetLayers(_includeLayers);
				instance._damagePerHit = _damagePerHit;
				instance.GetComponent<OwnerOfThisObject>().SetOwner(_owner);

				instance.enabled = true;

				return instance;
			}
		}
	}
}
