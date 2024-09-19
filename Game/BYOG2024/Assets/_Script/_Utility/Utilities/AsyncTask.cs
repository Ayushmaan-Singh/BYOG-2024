using System;
using System.Threading;
using System.Threading.Tasks;
namespace AstekUtility
{
	public class AsyncTask
	{
		private Func<CancellationToken, Task> _cachedFunction;
		private CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly object _lockObject = new object();
		public bool IsTaskPaused { get; private set; }

		public void CacheFunction(Func<CancellationToken, Task> asyncFunction)
		{
			_cachedFunction = asyncFunction;
		}

		public async void RunCachedFunction(Action taskCancelledException = null)
		{
			if (_cachedFunction == null)
			{
				throw new InvalidOperationException("No function cached.");
			}

			lock (_lockObject)
			{
				_cts = new CancellationTokenSource();
				IsTaskPaused = false;
			}

			try
			{
				await _cachedFunction(_cts.Token);
			}
			catch (TaskCanceledException)
			{
				taskCancelledException?.Invoke();
			}
		}

		public void PauseCachedFunction()
		{
			lock (_lockObject)
			{
				IsTaskPaused = true;
			}
		}

		public void ResumeCachedFunction()
		{
			lock (_lockObject)
			{
				if (_cachedFunction != null)
				{
					IsTaskPaused = false;
				}
				else
				{
					throw new InvalidOperationException("No function cached.");
				}
			}
		}

		public void CancelCachedFunction()
		{
			_cts.Cancel();
			_cts = new CancellationTokenSource();
			IsTaskPaused = false;
		}
	}
}