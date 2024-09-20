using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Global;
using UnityEngine;

namespace Gameplay
{
	public class GameplayManager : MonoBehaviour
	{
		[SerializeField] private CameraRuntimeSet cameraRTSet;
		[SerializeField] private Camera mainCamera;

		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
			cameraRTSet.Add(CameraInGame.MainCamera, mainCamera.gameObject);
		}

		private void OnDestroy()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
			cameraRTSet.Remove(CameraInGame.MainCamera);
		}
	}
}
