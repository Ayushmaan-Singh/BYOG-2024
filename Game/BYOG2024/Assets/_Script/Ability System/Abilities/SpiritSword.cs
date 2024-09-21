using System;
using System.Collections;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Input;
using AstekUtility.Odin.Utility;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.VFX;
namespace Entity.Abilities
{
	public class SpiritSword : AbilityBase
	{
		[SerializeField] private float damage;

		[Header("Visuals")]
		[SerializeField] private GameObject sword;
		[SerializeField] private SplineContainer[] attackSpline;
		[SerializeField] private SplineAnimate animate;
		[SerializeField] private TrailRenderer trail;
		 
		private string _oppositionTag="Enemy";

		private int _index = 0;
		//TODO:Add effects like trailrenderer and slashes

		private void Awake()
		{
			trail.Clear();
			sword.SetActive(false);
			GetComponentInChildren<OnCollisionEnterEvent>().Register(Damage);
		}

		private void Update()
		{
			if (animate.ElapsedTime / animate.Duration >= 1f)
			{
				sword.SetActive(false);
				animate.Restart(false);
				trail.Clear();
				_currentState = State.Usable;
			}
		}

		public override void Execute()
		{
			if (_currentState != State.Usable)
				return;

			_currentState = State.InProgress;
			sword.SetActive(true);
			animate.Container = attackSpline[_index];
			_index = _index == 0 ? 1 : 0;
			animate.Play();

		}

		public override void CancelExecution() { }

		private void Damage(Collision collision)
		{
			if (collision.collider.CompareTag(_oppositionTag))
			{
				EntityHealthManager healthManager = collision.collider.GetComponentInChildren<EntityHealthManager>();
				if(healthManager==null)
					healthManager = collision.collider.GetComponentInParent<EntityHealthManager>();
                
				healthManager.Damage(
					ServiceLocator.For(this).Get<EntityStatSystem>().GetInstanceStats(Stats.DamageScale) * damage / 100);
			}
		}
		//TODO: During on switch provide this character with tags
	}
}