using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace AstekUtility.VisualFeedback
{
	public class SpriteFlashFX : VisualFX
	{
		[SerializeField] private Material spriteFlashMaterial;
		[SerializeField, ColorUsage(true, true)] private Color flashColor;
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private float duration;

		private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
		private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
		private Material _spriteFlashMaterialInstance;
		private Material[] _defaultMaterial;

		private CoroutineTask flashCoroutine;

		private void OnEnable()
		{
			_spriteFlashMaterialInstance = new Material(spriteFlashMaterial);
			_defaultMaterial = spriteRenderer.materials;
			flashCoroutine = new CoroutineTask(FlashOut(), this, false);
		}

		private void OnDisable()
		{
			spriteRenderer.materials = _defaultMaterial;
			_spriteFlashMaterialInstance = null;
			_defaultMaterial = null;
			flashCoroutine = null;

		}

		public override void Play()
		{
			if (_defaultMaterial == null || flashCoroutine == null)
				return;

			StopAllCoroutines();
			_spriteFlashMaterialInstance.SetColor(FlashColor, flashColor);
			_spriteFlashMaterialInstance.SetFloat(FlashAmount, 1);
			flashCoroutine?.Start();
		}

		public override void Stop()
		{
			flashCoroutine?.Stop();
			spriteFlashMaterial.SetFloat(FlashAmount, 0);
		}

		private IEnumerable FlashOut()
		{
			float timeCounter = 0;
			spriteRenderer.materials = new[]
			{
				_spriteFlashMaterialInstance
			};
			while (timeCounter < duration)
			{
				spriteFlashMaterial.SetFloat(FlashAmount, Mathf.Lerp(1, 0, timeCounter / duration));
				timeCounter += Time.deltaTime;
				yield return null;
			}
			spriteFlashMaterial.SetFloat(FlashAmount, 0);

			spriteRenderer.materials = _defaultMaterial;
		}
	}
}