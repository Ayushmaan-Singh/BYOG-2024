using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;
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

		private CancellationTokenSource _ctk;

		private void OnEnable()
		{
			_flashMaterialInstance = new Material(meshFlashMaterial);
			foreach (Renderer matRenderer in renderers)
			{
				_defaultMaterials.Add(matRenderer, matRenderer.materials);
			}
			_ctk = new CancellationTokenSource();
		}

		private void OnDisable()
		{
			_ctk.Cancel();
			Reset();
			_flashMaterialInstance = null;
			_defaultMaterials = null;
			_ctk = null;
		}

		public override void Play()
		{
			if (_defaultMaterials == null || _ctk == null)
				return;

			StopAllCoroutines();
			_flashMaterialInstance.SetColor(FlashColor, flashColor);
			_flashMaterialInstance.SetFloat(FlashAmount, 1);
			_ctk = new CancellationTokenSource();
			FlashOut(_ctk.Token);
		}
		public override void Stop()
		{
			_ctk.Cancel();
			_flashMaterialInstance.SetFloat(FlashAmount, 0);
		}

		private async void FlashOut(CancellationToken ctk)
		{
			float timeCounter = 0;

			foreach (Renderer matRenderer in renderers)
			{
				matRenderer.materials = new[]
				{
					_flashMaterialInstance
				};
			}

			while (timeCounter < duration && !ctk.IsCancellationRequested)
			{
				_flashMaterialInstance.SetFloat(FlashAmount, Mathf.Lerp(1, 0, timeCounter / duration));
				timeCounter += Time.deltaTime;

				ctk.ThrowIfCancellationRequested();
				await Task.Delay(Random.Range(16, 33), ctk);
			}
			_flashMaterialInstance.SetFloat(FlashAmount, 0);
			Reset();
		}

		private void Reset()
		{
			foreach (Renderer matRenderer in renderers)
			{
				matRenderer.materials = _defaultMaterials[matRenderer];
			}
		}
	}
}