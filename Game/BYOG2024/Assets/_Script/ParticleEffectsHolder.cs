using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;

namespace Combat
{
	public class ParticleEffectsHolder : MonoBehaviour
	{
		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}
		
		private void OnDestroy()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
		}
	}
}
