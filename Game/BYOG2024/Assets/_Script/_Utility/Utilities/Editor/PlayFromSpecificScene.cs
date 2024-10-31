using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AstekUtility
{
	public class PlayFromSpecificScene : EditorWindow
	{
		private static readonly string _playmodeTargetScene = "GameMaster";
		private static string _currentScene;

		[MenuItem("Developer Tools/Play From Start")]
		private static void PlayFromMainMenu()
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				return;
			}
			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; } // Save current scene if it has unsaved changes

			// Remember currently active scene
			_currentScene = SceneManager.GetActiveScene().path;

			// Start playing from the main scene
			if (EditorSceneManager.OpenScene(GetScenePath(_playmodeTargetScene), OpenSceneMode.Single).IsValid())
			{
				EditorApplication.isPlaying = true;
				EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			}
			else { Debug.LogError("Scene not found: " + _playmodeTargetScene); }
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredEditMode)
			{
				if (!string.IsNullOrEmpty(_currentScene)) { EditorSceneManager.OpenScene(_currentScene); }
				EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			}
		}

		private static string GetScenePath(string sceneName)
		{
			foreach (var scene in EditorBuildSettings.scenes)
			{
				if (scene.path.Contains(sceneName)) { return scene.path; }
			}
			return null;
		}
	}
}