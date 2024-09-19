using System.Collections;
using UnityEngine;
namespace AstekUtility
{
	#region Coroutine Task Manager

    /// <summary>
    ///     Used if we want the live status of _coroutine to make some decision i.e _running ,_paused, completed
    ///     This one is for simple _coroutine which do not return any data
    /// </summary>
    public class CoroutineTask
	{

        /// Delegate for termination event subscribers.  
        /// manual is true if the _coroutine was _stopped with an explicit call to Stop().
        public delegate void FinishedHandler(bool manual);
		private readonly MonoBehaviour _owner;
		private TaskState _task;
		private readonly IEnumerable _taskIEnumerable;

        /// <summary>
        ///     If autostart==true then start the _coroutine execution Immediately
        ///     else then you have to manually start it using start
        /// </summary>
        /// <param name="task"></param>
        /// <param name="autoStart"></param>
        public CoroutineTask(IEnumerable task, MonoBehaviour owner, bool autoStart = true)
		{
			_taskIEnumerable = task;
			_owner = owner;

			if (autoStart)
				Start();
		}

        /// Returns true if _coroutine is _running.  
        /// Paused tasks are considered to be _running.
        public bool Running
		{
			get
			{
				if (_task != null)
					return _task.Running;
				return false;
			}
		}

		//Returns true if the _coroutine is pasued
		public bool Paused
		{
			get
			{
				if (_task != null)
					return _task.Paused;
				return false;
			}
		}

		//Termination event ,triggered when _coroutine is completely executed
		public event FinishedHandler Finished;

		//Start execution of _coroutine
		public void Start()
		{
			CreateTask();
			_task.Start();
		}

		//Stop the execution of _coroutine
		public void Stop()
		{
			_task.Stop();
		}

		public void Pause()
		{
			_task.Pause();
		}

		public void Unpause()
		{
			_task.Unpause();
		}

		public void Restart()
		{
			_task.Stop();
			CreateTask();
			_task.Start();
		}

		public void TaskFinished(bool manual)
		{
			FinishedHandler handler = Finished;
			if (handler != null)
				handler(manual);
		}

		private void CreateTask()
		{
			_task = new TaskState(_taskIEnumerable.GetEnumerator(), _owner);
			_task.Finished += TaskFinished;
		}

		//Task State Class
		private class TaskState
		{

			public delegate void FinishedHandler(bool manual);

			private readonly IEnumerator _coroutine;
			private readonly MonoBehaviour _owner;

			private bool _stopped;

			public TaskState(IEnumerator coroutineTask, MonoBehaviour owner)
			{
				_coroutine = coroutineTask;
				_owner = owner;
			}

			public bool Running { get; private set; }

			public bool Paused { get; private set; }
			public event FinishedHandler Finished;
			public void Pause()
			{
				Paused = true;
			}

			public void Unpause()
			{
				Paused = false;
			}

			public void Start()
			{
				Running = true;
				_owner.StartCoroutine(CallWrapper());
			}

			public void Stop()
			{
				_stopped = true;
				Running = false;
			}

			private IEnumerator CallWrapper()
			{
				yield return null;
				IEnumerator coroutineTask = _coroutine;

				while (Running)
				{
					if (Paused)
						yield return null;
					else
					{
						if (coroutineTask != null && coroutineTask.MoveNext())
						{
							yield return coroutineTask.Current;
						}
						else
						{
							Running = false;
						}
					}
				}
				FinishedHandler handler = Finished;
				if (handler != null)
					handler(_stopped);
			}
		}
	}

	#endregion
}