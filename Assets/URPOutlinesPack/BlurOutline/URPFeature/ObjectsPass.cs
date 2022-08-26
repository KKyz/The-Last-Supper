using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace OutlinesPack
{
    public class ObjectsPass : ScriptableRenderPass
    {
        private readonly RenderTargetHandle objects;

        private readonly Material material;
        private readonly List<ShaderTagId> shaderTagIdList;

        private FilteringSettings filteringSettings;
        private RenderStateBlock m_RenderStateBlock;


        public void SetDepthState(bool writeEnabled = true, CompareFunction function = CompareFunction.LessEqual)
        {
            m_RenderStateBlock.mask |= RenderStateMask.Depth;
            m_RenderStateBlock.depthState = new DepthState(writeEnabled, function);
        }

        public ObjectsPass(RenderPassEvent renderPassEvent, LayerMask outlinesLayerMask, bool useDepthMask)
        {
            filteringSettings = new FilteringSettings(RenderQueueRange.all, outlinesLayerMask);

            material = new Material(Shader.Find("OutlinesPack/OutlineObject"));
            material.SetFloat("_DrawBehind", useDepthMask ? 1 : 0);

            this.renderPassEvent = renderPassEvent;

            objects.Init("_OutlineObjectsTex");

            shaderTagIdList = new List<ShaderTagId>()
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefoultUnlit"),
                new ShaderTagId("DepthOnly")
            };

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            SetDepthState();
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
            normalsTextureDescriptor.colorFormat = RenderTextureFormat.ARGBFloat;
            normalsTextureDescriptor.msaaSamples = 1;

            cmd.GetTemporaryRT(objects.id, normalsTextureDescriptor, FilterMode.Bilinear);
            ConfigureTarget(objects.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!material)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(
                "_OutlineObjectsTex")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawingSettings.overrideMaterial = material;
                drawingSettings.enableDynamicBatching = true;

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref m_RenderStateBlock);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(objects.id);
        }
    }
}