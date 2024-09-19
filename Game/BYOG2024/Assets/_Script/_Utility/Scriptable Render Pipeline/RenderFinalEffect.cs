using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace AstekUtility.ScriptableRenderPipelines
{
	public partial class RenderFinalEffect : ScriptableRendererFeature
	{
		[SerializeField] private bool _useOutliner = true;
		[SerializeField] private bool _useInbuiltPixelator;

		public override void Create()
		{
			if (_useOutliner)
				Create_Outlines();
			if (_useInbuiltPixelator)
				Create_Pixelation();
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (_useOutliner)
				RenderPasses_Outlines(renderer, ref renderingData);
			if (_useInbuiltPixelator)
				RenderPasses_Pixelation(renderer, ref renderingData);
		}
	}
}