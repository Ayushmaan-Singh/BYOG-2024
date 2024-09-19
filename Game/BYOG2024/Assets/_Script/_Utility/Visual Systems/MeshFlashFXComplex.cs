using Sirenix.OdinInspector;
using UnityEngine;
namespace AstekUtility.VisualFeedback
{
	/// <summary>
	/// Used By Models that require multiple mesh flash fx due to multiple parts
	/// </summary>
	public class MeshFlashFXComplex : VisualFX
	{
		[SerializeField] private MeshFlashFX[] meshFlash;


		[Button]
		public override void Play()
		{
			foreach (MeshFlashFX flash in meshFlash)
			{
				flash.Play();
			}
		}
		public override void Stop()
		{
			foreach (MeshFlashFX flash in meshFlash)
			{
				flash.Stop();
			}
		}
	}
}