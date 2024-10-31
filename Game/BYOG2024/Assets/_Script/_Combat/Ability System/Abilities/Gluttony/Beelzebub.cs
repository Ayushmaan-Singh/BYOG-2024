using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using Combat;
using UnityEngine;

namespace Entity.Abilities
{
	public class Beelzebub : AbilityBase
	{
		[Header("Ability Property")]
		[SerializeField] private GameObject blackHole;
		[SerializeField] private float blackHoleRadius;
		[SerializeField] private Transform blackHoleModel;
		[SerializeField] private Vector3 maxSize;
		[SerializeField] private float duration;
		[SerializeField] private AnimationCurve blackHoleScaleCurve;
		[SerializeField] private GameObject indicator;

		[Header("Ability Effect")]
		[SerializeField] private float angularVelocity;
		[SerializeField] private float pullStrength;
		[SerializeField, Range(0f, 1f)] private float sizeReductionRate;
		[SerializeField, Range(0f, 1f), Tooltip("Normalized Time")] private float consumeAllEntitiesInRangeAfterTime;

		private CountdownTimer _effectTimer;

		private Dictionary<Rigidbody, ConsumableEntity> _objectAffectedByBeelzebub = new Dictionary<Rigidbody, ConsumableEntity>();

		private void Awake()
		{
			_effectTimer = new CountdownTimer(duration)
			{
				OnTimerStart = () =>
				{
					indicator.SetActive(false);
					blackHole.transform.position = indicator.transform.position.With(y:blackHole.transform.position.y);
					CurrentState = State.InProgress;
					blackHole.SetActive(true);
				},
				OnTimerStop = () =>
				{
					CurrentState = State.Usable;
					blackHole.SetActive(false);
				}
			};
		}

		private void OnEnable()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}

		private void OnDisable()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
		}

		private void Update()
		{
			if (indicator.activeInHierarchy)
				indicator.transform.position = ServiceLocator.For(this).Get<EntityAbilitySystem>()?.AimAt ?? Vector3.zero;

			_effectTimer.Tick(Time.deltaTime);

			if (!_effectTimer.IsRunning)
				return;

			//Size of blackHole
			blackHoleModel.transform.localScale = maxSize * blackHoleScaleCurve.Evaluate(_effectTimer.Progress);

			//Pull objects inside the black hole and if the time to consumeAllEntitiesInRangeAfterTime is crossed consume all entities in range

			List<Rigidbody> keys = _objectAffectedByBeelzebub.Keys.ToList();
			foreach (Rigidbody key in keys)
			{
				if (_effectTimer.Progress >= consumeAllEntitiesInRangeAfterTime)
				{
					ConsumeAndDestroy(key, _objectAffectedByBeelzebub[key]);
				}
				else
				{
					PullToCenter(key, _objectAffectedByBeelzebub[key]);
				}
			}
		}

		private void PullToCenter(Rigidbody rb, ConsumableEntity entity)
		{
			float distance = Mathf.Sqrt((blackHole.transform.position - rb.transform.position).sqrMagnitude);

			// Adjust pull strength based on distance
			float pullStrengthCalculated = pullStrength * distance;

			// Adjust angular velocity based on distance
			float angularVelocityCalculated = angularVelocity * distance;

			// Apply rotation around the black hole
			rb.transform.RotateAround(blackHole.transform.position.With(y:rb.transform.position.y), Vector3.up, angularVelocityCalculated * Time.deltaTime);

			// Move towards the black hole center
			Vector3 directionToCenter = (blackHole.transform.position.With(y:0) - rb.transform.position.With(y:0)).normalized;
			rb.transform.position += directionToCenter * (pullStrengthCalculated * Time.deltaTime);

			// Calculate size reduction factor based on distance and current size
			float sizeFactor = rb.transform.localScale.magnitude;
			float reductionFactor = sizeReductionRate * distance * sizeFactor * Time.deltaTime;
			rb.transform.localScale -= rb.transform.localScale * reductionFactor;

			// Ensure the object doesn’t scale to negative values
			if (rb.transform.localScale.magnitude <= 0.01f)
				rb.transform.localScale = Vector3.zero;

			//Consume if distance less than given or 
			if (Mathf.Sqrt((blackHole.transform.position - rb.transform.position).sqrMagnitude) < blackHoleRadius || rb.transform.localScale == Vector3.zero)
				ConsumeAndDestroy(rb, entity);
		}
		private void ConsumeAndDestroy(Rigidbody rb, ConsumableEntity entity)
		{
			ServiceLocator.ForSceneOf(this).Get<AbilityEvolveSystem>().ConsumeEntity(entity);
			_objectAffectedByBeelzebub.Remove(rb);
			Destroy(ServiceLocator.For(entity).gameObject);
		}

		public override void Execute()
		{
			if (CurrentState != State.Usable)
				return;

			indicator.SetActive(true);
		}

		public override void CancelExecution()
		{
			if (CurrentState == State.Usable)
				_effectTimer.Start();
		}

		private void OnCollisionEnter(Collision collision)
		{
			ConsumableEntity consumableEntity = collision.gameObject.GetComponentInChildren<ConsumableEntity>();
			consumableEntity?.Consume(this);
			Rigidbody rb = consumableEntity?.GetComponentInParent<Rigidbody>();
			if (rb != null && !_objectAffectedByBeelzebub.ContainsKey(rb))
				_objectAffectedByBeelzebub.Add(rb, consumableEntity);
		}
	}
}