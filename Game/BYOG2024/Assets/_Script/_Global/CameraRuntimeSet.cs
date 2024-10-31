using System.Collections.Generic;
using AstekUtility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Global
{
	public class CameraRuntimeSet : ScriptableObject
	{
		[ShowInInspector, EnableIf("@false")] private readonly Dictionary<CameraInGame, GameObject> _rtSet = new Dictionary<CameraInGame, GameObject>();

		public GameObject this[CameraInGame key]
		{
			get
			{
				List<CameraInGame> cameras = _rtSet.Keys.Where(x => !_rtSet[x].OrNull()).ToList();
				foreach (CameraInGame camera in cameras)
					_rtSet.Remove(camera);
				return _rtSet[key];
			}

			set
			{
				_rtSet[key] = value;
			}
		}

		public bool ContainsKey(CameraInGame key) => _rtSet.ContainsKey(key);

		public CameraRuntimeSet Add(CameraInGame key, GameObject obj)
		{
			_rtSet.Add(key, obj);
			return this;
		}

		public CameraRuntimeSet Remove(CameraInGame key)
		{
			_rtSet.Remove(key);
			return this;
		}
	}

	public enum CameraInGame
	{
		MainCamera,
		GameMasterCamera
	}
}