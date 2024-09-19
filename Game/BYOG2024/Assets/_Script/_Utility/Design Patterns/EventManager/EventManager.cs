using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.DesignPattern.EventSystem
{
    /// <summary>
    ///     Manages all types of Game Events.
    ///     All the communications should be from this class across the layers
    /// </summary>
    public class EventManager
	{

		public delegate void EventDelegate<T>(T e) where T : GameEvent;
		//Class instance
		private static EventManager instance;
		private static bool isInstanceDestroyed;
		private readonly Dictionary<Delegate, EventDelegate> delegateLookup = new Dictionary<Delegate, EventDelegate>();

		private readonly Dictionary<Type, EventDelegate> delegates = new Dictionary<Type, EventDelegate>();

		//Queue of events that has to run 
		private readonly Queue eventsInQueue = new Queue();
		//Only to be run once
		private readonly Dictionary<Delegate, bool> onceLookups = new Dictionary<Delegate, bool>();

		//To limit queue processing via queueProcessingTime or not
		public bool limitQueueProcessing = false;
		//Amount of duration allowed to process event in a single frame period
		public int queueProcessingTime = 20;

		public static EventManager Instance
		{
			get
			{
				if (!isInstanceDestroyed && instance == null)
				{
					instance = new EventManager();
				}
				return instance;
			}
		}

		public bool IsInstanceDestroyed
		{
			get { return isInstanceDestroyed; }
			set { isInstanceDestroyed = value; }
		}

		//Add delegate to an event
		private EventDelegate AddDelegate<T>(EventDelegate<T> eventDel) where T : GameEvent
		{
			if (delegateLookup.ContainsKey(eventDel))
				return null;

			//This is the one that gets involked
			EventDelegate internalDelegate = e => eventDel((T)e);
			delegateLookup[eventDel] = internalDelegate;

			EventDelegate tempDel = null;
			if (delegates.TryGetValue(typeof(T), out tempDel))
			{
				delegates[typeof(T)] = tempDel += internalDelegate;
			}
			else
			{
				delegates[typeof(T)] = internalDelegate;
			}

			return internalDelegate;
		}

		//Queues events to be run synchronously
		public bool QueueEvent(GameEvent gEvent)
		{
			if (!delegates.ContainsKey(gEvent.GetType()))
			{
				Debug.LogWarning($"EventManager: QueueEvent failed due to no listeners for event: {gEvent.GetType()}");
				return false;
			}

			eventsInQueue.Enqueue(gEvent);
			return true;
		}

        /// <summary>
        ///     Every update cycle the queue is processed, if the queue processing is limited,
        ///     a maximum processing time per update can be set after which the events will have
        ///     to be processed next update loop.
        /// </summary>
        public void Update()
		{
			if (eventsInQueue.Count > 0)
			{
				//Handling one event per frame
				GameEvent evt = eventsInQueue.Dequeue() as GameEvent;
				TriggerEvent(evt);
			}
		}

		//Clears all events and queues
		public void Release()
		{
			RemoveAll();
			eventsInQueue.Clear();
			isInstanceDestroyed = true;
		}
#if UNITY_EDITOR

        /// <summary>
        ///     This is used so that static variables are refreshed once the play mode is over when having domain reload disabled
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		public static void ResetStatic()
		{
			instance = null;
		}

#endif
		private delegate void EventDelegate(GameEvent e);

		#region Subscribing and Unsubscribing Listeners + checking listener

		//Registering Listener
		public void AddListener<T>(EventDelegate<T> eventDel) where T : GameEvent
		{
			AddDelegate(eventDel);
		}

		//Add Listener internal function
		private void AddListenerOnce<T>(EventDelegate<T> eventDel) where T : GameEvent
		{
			EventDelegate result = AddDelegate(eventDel);

			if (result != null)
			{
				//Only called once
				onceLookups[result] = true;
			}
		}

		//UnregisterListener Listener
		public void RemoveListener<T>(EventDelegate<T> eventDel) where T : GameEvent
		{
			if (delegateLookup.TryGetValue(eventDel, out EventDelegate internalDelegate))
			{
				if (delegates.TryGetValue(typeof(T), out EventDelegate tempDelegate))
				{
					tempDelegate -= internalDelegate;
					if (tempDelegate == null)
					{
						delegates.Remove(typeof(T));
					}
					else
					{
						delegates[typeof(T)] = tempDelegate;
					}
				}
				delegateLookup.Remove(eventDel);
			}
		}

		//Remove all listeners
		private void RemoveAll()
		{
			delegates.Clear();
			delegateLookup.Clear();
			onceLookups.Clear();
		}

		public bool HasListeners<T>(EventDelegate<T> eventDel) where T : GameEvent
		{
			return delegateLookup.ContainsKey(eventDel);
		}

		public void TriggerEvent(GameEvent gEvent)
		{
			EventDelegate eventDel;
			if (delegates.TryGetValue(gEvent.GetType(), out eventDel))
			{
				eventDel.Invoke(gEvent);

#if LOG_EVENTS
          if(e.IsTracable())
            Log.Print("Event : " + e + "", LogFilter.GameEvent);
#endif

				//Remove Listeners which should only be called once
				foreach (EventDelegate eventD in delegates[gEvent.GetType()].GetInvocationList())
				{
					if (onceLookups.ContainsKey(eventD))
					{
						onceLookups.Remove(eventD);
					}
				}
			}
			else
			{
				Debug.LogError($"Event {gEvent.GetType()} has no Listener");
			}
		}

		#endregion
	}
}