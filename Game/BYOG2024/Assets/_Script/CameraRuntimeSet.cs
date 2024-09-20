using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global
{
	public class CameraRuntimeSet : ScriptableObject
	{
		private Dictionary<CameraInGame, GameObject> _rtSet = new Dictionary<CameraInGame, GameObject>();

		public GameObject this[CameraInGame key]
		{
			get
			{
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
			_rtSet.Add(key,obj);
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