using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;

namespace AstekUtility.SceneManagement
{
	[System.Serializable]
	public class SceneGroup
	{
		public string GroupName = "New Scene Group";
		public List<SceneData> Scenes;

		public string FindSceneNameByType(SceneType sceneType)
		{
			return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType)?.Reference.Name;
		}
	}

	[System.Serializable]
	public class SceneData
	{
		public SceneReference Reference;
		public string Name => Reference.Name;
		public SceneType SceneType;
	}

	public enum SceneType
	{
		ActiveScene = 0,
		MainMenu = 1,
		UI = 2,
		HUD = 3,
		Cinematic = 4,
		Environment = 5,
		Tooling = 6
	}
}