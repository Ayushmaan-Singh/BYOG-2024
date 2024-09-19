using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AstekUtility.VisualFeedback
{
	public class DamageBlinking : VisualFX
	{
		[Tooltip("Meshes with materials to apply the effect on")]
		[SerializeField] private List<MeshRenderer> _applyEffectOnRenders;

		[Title("Sequence settings")]
		[SerializeField] private float _duration;
		[SerializeField] [Range(0, 10)] private float _blinkIntensity;
		[SerializeField] private Color _blinkColor;
		[SerializeField] private AnimationCurve _lerpCurve;

		private EffectState _currentState = EffectState.CanBeExecuted;
		private List<Texture> _emissionTexture = new List<Texture>();

		//For Internal Processing
		private bool _isFlashing;

		private List<Color> _materialbaseColor = new List<Color>();
		private List<Material> _materialsAffected = new List<Material>();
		private float _timeCounter;

		private void Update()
		{
			switch (_currentState)
			{
				case EffectState.CanBeExecuted:

					if (_isFlashing)
					{
						_isFlashing = false;
						_timeCounter = 0;

						for (int i = 0; i < _materialsAffected.Count; i++)
						{
							if (_materialsAffected[i].GetTexture("_EmissionMap") != null)
							{
								_materialsAffected[i].SetColor("_EmissionColor", Color.black);
								_materialsAffected[i].SetTexture("_EmissionMap", null);
							}
							else
							{
								_materialsAffected[i].EnableKeyword("_EMISSION");
							}
						}
						_currentState = EffectState.Running;
					}

					break;

				case EffectState.Running:

					if (_timeCounter <= _duration)
					{
						FlashMaterial(_materialsAffected.Count, Mathf.Clamp01(_timeCounter / _duration));
					}
					else
					{
						_currentState = EffectState.Restart;
					}
					_timeCounter += Time.deltaTime;

					break;

				case EffectState.Paused:
					break;

				case EffectState.Restart:

					ClearFlashingFromMaterial(_materialsAffected.Count);
					_currentState = EffectState.CanBeExecuted;

					break;
			}
		}

		private void OnEnable()
		{
			_materialbaseColor = _materialbaseColor ?? new List<Color>();
			_materialsAffected = _materialsAffected ?? new List<Material>();
			_emissionTexture = _emissionTexture ?? new List<Texture>();

			foreach (MeshRenderer mesh in _applyEffectOnRenders)
			{
				foreach (Material material in mesh.materials)
				{
					_emissionTexture.Add(material.GetTexture("_EmissionMap"));
					_materialbaseColor.Add(material.GetColor("_EmissionColor"));
					_materialsAffected.Add(material);
				}
			}
		}

		private void OnDisable()
		{
			_emissionTexture?.Clear();
			_materialbaseColor?.Clear();
			_materialsAffected?.Clear();
		}

		public override void Play()
		{
			if (_currentState == EffectState.Running)
			{
				_currentState = EffectState.Restart;
			}
			_isFlashing = true;
		}


		public override void Stop()
		{
			if (_currentState != EffectState.Running)
				return;
		}

		private void FlashMaterial(int size, float ratio)
		{
			for (int i = 0; i < size; i++)
			{
				_materialsAffected[i].SetColor("_EmissionColor", Color.Lerp(Color.black, _blinkColor, _lerpCurve.Evaluate(ratio)) * (ratio * _blinkIntensity));
			}
		}

		private void ClearFlashingFromMaterial(int size)
		{
			for (int i = 0; i < size; i++)
			{
				if (_emissionTexture[i] != null)
				{
					_materialsAffected[i].SetTexture("_EmissionMap", _emissionTexture[i]);
				}
				else
				{
					_materialsAffected[i].DisableKeyword("_EMISSION");
				}
				_materialsAffected[i].SetColor("_EmissionColor", _materialbaseColor[i]);
			}
		}
		private enum EffectState
		{
			Running,
			Paused,
			CanBeExecuted,
			Restart
		}
	}


}