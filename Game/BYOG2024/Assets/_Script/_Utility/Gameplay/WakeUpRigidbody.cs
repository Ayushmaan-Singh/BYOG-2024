using System;
using System.Collections;
using UnityEngine;

namespace AstekUtility.Gameplay
{
	public class WakeUpRigidbody : MonoBehaviour
	{
		private Rigidbody _rb;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
			_rb.sleepThreshold = 0;
		}

		private void FixedUpdate() => _rb.MovePosition(_rb.position);
	}
}