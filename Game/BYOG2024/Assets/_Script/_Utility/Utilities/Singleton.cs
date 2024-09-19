using UnityEngine;
namespace AstekUtility
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		protected static T _instance;
		public static bool HasInstance
		{
			get
			{
				return _instance != null;
			}
		}
		public static T TryGetInstance
		{
			get
			{
				return HasInstance ? _instance : null;
			}
		}
		public static T Current
		{
			get
			{
				return _instance;
			}
		}

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindFirstObjectByType<T>();
					if (_instance == null)
					{
						GameObject obj = new GameObject();
						obj.name = $"{typeof(T).Name}_AutoCreated";
						_instance = obj.AddComponent<T>();
					}
				}

				return _instance;
			}
		}

		protected virtual void Awake()
		{
			InitializeSingleton();
		}

		protected virtual void InitializeSingleton()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			_instance = this as T;
		}
	}
}