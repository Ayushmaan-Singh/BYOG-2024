using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Entity
{
	public class EntityAnimationController : SerializedMonoBehaviour
	{
		[ShowInInspector] private readonly Dictionary<string, Action> animationEventCollection;

		public Action this[string key]
		{
			get
			{
				return animationEventCollection[key];
			}
			set
			{
				animationEventCollection[key] = value;
			}
		}

		public void RegisterAnimEvent(string key, Action action)
		{
			if (!animationEventCollection.ContainsKey(key))
				animationEventCollection.Add(key, delegate { });

			animationEventCollection[key] += action;
		}
		public void DeregisterAnimEvent(string key, Action action)
		{
			if (!animationEventCollection.ContainsKey(key))
				return;

			animationEventCollection[key] -= action;
			if (animationEventCollection[key].GetInvocationList().Length <= 1)
				animationEventCollection.Remove(key);
		}

		public void ExecuteAnimationEvent(string key)
		{
			animationEventCollection[key].Invoke();
		}
	}
}