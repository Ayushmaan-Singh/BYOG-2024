using System;
using AstekUtility.Gameplay.Timer;
using UnityEngine;

namespace AstekUtility.DesignPattern.GOAP
{
	[RequireComponent(typeof(SphereCollider))]
	public class SphericalRangeSensor : MonoBehaviour, ISensor
	{
		[SerializeField] private float detectionRadius = 5f;
		[SerializeField] private float timeInterval = 1f;
		
		public event Action OnTargetChanged = delegate { };
		public Vector3 TargetPosition
		{
			get
			{
				return _target ? _target.transform.position : Vector3.zero;
			}
		}
		public bool IsTargetInRange
		{
			get
			{
				return TargetPosition != Vector3.zero;
			}
		}

		private SphereCollider _detectionRange;
		private GameObject _target;
		private Vector3 _lastKnownPosition;
		private CountdownTimer _timer;

		private void Awake()
		{
			_detectionRange = GetComponent<SphereCollider>();
			_detectionRange.isTrigger = true;
			_detectionRange.radius = detectionRadius;
		}

		private void Start()
		{
			_timer = new CountdownTimer(timeInterval);
			_timer.OnTimerStop += () =>
			{
				UpdateTargetPosition(_target.OrNull());
				_timer.Start();
			};
			_timer.Start();
		}

		private void Update()
		{
			_timer.Tick(Time.deltaTime);
		}

		private void UpdateTargetPosition(GameObject target = null)
		{
			_target = target;
			if (IsTargetInRange && (_lastKnownPosition != TargetPosition || _lastKnownPosition != Vector3.zero))
			{
				_lastKnownPosition = TargetPosition;
				OnTargetChanged.Invoke();
			}
		}

		//TODO: For now we are only detecting player Make different sensors for different enemy with different things to detect
		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Player")) return;
			UpdateTargetPosition(other.gameObject);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag("Player"))
				return;

			UpdateTargetPosition();
		}

		#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			Gizmos.color = IsTargetInRange ? Color.red : Color.green;
			Gizmos.DrawWireSphere(transform.position, detectionRadius);
		}

		#endif
	}
}