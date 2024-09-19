using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.XR;
namespace AstekUtility.Gameplay
{
	public static class PlayerLoopUtils
	{
		//Remove a system from player loop
		public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
		{
			if (loop.subSystemList == null) return;

			List<PlayerLoopSystem> playerLoopSystemsList = new List<PlayerLoopSystem>(loop.subSystemList);
			for (int i = 0; i < playerLoopSystemsList.Count; i++)
			{
				if (playerLoopSystemsList[i].type == systemToRemove.type && playerLoopSystemsList[i].updateDelegate == systemToRemove.updateDelegate)
				{
					playerLoopSystemsList.RemoveAt(i);
					loop.subSystemList = playerLoopSystemsList.ToArray();
				}
			}

			HandleSubSystemLoopForRemoval<T>(ref loop, systemToRemove);
		}

		private static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
		{
			if (loop.subSystemList == null) return;

			for (int i = 0; i < loop.subSystemList.Length; i++)
			{
				RemoveSystem<T>(ref loop.subSystemList[i], systemToRemove);
			}
		}

		//Insert a system into the player loop
		public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
		{
			if (loop.type != typeof(T)) return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);

			List<PlayerLoopSystem> playerLoopSystemsList = new List<PlayerLoopSystem>();
			if (loop.subSystemList != null) playerLoopSystemsList.AddRange(loop.subSystemList);
			playerLoopSystemsList.Insert(index, systemToInsert);
			loop.subSystemList = playerLoopSystemsList.ToArray();
			return true;
		}

		private static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
		{
			if (loop.subSystemList == null) return false;

			for (int i = 0; i < loop.subSystemList.Length; i++)
			{
				if (!InsertSystem<T>(ref loop.subSystemList[i], in systemToInsert, index)) continue;
				return true;
			}

			return false;
		}

		#if UNITY_EDITOR
		public static void PrintPlayerLoop(PlayerLoopSystem loop)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Unity Player Loop");
			foreach (PlayerLoopSystem subsystem in loop.subSystemList)
			{
				PrintSubsystem(subsystem, sb, 0);
			}
			Debug.Log(sb.ToString());
		}

		private static void PrintSubsystem(PlayerLoopSystem system, StringBuilder sb, int level)
		{
			sb.Append(' ', level = 2).AppendLine(system.type.ToString());
			if (system.subSystemList == null || system.subSystemList.Length == 0) return;

			foreach (PlayerLoopSystem subsystem in system.subSystemList)
			{
				PrintSubsystem(subsystem, sb, level + 1);
			}
		}
		#endif
	}
}