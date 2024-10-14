using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;

namespace Global.Pool
{
	public class EnemyInGameObjectPool : InGameObjectPoolBase
	{
		private void Awake()
		{
			ServiceLocator.ForSceneOf(this).Register(this);
		}
		
		private void OnDestroy()
		{
			ServiceLocator.ForSceneOf(this)?.Deregister(this);
		}
	}
}