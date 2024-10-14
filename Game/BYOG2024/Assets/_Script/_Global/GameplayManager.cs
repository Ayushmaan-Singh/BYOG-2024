using System;
using System.Collections;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Odin;
using Global;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
	public class GameplayManager : MonoBehaviour
	{
		[SerializeField] private CameraRuntimeSet cameraRTSet;
		[SerializeField] private Camera mainCamera;
		[SerializeField, ValueDropdown("@Global.InputManager.ActionMapsName")] private string combatActionMap;
		[SerializeField, ValueDropdown("@Global.InputManager.ActionMapsName")] private string nullMap;

		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
			if (!cameraRTSet.ContainsKey(CameraInGame.MainCamera))
				cameraRTSet.Add(CameraInGame.MainCamera, mainCamera.gameObject);
			else
				cameraRTSet[CameraInGame.MainCamera] = mainCamera.gameObject;
		}

		private void Start()
		{
			ServiceLocator.Global.Get<InputManager>().SwitchActiveActionMap(combatActionMap);
		}

		private void OnDestroy()
		{
			cameraRTSet.Remove(CameraInGame.MainCamera);
			ServiceLocator.ForSceneOf(this)?.Deregister(this);

			InputManager inputManager = null;
			ServiceLocator.Global?.TryGetService(out inputManager);
			if (inputManager != null)
				inputManager.SwitchActiveActionMap(nullMap);
		}
	}
}