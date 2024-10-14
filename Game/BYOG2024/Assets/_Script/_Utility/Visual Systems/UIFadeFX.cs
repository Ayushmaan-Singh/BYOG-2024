using System;
using AstekUtility.Gameplay.Timer;
using UnityEngine;
namespace AstekUtility.VisualFeedback
{
	public class UIFadeFX : VisualFX
	{
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private float initialAlpha;
		[SerializeField] private float finalAlpha;

		[SerializeField] private float time;
		[SerializeField] private AnimationCurve animCurve;

		[SerializeField] private bool runAtTimeScaleZero = true;

		private CountdownTimer _timer;
		private float? _startAt;
		public float Progress => _timer.Progress;

		private void Awake()
		{
			_timer = new CountdownTimer(time);
		}

		private void Update()
		{
			if (!_timer.IsRunning)
				return;

			_timer.Tick(runAtTimeScaleZero ? Time.unscaledDeltaTime : Time.deltaTime);
			canvasGroup.alpha = (Mathf.Lerp(initialAlpha, finalAlpha, Progress));
		}

		public UIFadeFX StartAt(float startAtTime)
		{
			_startAt = startAtTime;
			return this;
		}

		public override void Play()
		{
			_timer.Time = _startAt ?? 0;
			_startAt = null;
			_timer.Start();
		}

		public override void Stop()
		{
			_timer.Stop();
		}

		public UIFadeFX ResetFade()
		{
			_timer.Stop();
			canvasGroup.alpha = (initialAlpha);
			return this;
		}
	}
}