using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OutlinesPack
{
    public class EdgeOutlinesPass : ScriptableRenderPass
    {
        private readonly Material material;
        private RenderTargetIdentifier cameraColorTarget;
        private RenderTargetHandle temporaryBuffer;

#if !UNITY_2020_2_OR_NEWER // v8
        private ScriptableRenderer renderer;
#endif

        public EdgeOutlinesPass(RenderPassEvent renderPassEvent, EdgeOutlinesSettings edgeOutlinesSettings)
        {
            this.renderPassEvent = renderPassEvent;

            material = new Material(Shader.Find("OutlinesPack/EdgeDetectionOutlines"));
            material.SetFloat("_Thickness", edgeOutlinesSettings.Width);
            material.SetColor("_Color", edgeOutlinesSettings.Color);
            material.SetFloat("_DepthThreshold", edgeOutlinesSettings.DepthThreshold);
            material.SetFloat("_NormalThreshold", edgeOutlinesSettings.NormalThreshold);
        }

        public void Setup(ScriptableRenderer renderer)
        {
#if UNITY_2020_2_OR_NEWER // v10+
					ConfigureInput(ScriptableRenderPassInput.Normal);
#else // v8
            this.renderer = renderer;
#endif
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!material) return;

            // Set Source / Destination
#if UNITY_2020_2_OR_NEWER // v10+
				var renderer = renderingData.cameraData.renderer;
#else // v8
            // For older versions, cameraData.renderer is internal so can't be accessed. Will pass it through from AddRenderPasses instead
            var renderer = this.renderer;
#endif

            cameraColorTarget = renderer.cameraColorTarget;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(
                "_EdgeDetecionOutlinesBlit")))
            {
                RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;

                cmd.GetTemporaryRT(temporaryBuffer.id, opaqueDescriptor, FilterMode.Point);
                Blit(cmd, cameraColorTarget, temporaryBuffer.Identifier(), material, 0);
                Blit(cmd, temporaryBuffer.Identifier(), cameraColorTarget);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(temporaryBuffer.id);
        }
    }

}