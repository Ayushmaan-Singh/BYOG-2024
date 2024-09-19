using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace AstekUtility.Input
{
	public class InputUtils : MonoBehaviour
	{
		public delegate Vector3 MousePosition();
		public delegate void UpdateMousePos(Camera camera);

		public MousePosition GetMousePosition { get; private set; }
		public UpdateMousePos UpdateMousePosition { get; private set; }

		private Vector3 _currentMousePos;

		public void Awake()
		{
			GetMousePosition = GetCurrentMousePosition;
			UpdateMousePosition = MousePositionToXZPlane;
			
			ServiceLocator.Global.Register(GetMousePosition);
			ServiceLocator.Global.Register(UpdateMousePosition);
		}

		public void OnDestroy()
		{
			ServiceLocator.Global?.Deregister(GetMousePosition);
			ServiceLocator.Global?.Deregister(UpdateMousePosition);
		}

		private Vector3 GetCurrentMousePosition()
		{
			return _currentMousePos;
		}

		private void MousePositionToXZPlane(Camera camera)
		{
			Plane plane = new Plane(Vector3.up, 0);
			Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (plane.Raycast(ray, out float distance))
			{
				_currentMousePos = ray.GetPoint(distance);
			}
		}

		/// <summary>
		/// Not used in current game
		/// </summary>
		/// <param name="camera"></param>
		private void MousePositionToXYPlane(Camera camera)
		{
			_currentMousePos = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		}
	}
}