using AstekUtility.Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace AstekUtility.Gameplay.ImprovedTimer
{
	public static class TimerBootstrapper
	{
		private static PlayerLoopSystem _TIMERSYSTEM;

		#if UNITY_EDITOR
		private static bool SHOWPLAYERLOOP = true;
        #endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		internal static void Initialize()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

			if (!InsertTimerManager<Update>(ref currentPlayerLoop,0))
			{
				Debug.LogWarning("Improved Timer not initialized, unable to register Timermanager into the update loop");
				return;
			}
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
			
			#if UNITY_EDITOR
			
			if (SHOWPLAYERLOOP)
				PlayerLoopUtils.PrintPlayerLoop(currentPlayerLoop);

			EditorApplication.playModeStateChanged -= OnPlayModeState;
			EditorApplication.playModeStateChanged += OnPlayModeState;
			
			static void OnPlayModeState(PlayModeStateChange state)
			{
				if (state == PlayModeStateChange.ExitingPlayMode)
				{
					PlayerLoopSystem currentPlayerLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
					RemoveTimerManager<Update>(ref currentPlayerLoopSystem);
					PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
					TimerManager.Clear();
				}
			}
			#endif
		}
		
		public static void RemoveTimerManager<T>(ref PlayerLoopSystem loop)
		{
			PlayerLoopUtils.RemoveSystem<T>(ref loop, in _TIMERSYSTEM);
		}

		public static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index)
		{
			_TIMERSYSTEM = new PlayerLoopSystem()
			{
				type = typeof(TimerManager),
				updateDelegate = TimerManager.UpdateTimers,
				subSystemList = null
			};

			return PlayerLoopUtils.InsertSystem<T>(ref loop, in _TIMERSYSTEM, index);
		}
	}

}