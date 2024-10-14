using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
namespace AstekUtility.VisualFeedback
{
	public class MeshFlashFX : VisualFX
	{
		[SerializeField] private Material meshFlashMaterial;
		[SerializeField, ColorUsage(true, true)] private Color flashColor;
		[SerializeField] private Renderer[] renderers;
		[SerializeField] private float duration = 0.3f;

		private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
		private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
		private Material _flashMaterialInstance;
		private Dictionary<Renderer, Material[]> _defaultMaterials = new Dictionary<Renderer, Material[]>();

		private CoroutineTask flashCoroutine;

		private void OnEnable()
		{
			_flashMaterialInstance = new Material(meshFlashMaterial);
			foreach (Renderer renderer in renderers)
			{
				_defaultMaterials.Add(renderer, renderer.materials);
			}
			flashCoroutine = new CoroutineTask(FlashOut(), this, false);
		}

		private void OnDisable()
		{
			Reset();
			_flashMaterialInstance = null;
			_defaultMaterials = null;
			flashCoroutine = null;
		}

		public override void Play()
		{
			if (_defaultMaterials == null || flashCoroutine == null)
				return;

			StopAllCoroutines();
			_flashMaterialInstance.SetColor(FlashColor, flashColor);
			_flashMaterialInstance.SetFloat(FlashAmount, 1);
			flashCoroutine?.Start();
		}
		public override void Stop()
		{
			flashCoroutine?.Stop();
			_flashMaterialInstance.SetFloat(FlashAmount, 0);
		}

		private IEnumerable FlashOut()
		{
			float timeCounter = 0;

			foreach (Renderer renderer in renderers)
			{
				renderer.materials = new[]
				{
					_flashMaterialInstance
				};
			}

			while (timeCounter < duration)
			{
				_flashMaterialInstance.SetFloat(FlashAmount, Mathf.Lerp(1, 0, timeCounter / duration));
				timeCounter += Time.deltaTime;
				yield return null;
			}
			_flashMaterialInstance.SetFloat(FlashAmount, 0);
			Reset();
		}

		private void Reset()
		{
			foreach (Renderer renderer in renderers)
			{
				renderer.materials = _defaultMaterials[renderer];
			}
		}
	}
}