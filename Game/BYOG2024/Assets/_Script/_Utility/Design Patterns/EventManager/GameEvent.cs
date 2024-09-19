namespace AstekUtility.DesignPattern.EventSystem
{
	// Base class for all types of Events
	public class GameEvent
	{
#if UNITY_EDITOR
		private bool isTracebale = true;
#endif
		protected void StopTracing()
		{
#if UNITY_EDITOR
			isTracebale = false;
#endif
		}

		public bool IsTracable()
		{
#if UNITY_EDITOR
			return isTracebale;
#else
            return false;
#endif
		}
	}
}