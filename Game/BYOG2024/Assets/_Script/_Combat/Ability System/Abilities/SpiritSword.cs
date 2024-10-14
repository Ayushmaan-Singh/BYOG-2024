using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay;
using AstekUtility.Gameplay.Collision;
using AstekUtility.Input;
using AstekUtility.Odin.Utility;
using Entity.Player;
using Global.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Splines;
using UnityEngine.VFX;
namespace Entity.Abilities
{
	public class SpiritSword : AbilityBase
	{
		[SerializeField] private float damage;

		[Header("Visuals")]
		[SerializeField] private GameObject sword;
		[SerializeField] private GameObject hitEffect;
		[SerializeField] private Collider[] damageColliders;
		[SerializeField, RequiredListLength(2)] private SplineContainer[] attackSpline;
		[SerializeField, RequiredListLength(2)] private GameObject[] slashParticleSystem;
		[SerializeField] private SplineAnimate animate;

		private Rigidbody _rb;
		private HandleMultiColliderSingleEntityCollision handleCollisionProcessing = new HandleMultiColliderSingleEntityCollision();

		private int _index = 0;

		private void Awake()
		{
			sword.SetActive(false);
			damageColliders.ForEach(damageCollider => damageCollider.enabled = false);
			_rb = GetComponentInChildren<Rigidbody>();
		}

		private void Update()
		{
			if (Mathf.Approximately(animate.ElapsedTime / animate.Duration, 1f) || animate.ElapsedTime / animate.Duration > 1f)
			{
				sword.SetActive(false);
				damageColliders.ForEach(damageCollider => damageCollider.enabled = false);
				handleCollisionProcessing.Reset();
				animate.Restart(false);
				CurrentState = State.Usable;
			}
			else
			{
				Progress = animate.ElapsedTime / animate.Duration;
			}
		}

		public override void Execute()
		{
			if (CurrentState != State.Usable)
				return;

			CurrentState = State.InProgress;
			damageColliders.ForEach(damageCollider => damageCollider.enabled = true);
			sword.SetActive(true);
			animate.Container = attackSpline[_index];
			slashParticleSystem[_index].GetComponentsInChildren<ParticleSystem>().ForEach(particle => particle.Play());
			sword.transform.localRotation = Quaternion.Euler(sword.transform.localRotation.x, sword.transform.localRotation.y, 90 + (_index * 180));
			_index = _index == 0 ? 1 : 0;
			animate.Play();
		}

		public override void CancelExecution() { }

		private void OnCollisionEnter(Collision collision)
		{
			handleCollisionProcessing.ProcessCollisionEnter(collision, out bool isAlreadyInCollision);
			if (isAlreadyInCollision || ServiceLocator.For(this).transform == collision.gameObject.GetComponentInParent<ServiceLocator>().transform)
				return;

			Debug.Log(collision.gameObject.name);
			GameObject effect = Instantiate(hitEffect, ServiceLocator.ForSceneOf(this).Get<ParticleEffectsInGameObjectPool>()?.transform);
			effect.transform.position = collision.GetContact(0).point;
			effect.transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);
			effect.GetComponentsInChildren<ParticleSystem>().ForEach(particle => particle.Play());

			IDamageable damageableEntity = collision.gameObject.GetComponent<Collider>().GetComponentInParent<IDamageable>();
			damageableEntity?.Damage(
				ServiceLocator.For(this).Get<EntityStatSystem>().GetInstanceStats(Stats.DamageScale) * damage / 100);
		}

		private void OnCollisionExit(Collision collision) => handleCollisionProcessing.ProcessCollisionExit(collision);


		public class Builder
		{
			private LayerMask _excludeLayers;
			private LayerMask _includeLayers;

			public Builder ExcludeLayers(LayerMask tags)
			{
				_excludeLayers = tags;
				return this;
			}

			public Builder IncludeLayers(LayerMask tags)
			{
				_includeLayers = tags;
				return this;
			}

			public SpiritSword Build(SpiritSword instance)
			{
				instance._rb.includeLayers = _includeLayers;
				instance._rb.excludeLayers = _excludeLayers;

				//position, rotation and scale
				TransformValues offset = instance.localOffsetFromModel[typeof(SpiritSword)];
				instance.transform.localPosition = offset.Position;
				instance.transform.localRotation = offset.Rotation;
				instance.transform.localScale = offset.Scale;

				//Parent constraint
				instance.GetComponentInChildren<ParentConstraint>().AddSource(new ConstraintSource
				{
					sourceTransform = ServiceLocator.For(instance).Get<PlayerMediator>().MainRotationBody,
					weight = 1.0f
				});

				return instance;
			}
		}
	}
}