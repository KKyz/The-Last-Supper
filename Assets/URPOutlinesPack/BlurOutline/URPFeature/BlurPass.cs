using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
namespace OutlinesPack
{
    public class BlurPass : ScriptableRenderPass
    {
        private readonly Material material;
        private RenderTargetIdentifier cameraColorTarget;

        private RenderTargetHandle temporaryBuffer;

        private int MaxWidth = 32;
        private float[] gaussSamples;

#if !UNITY_2020_2_OR_NEWER // v8
        private ScriptableRenderer renderer;
#endif


        private float[] GetGaussSamples(int width, float[] samples)
        {
            var stdDev = width * 0.5f;

            if (samples is null)
            {
                samples = new float[MaxWidth];
            }

            for (var i = 0; i < width; i++)
            {
                samples[i] = Gauss(i, stdDev);
            }

            return samples;
        }

        private float Gauss(float x, float stdDev)
        {
            var stdDev2 = stdDev * stdDev * 2;
            var a = 1 / Mathf.Sqrt(Mathf.PI * stdDev2);
            var gauss = a * Mathf.Pow((float)Math.E, -x * x / stdDev2);

            return gauss;
        }

        public BlurPass(RenderPassEvent renderPassEvent, BlurOutlineSettings blurOutlineSettings, int depthMask)
        {
            this.renderPassEvent = renderPassEvent;

            material = new Material(Shader.Find("OutlinesPack/Blur"));
            material.SetFloat("_Intensity", blurOutlineSettings.blur ? blurOutlineSettings.intensity : 100);
            material.SetColor("_Color", blurOutlineSettings.color);
            material.SetFloat("_Width", blurOutlineSettings.width);
            material.SetInt("_DepthMask", depthMask);

            gaussSamples = GetGaussSamples(32, gaussSamples);
            material.SetFloatArray("_GaussSamples", gaussSamples);

            temporaryBuffer.Init("_BluerredObjectsTex");
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
                "_BlurOutlinesBlit")))
            {
                RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDescriptor.colorFormat = RenderTextureFormat.RHalf;
                opaqueDescriptor.msaaSamples = 1;

                cmd.GetTemporaryRT(temporaryBuffer.id, opaqueDescriptor, FilterMode.Point);

                Blit(cmd, cameraColorTarget, temporaryBuffer.Identifier(), material, 0);
                Blit(cmd, temporaryBuffer.Identifier(), cameraColorTarget, material, 1);
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