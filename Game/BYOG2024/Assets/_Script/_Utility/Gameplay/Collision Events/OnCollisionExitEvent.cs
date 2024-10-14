using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
	public class OnCollisionExitEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent<Collision> onCollisionExit;

		private void OnCollisionExit(Collision other)
		{
			onCollisionExit?.Invoke(other);
		}

		public void Register(UnityAction<Collision> action)
		{
			onCollisionExit.AddListener(action);
		}
		public void Deregister(UnityAction<Collision> action)
		{
			onCollisionExit.AddListener(action);
		}
	}
}