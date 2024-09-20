using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	[SerializeField] private bool invertDirection=true;

	private void LateUpdate()
	{
		if (!invertDirection)
			transform.forward = Camera.main.transform.forward;
		else
			transform.forward = -Camera.main.transform.forward;
	}
}