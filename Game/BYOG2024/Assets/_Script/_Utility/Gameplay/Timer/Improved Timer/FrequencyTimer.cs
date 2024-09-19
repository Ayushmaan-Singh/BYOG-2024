using System;
using UnityEngine;

namespace AstekUtility.Gameplay.ImprovedTimer
{
	/// <summary>
	/// Timer that ticks at a specific frequency. (N times per second)
	/// </summary>
	public class FrequencyTimer : Timer
	{
		public int TicksPerSecond { get; private set; }

		public Action OnTick = delegate { };

		public float TimeThreshold;

		public FrequencyTimer(int ticksPerSecond) : base(0)
		{
			CalculateTimeThreshold(ticksPerSecond);
		}

		public override void Tick()
		{
			if (IsRunning && CurrentTime >= TimeThreshold)
			{
				CurrentTime -= TimeThreshold;
				OnTick.Invoke();
			}

			if (IsRunning && CurrentTime < TimeThreshold)
			{
				CurrentTime += Time.deltaTime;
			}
		}

		public override bool IsFinished => !IsRunning;

		public override void Reset()
		{
			CurrentTime = 0;
		}

		public void Reset(int newTicksPerSecond)
		{
			CalculateTimeThreshold(newTicksPerSecond);
			Reset();
		}

		void CalculateTimeThreshold(int ticksPerSecond)
		{
			TicksPerSecond = ticksPerSecond;
			TimeThreshold = 1f / TicksPerSecond;
		}
	}
}