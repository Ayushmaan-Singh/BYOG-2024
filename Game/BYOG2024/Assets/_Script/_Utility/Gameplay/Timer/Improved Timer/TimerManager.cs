using System.Collections.Generic;

namespace AstekUtility.Gameplay.ImprovedTimer
{
	public abstract class TimerManager
	{
		private static readonly List<Timer> _TIMERS = new List<Timer>();

		public static void RegisterTimer(Timer timer) => _TIMERS.Add(timer);
		public static void DeregisterTimer(Timer timer) => _TIMERS.Remove(timer);

		public static void UpdateTimers()
		{
			foreach (Timer timer in new List<Timer>(_TIMERS))
			{
				timer.Tick();
			}
		}

		public static void Clear() => _TIMERS.Clear();
	}
}