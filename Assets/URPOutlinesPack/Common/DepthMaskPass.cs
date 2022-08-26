using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OutlinesPack
{
    public class DepthMaskPass : ScriptableRenderPass
    {
        private readonly string name;
        private readonly RenderTargetHandle mask;

        private readonly Material maskMaterial;
        private readonly List<ShaderTagId> shaderTagIdList;

        private FilteringSettings filteringSettings;
        private float downScale;

        public DepthMaskPass(RenderPassEvent renderPassEvent, LayerMask outlinesLayerMask, string name, float downScale = 1)
        {
            this.downScale = downScale;
            this.name = name;
            LayerMask layerMask = ~outlinesLayerMask;

            filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);

            maskMaterial = new Material(Shader.Find("OutlinesPack/MaskShader"));

            this.renderPassEvent = renderPassEvent;

            mask.Init(name);

            shaderTagIdList = new List<ShaderTagId>()
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefoultUnlit"),
                new ShaderTagId("DepthOnly")
            };

            //m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            //SetDepthState();
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor textureDescriptor = cameraTextureDescriptor;
            textureDescriptor.colorFormat = RenderTextureFormat.RHalf;
            textureDescriptor.width = (int)(cameraTextureDescriptor.width / downScale);
            textureDescriptor.height = (int)(cameraTextureDescriptor.height / downScale);

            cmd.GetTemporaryRT(mask.id, textureDescriptor, FilterMode.Bilinear);
            ConfigureTarget(mask.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!maskMaterial)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(
                name)))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawingSettings.overrideMaterial = maskMaterial;
                drawingSettings.enableDynamicBatching = true;

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);//, ref m_RenderStateBlock
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(mask.id);
        }
    }
}