using System;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Entity
{
	[RequireComponent(typeof(Animator))]
	public class EntityAnimationController : MonoBehaviour
	{
		[ShowInInspector, EnableIf("@false")] private readonly Dictionary<string, Action> animationEventCollection = new Dictionary<string, Action>();

		private Animator _animator;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		private void OnEnable()
		{
			ServiceLocator.For(this).Register(this);
			_animator.enabled = true;
		}

		private void OnDisable()
		{
			ServiceLocator.For(this)?.Deregister(this);
			_animator.enabled = false;
		}

		#region Animation Event

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

		#endregion

		public void CrossFade(int toStateId, int layer = 0, float? normalizedTimeDuration = null, float? normalizedTimeOffset = null,
			float? fixedTimeDuration = null, float? fixedTimeOffset = null)
		{
			if (normalizedTimeDuration != null && normalizedTimeOffset != null)
				_animator.CrossFade(toStateId, (float)normalizedTimeDuration, layer, (float)normalizedTimeOffset);
			else
				_animator.CrossFadeInFixedTime(toStateId, fixedTimeDuration ?? 0, layer, fixedTimeOffset ?? 0);
		}

		public void SetWeight(int index, float amount) => _animator.SetLayerWeight(index, amount);
	}
}