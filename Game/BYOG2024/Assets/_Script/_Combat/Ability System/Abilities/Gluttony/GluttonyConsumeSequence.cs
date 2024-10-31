using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Gameplay.Timer;
using Combat;
using Combat.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entity.Abilities
{
	public class GluttonyConsumeSequence : MonoBehaviour
	{
		[SerializeField] private float totalDuration;
		[SerializeField, Range(0f, 1f)] private float scaleToMaxSizeNormalizedTime;
		[SerializeField] private AnimationCurve scaleToMaxSizeCurve;
		[SerializeField] private AnimationCurve scaleToZeroSizeCurve;

		public bool EntityReadyToBeDestroyed { get; private set; } = false;

		private Vector3 _maxSize;
		private CountdownTimer _toMaxSizeTimer;
		private CountdownTimer _toZeroSizeTimer;

		private void Awake()
		{
			_toMaxSizeTimer = new CountdownTimer(totalDuration * scaleToMaxSizeNormalizedTime)
			{
				OnTimerStop = () =>
				{
					EntityReadyToBeDestroyed = true;
					_toZeroSizeTimer.Start();
				}
			};
			_toZeroSizeTimer = new CountdownTimer(totalDuration * (1f - scaleToMaxSizeNormalizedTime))
			{
				OnTimerStop = () =>
				{
					Destroy(gameObject);
				}
			};

			enabled = false;
		}

		private void Update()
		{
			_toMaxSizeTimer.Tick(Time.deltaTime);
			_toZeroSizeTimer.Tick(Time.deltaTime);

			if (!_toMaxSizeTimer.IsRunning && !_toZeroSizeTimer.IsRunning)
				return;

			float sizeScaleEvaluated = 0;
			sizeScaleEvaluated = _toMaxSizeTimer.IsRunning ? scaleToMaxSizeCurve.Evaluate(_toMaxSizeTimer.Progress) : scaleToZeroSizeCurve.Evaluate(_toZeroSizeTimer.Progress);
			transform.localScale = _maxSize * sizeScaleEvaluated;
		}

		public GluttonyConsumeSequence Execute()
		{
			_toMaxSizeTimer.Start();
			return this;
		}

		public class Builder
		{
			private Vector3 _position;
			private Vector3 _size;

			public Builder SetPosition(Vector3 pos)
			{
				_position = pos;
				return this;
			}
			public Builder SetMaxSize(float size)
			{
				_size = new Vector3(size, size, size);
				return this;
			}

			public GluttonyConsumeSequence Build(GluttonyConsumeSequence instance)
			{
				instance.transform.localPosition = _position;
				instance._maxSize = _size;
				//instance.consumable = _consumable;
				instance.transform.localScale = Vector3.zero;

				instance.enabled = true;

				return instance;
			}
		}
	}
}