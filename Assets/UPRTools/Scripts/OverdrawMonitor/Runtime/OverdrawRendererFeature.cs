#if URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OverdrawForURP
{
	public class OverdrawRendererFeature : ScriptableRendererFeature
	{

		private OverdrawPass opaquePass;
		private OverdrawPass transparentPass;

		[SerializeField] private Shader opaqueShader = null;
		[SerializeField] private Shader transparentShader = null;

		private RenderTargetHandle _overdrawAttachment;
		public override void Create()
		{

			if (!opaqueShader || !transparentShader)
			{
				return;
			}
			_overdrawAttachment.Init("_OverdrawAttachment");
			opaquePass = new OverdrawPass("Overdraw Render Opaque", RenderQueueRange.opaque, opaqueShader, true);
			opaquePass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
			transparentPass = new OverdrawPass("Overdraw Render Transparent", RenderQueueRange.transparent, transparentShader, false);
			transparentPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{

			renderer.EnqueuePass(opaquePass);
			renderer.EnqueuePass(transparentPass);
		}
		
	}
}
#endif