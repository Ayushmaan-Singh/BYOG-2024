using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AstekUtility.VisualFeedback
{
	public class HitPause : Singleton<HitPause>
	{
		[SerializeField, Tooltip("In Milli Seconds")] private int defaultHitPauseTime = 66;
		private CancellationTokenSource _cts;

		public void ExecuteHitPause(float? timeInSec = null, int? timeInMilliSec = null)
		{
			_cts = new CancellationTokenSource();
			int time = defaultHitPauseTime;

			if (timeInMilliSec != null && timeInMilliSec != 0)
				time = (int)timeInMilliSec;
			else if (timeInSec != null && timeInSec != 0)
				time = Mathf.RoundToInt((float)timeInSec * 1000);

			Pause(time);
		}

		public void CancelHitPause()
		{
			_cts.Cancel();
		}

		private async void Pause(int timeInMilliSec)
		{
			Time.timeScale = 0;
			try
			{
				await Task.Delay(timeInMilliSec, _cts.Token);
			}
			finally
			{
				Time.timeScale = 1;
			}
		}
	}
}