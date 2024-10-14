using System;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Input;
using UnityEngine;

namespace Entity.Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField] private Rigidbody rb;
		[SerializeField] private Transform animatedBody;
		[SerializeField] private float rotationSpeed;

		[SerializeField, Range(0, 100)] private int speedPercentLossOnSideMovement;
		[SerializeField, Range(0, 100)] private int speedPercentLossOnBackMovement;
		[SerializeField, Range(0f, 1f)] private float speedLossDetectionOnMovingSideThreshold;
		[SerializeField, Range(0f, 1f)] private float speedLossDetectionOnMovingBackThreshold;

		private Vector3 movementDir = Vector3.zero;

		private ServiceLocator _serviceLocator;
		private static readonly int ZDir = Animator.StringToHash("ZDir");
		private static readonly int XDir = Animator.StringToHash("XDir");

		private float _currentSpeed;

		private void Awake()
		{
			_serviceLocator = ServiceLocator.For(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}

		private void FixedUpdate()
		{

			AnimationDirection();
			if (rb && movementDir != Vector3.zero)
				rb.MovePosition(rb.position + movementDir * (_currentSpeed * Time.fixedDeltaTime));

			rb.MoveRotation(
				Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(ServiceLocator.Global.Get<InputUtils.MousePosition>().Invoke()
				                                                      - rb.position.With(y:0), Vector3.up), Time.deltaTime * rotationSpeed));
		}

		public void Movement(Vector2 direction)
		{
			Vector3 dir = Vector3.zero;

			if (direction.y > 0)
				dir += new Vector3(1, 0, 1);
			if (direction.y < 0)
				dir += new Vector3(-1, 0, -1);
			if (direction.x < 0)
				dir += new Vector3(-1, 0, 1);
			if (direction.x > 0)
				dir += new Vector3(1, 0, -1);

			movementDir = new Vector3(dir.x > 0 ? 1 : dir.x == 0 ? 0 : -1, 0, dir.z > 0 ? 1 : dir.z == 0 ? 0 : -1);
		}

		private void AnimationDirection()
		{
			Vector3 relativeDirection = transform.InverseTransformDirection(movementDir.SetPrecision(1));
			relativeDirection = relativeDirection.With(Mathf.Clamp(Mathf.RoundToInt(relativeDirection.x), -1, 1), 0, Mathf.Clamp(Mathf.RoundToInt(relativeDirection.z), -1, 1));

			animator.SetFloat(XDir, relativeDirection.x);
			animator.SetFloat(ZDir, relativeDirection.z);

			_currentSpeed = _serviceLocator.Get<EntityStatSystem>().GetInstanceStats(Stats.MovementSpeed);
			float reduction = (relativeDirection.x > speedLossDetectionOnMovingSideThreshold || relativeDirection.x < -speedLossDetectionOnMovingSideThreshold ? _currentSpeed * speedPercentLossOnSideMovement / 100f : 0)
			                  + (relativeDirection.z < -speedLossDetectionOnMovingBackThreshold ? _currentSpeed * speedPercentLossOnBackMovement / 100f : 0);
			_currentSpeed -= reduction;
		}
	}
}