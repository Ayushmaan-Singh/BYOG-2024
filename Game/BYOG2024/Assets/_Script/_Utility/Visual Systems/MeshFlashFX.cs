using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
namespace AstekUtility.VisualFeedback
{
	public class MeshFlashFX : VisualFX
	{
		[SerializeField] private Material meshFlashMaterial;
		[SerializeField, ColorUsage(true, true)] private Color flashColor;
		[SerializeField] private MeshRenderer meshRenderer;
		[SerializeField] private float duration = 0.3f;

		private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
		private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
		private Material _flashMaterialInstance;
		private Material[] _defaultMaterials;

		private CoroutineTask flashCoroutine;

		private void OnEnable()
		{
			_flashMaterialInstance = new Material(meshFlashMaterial);
			_defaultMaterials = meshRenderer.materials;
			flashCoroutine = new CoroutineTask(FlashOut(), this, false);
		}

		private void OnDisable()
		{
			meshRenderer.materials = _defaultMaterials;
			_flashMaterialInstance = null;
			_defaultMaterials = null;
			flashCoroutine = null;
		}

		[Button]
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

			meshRenderer.materials = new[]
			{
				_flashMaterialInstance
			};

			while (timeCounter < duration)
			{
				_flashMaterialInstance.SetFloat(FlashAmount, Mathf.Lerp(1, 0, timeCounter / duration));
				timeCounter += Time.deltaTime;
				yield return null;
			}
			_flashMaterialInstance.SetFloat(FlashAmount, 0);

			meshRenderer.materials = _defaultMaterials;
		}
	}
}