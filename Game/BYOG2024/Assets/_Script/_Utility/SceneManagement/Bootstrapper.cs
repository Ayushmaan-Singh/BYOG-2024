using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace AstekUtility.SceneManagement
{
	public class Bootstrapper : PersistentSingleton<Bootstrapper>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static async void Init()
		{
			Debug.Log("Bootstrapper........");
			if (SceneManager.GetActiveScene().name != "GameMaster")
				await Addressables.LoadSceneAsync("Assets/_Scenes/GameMaster.unity", LoadSceneMode.Single).Task;
		}
	}
}