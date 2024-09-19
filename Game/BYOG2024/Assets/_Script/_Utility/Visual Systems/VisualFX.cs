using UnityEngine;

namespace AstekUtility.VisualFeedback
{
	public abstract class VisualFX : MonoBehaviour
	{
		public abstract void Play();
		public abstract void Stop();
	}
}