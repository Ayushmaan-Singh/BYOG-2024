using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


namespace AstekUtility.SceneManagement
{
	public class SceneGroupManager
	{
		public event Action<string> OnSceneLoaded = delegate { };
		public event Action<string> OnSceneUnloaded = delegate { };
		public event Action OnSceneGroupLoaded = delegate { };

		private readonly AsyncOperationHandleGroup handleGroup = new AsyncOperationHandleGroup(10);

		private SceneGroup ActiveSceneGroup;

		/// <summary>
		/// Used for loading scene groups
		/// ,Other than Bootstrap scene all the scenes will be loaded Async
		/// </summary>
		/// <param name="group">Scene group we are loading</param>
		/// <param name="progress"> Progress of loading</param>
		/// <param name="reloadDupScenes"> In case a scene is already Loaded should we reload it or not.
		/// Setting it to true will allow duplicate</param>
		public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
		{
			ActiveSceneGroup = group;
			List<string> loadedScenes = new List<string>();

			await UnloadScenes();
			int sceneCount = SceneManager.sceneCount;

			for (int i = 0; i < sceneCount; i++)
			{
				loadedScenes.Add(SceneManager.GetSceneAt(i).name);
			}

			int totalScenesToLoad = ActiveSceneGroup.Scenes.Count;

			AsyncOperationGroup operationGroup = new AsyncOperationGroup(totalScenesToLoad);

			for (int i = 0; i < totalScenesToLoad; i++)
			{
				SceneData sceneData = group.Scenes[i];
				if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name)) continue;

				switch (sceneData.Reference.State)
				{
					case SceneReferenceState.Regular:
						AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
						operationGroup.Operations.Add(operation);
						break;

					case SceneReferenceState.Addressable:
						AsyncOperationHandle<SceneInstance> sceneHandle = Addressables.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
						handleGroup.Handles.Add(sceneHandle);
						break;
				}

				OnSceneLoaded.Invoke(sceneData.Name);
			}

			while (!operationGroup.IsDone || !handleGroup.IsDone)
			{
				progress?.Report((operationGroup.Progress + handleGroup.Progress) / 2);
				await Task.Delay(100); //Delay To avoid tight loop
			}

			Scene activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

			if (activeScene.IsValid())
			{
				SceneManager.SetActiveScene(activeScene);
			}

			OnSceneGroupLoaded.Invoke();
		}

		public async Task UnloadScenes()
		{
			List<string> scenes = new List<string>();
			string activeScene = SceneManager.GetActiveScene().name;

			int sceneCount = SceneManager.sceneCount;

			for (int i = 0; i < sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (!sceneAt.isLoaded) continue;
				string sceneName = sceneAt.name;
				if (sceneName.Equals(activeScene) || sceneName == "GameMaster"
				                                  || handleGroup.Handles.Any(h => h.IsValid() && h.Result.Scene.name == sceneName)) continue;
			}

			AsyncOperationGroup operationGroup = new AsyncOperationGroup(scenes.Count);

			foreach (string scene in scenes)
			{
				AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
				if (operation == null) continue;

				operationGroup.Operations.Add(operation);

				OnSceneUnloaded.Invoke(scene);
			}

			foreach (AsyncOperationHandle<SceneInstance> handle in handleGroup.Handles)
			{
				if (handle.IsValid())
				{
					Addressables.UnloadSceneAsync(handle);
				}
			}
			handleGroup.Handles.Clear();

			while (!operationGroup.IsDone)
			{
				await Task.Delay(100); //Delay To avoid tight loop
			}
		}

		public readonly struct AsyncOperationGroup
		{
			public readonly List<AsyncOperation> Operations;

			public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
			public bool IsDone => Operations.Count == 0 || Operations.All(o => o.isDone);

			public AsyncOperationGroup(int initCapacity)
			{
				Operations = new List<AsyncOperation>(initCapacity);
			}
		}

		/// <summary>
		/// This ios for working with addressable system
		/// </summary>
		public readonly struct AsyncOperationHandleGroup
		{
			public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

			public float Progress => Handles.Count == 0 ? 0 : Handles.Average(h => h.PercentComplete);
			public bool IsDone => Handles.Count == 0 || Handles.All(h => h.IsDone);

			public AsyncOperationHandleGroup(int initialCapacity)
			{
				Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
			}
		}
	}
}