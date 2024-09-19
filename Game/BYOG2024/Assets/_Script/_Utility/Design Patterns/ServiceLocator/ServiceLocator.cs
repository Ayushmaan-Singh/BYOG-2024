using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AstekUtility.DesignPattern.ServiceLocatorTool
{
	public sealed class ServiceLocator : MonoBehaviour
	{
		private static ServiceLocator _GLOBAL;
		private static Dictionary<Scene, ServiceLocator> _SCENECONTAINERS;
		private static List<GameObject> _TMPSCENEGAMEOBJECTS;
		private static List<object> _CACHESERVICESOFDESTROYEDSERVICELOCATOR;

		private static bool _APPLICATIONQUIT = false;

		private readonly ServiceManager _services = new ServiceManager();

		const string k_globalServiceLocatorName = "ServiceLocator [Global]";
		const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";

		internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
		{
			if (_GLOBAL == this)
			{
				Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
			}
			else if (_GLOBAL)
			{
				Debug.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
				Destroy(gameObject);
			}
			else
			{
				_GLOBAL = this;
				if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
			}
		}

		internal void ConfigureForScene()
		{
			Scene scene = gameObject.scene;

			if (_SCENECONTAINERS.ContainsKey(scene))
			{
				Debug.LogError("ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
				Destroy(gameObject);
				return;
			}

			_SCENECONTAINERS.Add(scene, this);
		}

		/// <summary>
		/// Gets the global ServiceLocator instance. Creates new if none exists.
		/// </summary>        
		public static ServiceLocator Global
		{
			get
			{
				if (_GLOBAL) return _GLOBAL;

				if (FindFirstObjectByType<ServiceLocatorGlobal>() is { } found)
				{
					found.BootstrapOnDemand();
					return _GLOBAL;
				}

				if (!_APPLICATIONQUIT)
				{
					GameObject container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
					container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();
				}

				return _GLOBAL.OrNull();
			}
		}

		/// <summary>
		/// Returns the <see cref="ServiceLocator"/> configured for the scene of a MonoBehaviour. Falls back to the global instance.
		/// </summary>
		public static ServiceLocator ForSceneOf(MonoBehaviour mb)
		{
			Scene scene = mb.gameObject.scene;

			if (_SCENECONTAINERS.TryGetValue(scene, out ServiceLocator container) && container != mb)
			{
				return container;
			}

			_TMPSCENEGAMEOBJECTS.Clear();
			scene.GetRootGameObjects(_TMPSCENEGAMEOBJECTS);

			foreach (GameObject go in _TMPSCENEGAMEOBJECTS.Where(go => go.GetComponent<ServiceLocatorScene>()))
			{
				if (go.TryGetComponent(out ServiceLocatorScene bootstrapper) && bootstrapper.Container != mb)
				{
					bootstrapper.BootstrapOnDemand();
					return bootstrapper.Container;
				}
			}

			return Global;
		}

		/// <summary>
		/// Gets the closest ServiceLocator instance to the provided 
		/// MonoBehaviour in hierarchy, the ServiceLocator for its scene, or the global ServiceLocator.
		/// </summary>
		public static ServiceLocator For(MonoBehaviour mb)
		{
			return mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;
		}

		/// <summary>
		/// Registers a service to the ServiceLocator using the service's type.
		/// </summary>
		/// <param name="service">The service to register.</param>  
		/// <typeparam name="T">Class type of the service to be registered.</typeparam>
		/// <returns>The ServiceLocator instance after registering the service.</returns>
		public ServiceLocator Register<T>(T service) where T:class
		{
			T cachedService = GetObjectFromCache<T>();
			if (cachedService != null)
			{
				_CACHESERVICESOFDESTROYEDSERVICELOCATOR.Remove(cachedService);
			}
			
			_services.Register(service);
			return this;
		}

		/// <summary>
		/// Registers a service to the ServiceLocator using a specific type.
		/// </summary>
		/// <param name="type">The type to use for registration.</param>
		/// <param name="service">The service to register.</param>  
		/// <returns>The ServiceLocator instance after registering the service.</returns>
		public ServiceLocator Register(Type type, object service)
		{
			_services.Register(type, service);
			return this;
		}

		/// <summary>
		/// Deregisters a service to the ServiceLocator using the service's type.
		/// </summary>
		/// <param name="service">The service to register.</param>  
		/// <typeparam name="T">Class type of the service to be registered.</typeparam>
		/// <returns>The ServiceLocator instance after registering the service.</returns>
		public ServiceLocator Deregister<T>(T service) where T : class
		{
			if (_APPLICATIONQUIT == true)
				return this;

			T cachedService = GetObjectFromCache<T>();

			if (cachedService != null)
			{
				_CACHESERVICESOFDESTROYEDSERVICELOCATOR.Remove(cachedService);
				return this;
			}

			if (TryDeregister(service))
				return this;

			ServiceLocator container = null;
			TryGetNextInHierarchy(out container);
			while (container != null)
			{
				if (container.TryDeregister(service))
				{
					return this;
				}
				TryGetNextInHierarchy(out container);
			}

			Debug.LogError($"ServiceLocator.Deregister:No service of type {typeof(T).Name} is registered");
			return this;
		}

		private bool TryDeregister<T>(T service)
		{
			return _services.TryDeregister(service);
		}

		/// <summary>
		/// Gets a service of a specific type. If no service of the required type is found, an error is thrown.
		/// </summary>
		/// <param name="service">Service of type T to get.</param>  
		/// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
		/// <returns>The ServiceLocator instance after attempting to retrieve the service.</returns>
		public ServiceLocator Get<T>(out T service) where T : class
		{
			T cachedService = GetObjectFromCache<T>();

			if (cachedService != null)
			{
				Register(cachedService);
				_CACHESERVICESOFDESTROYEDSERVICELOCATOR.Remove(cachedService);
			}

			if (TryGetService(out service)) return this;

			if (TryGetNextInHierarchy(out ServiceLocator container))
			{
				container.Get(out service);
				return this;
			}

			throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
		}

		/// <summary>
		/// Allows retrieval of a service of a specific type. An error is thrown if the required service does not exist.
		/// </summary>
		/// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
		/// <returns>Instance of the service of type T.</returns>
		public T Get<T>() where T : class
		{
			T cachedService = GetObjectFromCache<T>();

			if (cachedService != null)
			{
				Register(cachedService);
				_CACHESERVICESOFDESTROYEDSERVICELOCATOR.Remove(cachedService);
			}

			Type type = typeof(T);

			if (TryGetService(type, out T service)) return service;

			if (TryGetNextInHierarchy(out ServiceLocator container))
				return container.Get<T>();

			throw new ArgumentException($"Could not resolve type '{typeof(T).FullName}'.");
		}

		private bool TryGetService<T>(out T service) where T : class
		{
			return _services.TryGet(out service);
		}

		private bool TryGetService<T>(Type type, out T service) where T : class
		{
			return _services.TryGet(out service);
		}

		private bool TryGetNextInHierarchy(out ServiceLocator container)
		{
			//Servicelocator is already global so nothing next in heirarchy
			if (this == _GLOBAL)
			{
				container = null;
			}
			//Servicelocator is scene level so we return global
			else if (_SCENECONTAINERS.ContainsValue(this))
			{
				container = Global;
			}
			//Servicelocator is local level so return next higher in heirarchy i.e superceeding servicelocator, sceneservicelocator or globalservicelocator
			else
			{
				container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
			}
			return container;
		}

		/// <summary>
		/// Checks and return if there is object of type T in cache. At the same time cleans any null in the list
		/// </summary>
		/// <returns></returns>
		private T GetObjectFromCache<T>() where T : class
		{
			_CACHESERVICESOFDESTROYEDSERVICELOCATOR = _CACHESERVICESOFDESTROYEDSERVICELOCATOR.Where(x => x != null).ToList();
			foreach (object cachedService in _CACHESERVICESOFDESTROYEDSERVICELOCATOR)
			{
				if (cachedService.GetType() == typeof(T) || typeof(T).IsAssignableFrom(cachedService.GetType()))
				{
					return cachedService as T;
				}
			}
			return null;
		}

		private void OnApplicationQuit()
		{
			_APPLICATIONQUIT = true;
		}

		private void OnDestroy()
		{
			if (_services.RegisteredServices.Count() > 0)
				_CACHESERVICESOFDESTROYEDSERVICELOCATOR.AddRange(_services.RegisteredServices);

			if (this == _GLOBAL)
			{
				_GLOBAL = null;
			}
			else if (_SCENECONTAINERS.ContainsValue(this))
			{
				_SCENECONTAINERS.Remove(gameObject.scene);
			}
		}

		// https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			_GLOBAL = null;
			_SCENECONTAINERS = new Dictionary<Scene, ServiceLocator>();
			_TMPSCENEGAMEOBJECTS = new List<GameObject>();
			_CACHESERVICESOFDESTROYEDSERVICELOCATOR = new List<object>();
			_APPLICATIONQUIT = false;
		}

#if UNITY_EDITOR
		[MenuItem("GameObject/ServiceLocator/Add Global")]
		static void AddGlobal()
		{
			GameObject go = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocatorGlobal));
		}

		[MenuItem("GameObject/ServiceLocator/Add Scene")]
		static void AddScene()
		{
			GameObject go = new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorScene));
		}
#endif
	}
}