using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OutlinesPack
{
    public class BlurFeature : ScriptableRendererFeature
    {
        //Behind | Front | Both
        private enum DepthMask
        {
            BehindOnly, FrontOnly, Disable
        }

        [SerializeField] BlurOutlineSettings OutlinesSettings = new BlurOutlineSettings();

        [SerializeField] private LayerMask outlinesLayerMask = ~0;
        [SerializeField] private DepthMask depthMask = DepthMask.Disable;

        DepthMaskPass depthMaskPass;
        ObjectsPass objectsPass;
        BlurPass blurOutlinePass;

        public override void Create()
        {
            RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingSkybox;

            if (depthMask != DepthMask.Disable) depthMaskPass = new DepthMaskPass(renderPassEvent, outlinesLayerMask, "_SceneMask_Blur");
            objectsPass = new ObjectsPass(renderPassEvent, outlinesLayerMask, depthMask == DepthMask.Disable ? false : true);
            blurOutlinePass = new BlurPass(renderPassEvent, OutlinesSettings, (int)depthMask);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (depthMask != DepthMask.Disable) renderer.EnqueuePass(depthMaskPass);
            renderer.EnqueuePass(objectsPass);
            blurOutlinePass.Setup(renderer);
            renderer.EnqueuePass(blurOutlinePass);
        }
    }
}