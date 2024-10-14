#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace AstekUtility.Odin.SFX
{

	public class SFXClip : ScriptableObject
	{
		[Title("Audio Clip")]
		[Space, Required]
		public AudioClip clip;

		[Title("Clip Settings")]
		public float volume = 1f;
		[Tooltip("Randomness amount")] public float volumeVariation = 0.05f;
		public float pitch = 1f;
		[Tooltip("Randomness amount")] public float pitchVariation = 0.05f;
	}
}
#endif