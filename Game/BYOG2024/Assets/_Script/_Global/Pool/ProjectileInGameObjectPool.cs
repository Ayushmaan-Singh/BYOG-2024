using AstekUtility.DesignPattern.ServiceLocatorTool;

namespace Global.Pool
{
	public class ProjectileInGameObjectPool : InGameObjectPoolBase
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