using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;

namespace Global
{
	public class VFXHolder : MonoBehaviour
	{
		private void OnEnable()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}

		private void OnDisable()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
		}
	}
}