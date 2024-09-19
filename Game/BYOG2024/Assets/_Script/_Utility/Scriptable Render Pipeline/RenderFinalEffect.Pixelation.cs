using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace AstekUtility.ScriptableRenderPipelines
{
	public partial class RenderFinalEffect
	{

		private void Create_Pixelation()
		{
			customPass = new PixelizePass(_settings);
		}
		private void RenderPasses_Pixelation(ScriptableRenderer renderer, ref RenderingData renderingData)
		{

#if UNITY_EDITOR
			if (renderingData.cameraData.isSceneViewCamera) return;
#endif
			renderer.EnqueuePass(customPass);
		}
		#region Pixelation Variables

		[Space(20)]
		[FoldoutGroup("=====Pixelation=====")]
		[SerializeField] private PixelationPassSettings _settings;
		private PixelizePass customPass;

		#endregion

		#region Pixelation Render Classes

		[Serializable]
		public class PixelationPassSettings
		{
			public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
			public FilterMode filterMode = FilterMode.Point;
			public int screenHeight = 144;
		}

		public class PixelizePass : ScriptableRenderPass
		{

			private RenderTargetIdentifier _colorBuffer, _pixelBuffer;
			private readonly int _pixelBufferID = Shader.PropertyToID("_PixelBuffer");
			private int _pixelScreenHeight, _pixelScreenWidth;

			private RenderTargetIdentifier _pointBuffer;
			private readonly int _pointBufferID = Shader.PropertyToID("_PointBuffer");
			private readonly PixelationPassSettings _settings;

			public PixelizePass(PixelationPassSettings settings)
			{
				_settings = settings;
				renderPassEvent = settings.renderPassEvent;
			}

			public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
			{
				_colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
				RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

				cmd.GetTemporaryRT(_pointBufferID, descriptor.width, descriptor.height, 0, _settings.filterMode);
				_pointBuffer = new RenderTargetIdentifier(_pointBufferID);

				_pixelScreenHeight = _settings.screenHeight;
				_pixelScreenWidth = (int)(_pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

				descriptor.height = _pixelScreenHeight;
				descriptor.width = _pixelScreenWidth;

				cmd.GetTemporaryRT(_pixelBufferID, descriptor, _settings.filterMode);
				_pixelBuffer = new RenderTargetIdentifier(_pixelBufferID);
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = CommandBufferPool.Get();
				using (new ProfilingScope(cmd, new ProfilingSampler("Pixelize Pass")))
				{
					// No-shader variant
					cmd.Blit(_colorBuffer, _pointBuffer);
					cmd.Blit(_pointBuffer, _pixelBuffer);
					cmd.Blit(_pixelBuffer, _colorBuffer);
				}

				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}

			public override void OnCameraCleanup(CommandBuffer cmd)
			{
				if (cmd == null) throw new ArgumentNullException("_cmd");
				cmd.ReleaseTemporaryRT(_pixelBufferID);
				cmd.ReleaseTemporaryRT(_pointBufferID);
			}
		}

		#endregion
	}
}