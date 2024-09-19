using System;
using System.Threading.Tasks;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace AstekUtility.SceneManagement
{
	public class SceneLoader : MonoBehaviour
	{
		[SerializeField] private Image loadingBar;
		[SerializeField] private float fillSpeed = 0.5f;
		[SerializeField] private Canvas loadingCanvas;
		[SerializeField] private Camera loadingCamera;
		[SerializeField] private SceneGroup[] sceneGroups;

		private float targetProgress;
		private bool isLoading;

		public readonly SceneGroupManager manager = new SceneGroupManager();

		private void Awake()
		{
			ServiceLocator.Global.Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.Global?.Deregister(this);
		}

		private void Update()
		{
			if (!isLoading) return;

			float currentFillAmount = loadingBar.fillAmount;
			float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

			float dynamicFillSpeed = progressDifference * fillSpeed;

			loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
		}

		/// <summary>
		/// Modify if we require a whole another scene for loading
		/// </summary>
		/// <param name="index"></param>
		public async Task LoadSceneGroup(int index)
		{
			loadingBar.fillAmount = 0;
			targetProgress = 1f;

			if (index < 0 || index >= sceneGroups.Length)
			{
				Debug.LogError($"Invalid Scene Group Index: {index}");
				return;
			}

			LoadingProgress progress = new LoadingProgress();
			progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

			EnableLoadingCanvas(true);
			await manager.LoadScenes(sceneGroups[index], progress);
			EnableLoadingCanvas(false);
		}

		private void EnableLoadingCanvas(bool enable = true)
		{
			isLoading = enable;
			loadingCanvas.gameObject.SetActive(enable);
			loadingCamera.gameObject.SetActive(enable);
		}
	}

	public class LoadingProgress : IProgress<float>
	{
		public event Action<float> Progressed;

		private const float ratio = 1f;

		public void Report(float value)
		{
			Progressed?.Invoke(value / ratio);
		}
	}
}