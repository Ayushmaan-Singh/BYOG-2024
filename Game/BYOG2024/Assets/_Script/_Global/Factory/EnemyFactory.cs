using AstekUtility.DesignPattern.ServiceLocatorTool;
using Combat.Enemy;
using Entity.Abilities;
using Global.Pool;
using UnityEngine;

namespace Global.Factory
{
	public class EnemyFactory : FactoryBase
	{
		[SerializeField] private PrefabCollection enemies;
		
		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
		}

		public override T Instantiate<T>()
		{
			if (!typeof(EnemyController).IsAssignableFrom(typeof(T)))
				return default(T);

			if (ServiceLocator.ForSceneOf(this).Get<EnemyInGameObjectPool>().TryGetPooledObject(out T projectile))
			{
				return projectile;
			}
			if (enemies.TryGet(typeof(T),out GameObject prefab))
			{
				return Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponentInChildren<T>();
			}

			Debug.LogError($"Projectile Factory: No object of type {typeof(T).Name} in projectile pool or collection");
			return null;
		}
	}
}