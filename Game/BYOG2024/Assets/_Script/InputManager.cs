using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Global
{
	public class InputManager : MonoBehaviour
	{
		[Header("Input Action Asset")]
		[SerializeField] private InputActionAssetCollection _inputActionAssets;
		[SerializeField] private InputActionType _activeActionType;

		[Header("Action Map")]
		[SerializeField, ValueDropdown("@ActionMapsName")] private string defaultActionMap;

		private InputActionMap _currentActionMap;
		private InputActionMap _prevActionMap;

		#if UNITY_EDITOR && ODIN_INSPECTOR

		public static ValueDropdownList<InputActionMap> ActionMaps;
		public static ValueDropdownList<string> ActionMapsName;
		private void OnValidate()
		{
			ActionMaps = new ValueDropdownList<InputActionMap>();
			ActionMapsName = new ValueDropdownList<string>();
			if (_inputActionAssets != null)
			{
				foreach (InputActionMap map in _inputActionAssets[_activeActionType].actionMaps)
				{
					ActionMaps.Add($"{map.name}", map);
					ActionMapsName.Add(map.name);
				}
			}
		}
		#endif

		private void Awake()
		{
			ServiceLocator.Global.Register(this);
			SwitchActiveActionMap(defaultActionMap);
		}

		private void OnDestroy()
		{
			ServiceLocator.Global?.Deregister(this);
		}

		/// <summary>
		/// Allows multiple action map to work at same time
		/// </summary>
		/// <param name="actionMap"></param>
		/// <param name="toggleOn"></param>
		/// <returns></returns>
		public InputManager ToggleActionMap(InputActionMap actionMap, bool toggleOn = false)
		{
			if (toggleOn)
				actionMap.Enable();
			else
				actionMap.Disable();
			return this;
		}
		
		/// <summary>
		/// Allows only 1 action map to work at any given moment
		/// </summary>
		public void SwitchActiveActionMap(string actionMap)
		{
			_currentActionMap?.Disable();
			(_prevActionMap, _currentActionMap) = (_currentActionMap, _inputActionAssets[_activeActionType].FindActionMap(actionMap));
			_currentActionMap?.Enable();
		}

		/// <summary>
		/// Allows only 1 action map to work at any given moment, switches current action map to previous one
		/// </summary>
		public void SwitchActiveActionMapToPrev()
		{
			_currentActionMap?.Disable();
			(_prevActionMap, _currentActionMap) = (_currentActionMap, _prevActionMap);
			_currentActionMap?.Enable();
		}
	}
}