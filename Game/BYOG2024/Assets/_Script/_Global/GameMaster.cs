using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Input;
using AstekUtility.SceneManagement;
using UnityEngine;

namespace Global
{
	public class GameMaster : MonoBehaviour
	{
		[SerializeField] private CameraRuntimeSet cameraRTSet;
		[SerializeField] private Camera cam;
		private InputUtils mousePos;

		private void Awake()
		{
			cameraRTSet.Add(CameraInGame.GameMasterCamera, cam.gameObject);
		}

		private void OnDestroy()
		{
			cameraRTSet.Remove(CameraInGame.GameMasterCamera);
		}

		private async void Start()
		{
			await ServiceLocator.Global.Get<SceneLoader>().LoadSceneGroup(0);
		}

		private void FixedUpdate()
		{
			if (cameraRTSet.ContainsKey(CameraInGame.MainCamera))
				ServiceLocator.Global.Get<InputUtils.UpdateMousePos>()?.Invoke(cameraRTSet[CameraInGame.MainCamera].GetComponentInChildren<Camera>());
			else
				ServiceLocator.Global.Get<InputUtils.UpdateMousePos>()?.Invoke(cameraRTSet[CameraInGame.GameMasterCamera].GetComponentInChildren<Camera>());
		}
	}
}