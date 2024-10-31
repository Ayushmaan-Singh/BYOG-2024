using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay;
using UnityEngine;

namespace Entity.Abilities
{
	[RequireComponent(typeof(OwnerOfThisObject))]
	public class ProjectileBase : MonoBehaviour
	{
		[SerializeField] private List<Collider> colliders;
		[SerializeField] private GameObject mainObject;

		[SerializeField] private LayerMask gluttonyShieldLayerMask;

		private Rigidbody _rb;
		private float _attackDmg;
		private float _speed;

		private bool _canPlay;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
			enabled = false;
		}

		private void FixedUpdate()
		{
			if (_canPlay)
				_rb.MovePosition(_rb.position + _rb.transform.forward * (Time.fixedDeltaTime * _speed));
		}

		public ProjectileBase Execute()
		{
			_canPlay = true;
			return this;
		}

		private void DestroyProjectile()
		{
			//ServiceLocator.ForSceneOf(this).Get<ProjectileInGameObjectPool>().PoolThisObject(this);
			_canPlay = false;
			enabled = false;
			mainObject.SetActive(false);
		}

		public void OnCollisionEnter(Collision collision)
		{
			_speed = 0;
			//if colliding with gluttony shield
			if (collision.gameObject.IsInLayer(gluttonyShieldLayerMask))
			{
				collision.collider.GetComponentInParent<Gluttony>().OnCollisionEnter(gameObject);
			}
			//If its some other object
			else
			{
				collision.gameObject.GetComponentInParent<IDamageable>().Damage(_attackDmg);
			}

			//Destroy Projectile
			DestroyProjectile();
		}

		public class Builder
		{
			private Vector3 _position;
			private Vector3 _direction;

			private LayerMask _includeLayers;
			private LayerMask _excludeLayers;

			private float _dmgAmount;
			private float _dmgScaleStat;
			private float _speed;

			public Builder InitPosition(Vector3 position)
			{
				_position = position;
				return this;
			}

			public Builder InitForwardDirection(Vector3 direction)
			{
				_direction = direction;
				return this;
			}

			public Builder InitIncludeLayers(LayerMask layerMask)
			{
				_includeLayers = layerMask;
				return this;
			}

			public Builder InitExcludeLayers(LayerMask layerMask)
			{
				_excludeLayers = layerMask;
				return this;
			}

			public Builder InitDmgAmount(float dmgAmount)
			{
				_dmgAmount = dmgAmount;
				return this;
			}

			public Builder InitProjectileSpeed(float speed)
			{
				_speed = speed;
				return this;
			}

			public Builder InitDmgScaleStat(float dmgScaleStat)
			{
				_dmgScaleStat = dmgScaleStat;
				return this;
			}

			public ProjectileBase Build(ProjectileBase instance)
			{
				instance._rb.transform.position = _position;
				instance._rb.transform.forward = _direction;

				instance.colliders.ForEach(collider =>
				{
					collider.includeLayers = _includeLayers;
					collider.excludeLayers = _excludeLayers;
				});

				instance._attackDmg = _dmgAmount * _dmgScaleStat / 100;
				instance._speed = _speed;

				instance.enabled = true;
				instance.mainObject.SetActive(true);

				return instance;
			}
		}
	}
}