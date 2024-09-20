using System;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Input;
using UnityEngine;

namespace Entity.Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private Rigidbody rb;
		private Vector3 movementDir = Vector3.zero;

		private ServiceLocator _serviceLocator;

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
			if (rb && movementDir != Vector3.zero)
				rb.MovePosition(rb.position + movementDir * (_serviceLocator.Get<EntityStatSystem>().GetInstanceStats(Stats.MovementSpeed) * Time.fixedDeltaTime));
			
			rb.rotation = Quaternion.LookRotation(ServiceLocator.Global.Get<InputUtils.MousePosition>().Invoke() - rb.position.With(y:0),Vector3.up);
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

			movementDir = dir.normalized;
		}
	}
}