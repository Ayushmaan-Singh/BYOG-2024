using UnityEngine;
using UnityEngine.Events;
namespace AstekUtility.EventArchitecture
{
	public class GameEventListener : MonoBehaviour
	{
		[Tooltip("Event to register with.")]
		public GameEvent_SO Event;

		[Tooltip("Response to invoke when Event is raised.")]
		public UnityEvent Response;

		private void OnEnable()
		{
			Event.RegisterListener(this);
		}

		private void OnDisable()
		{
			Event.UnregisterListener(this);
		}

		public void OnEventRaised()
		{
			Response.Invoke();
		}
	}
}