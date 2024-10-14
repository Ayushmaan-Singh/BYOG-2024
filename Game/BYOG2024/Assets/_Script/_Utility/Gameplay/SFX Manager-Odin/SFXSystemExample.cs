#if ODIN_INSPECTOR && UNITY_EDITOR

using System;
using UnityEngine;
namespace AstekUtility.Odin.SFX
{
	public class SFXSystemExample : MonoBehaviour
	{
		public SFX sfxToPlay;

		public void Start()
		{
			sfxToPlay.PlaySFX();
		}
	}
}

#endif