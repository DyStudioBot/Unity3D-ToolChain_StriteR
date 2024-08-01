﻿using System;
using System.Collections.Generic;
using System.Linq.Extensions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Rendering.PostProcess;

namespace Rendering.Pipeline
{
    [Flags]
    public enum EPipeLineExtensionFeature
    {
        Normal = 1 << 0,
        Mask = 1 << 1,
        MotionVector = 1 << 2,
        Reflection = 1 << 3,
        Antialiasing = 1 << 4,
    }
    
    [Serializable]
    public struct FPipelineExtensionParameters
    {
        [Header("Screen Space"), Tooltip("Screen Space World Position Reconstruction")]

        public EPipeLineExtensionFeature m_Features;
        [MFoldout(nameof(m_Features), EPipeLineExtensionFeature.Mask)] public SRD_MaskData m_MaskData;
        [MFoldout(nameof(m_Features), EPipeLineExtensionFeature.Reflection)] public ReflectionPassData m_PlanarReflection;
        [MFoldout(nameof(m_Features), EPipeLineExtensionFeature.Antialiasing)] public DAntiAliasing m_AntiAliasing;

        public static FPipelineExtensionParameters kDefault = new FPipelineExtensionParameters()
        {
            m_Features = default,
            m_MaskData = SRD_MaskData.kDefault,
            m_PlanarReflection = ReflectionPassData.kDefault,
            m_AntiAliasing = DAntiAliasing.kDefault,
        };
    }
    
    public class PipelineExtensionFeature : ScriptableRendererFeature
    {
        public RenderResources m_Resources;
        public FPipelineExtensionParameters m_Data = FPipelineExtensionParameters.kDefault;
        
        private SRP_GlobalParameters m_GlobalParameters;
        private NormalTexturePass m_Normal;
        private MaskTexturePass m_Mask;
        private MotionVectorTexturePass m_MotionVectorTexture;
        private ReflectionTexturePass m_Reflection;

        private SRP_TAAPass m_TAA;
        private SRP_ComponentBasedPostProcess m_OpaquePostProcess;
        private SRP_ComponentBasedPostProcess m_ScreenPostProcess;
        private PostProcess_AntiAliasing m_AntiAliasingPostProcess;
        private ISRPBase[] m_Passes;

        public override void Create()
        {
            if (!m_Resources)
                return;

            m_GlobalParameters = new SRP_GlobalParameters() { renderPassEvent = RenderPassEvent.BeforeRendering };
            m_TAA = new SRP_TAAPass() { renderPassEvent = RenderPassEvent.BeforeRenderingOpaques - 1 };
            m_MotionVectorTexture = new MotionVectorTexturePass() {renderPassEvent = RenderPassEvent.BeforeRenderingOpaques - 1};
            m_Mask = new MaskTexturePass() { renderPassEvent = RenderPassEvent.BeforeRenderingOpaques };
            m_Normal = new NormalTexturePass() { renderPassEvent = RenderPassEvent.BeforeRenderingSkybox + 1 };
            m_Reflection = new ReflectionTexturePass(m_Data.m_PlanarReflection, RenderPassEvent.BeforeRenderingSkybox + 2);
            
            m_OpaquePostProcess = new SRP_ComponentBasedPostProcess() { renderPassEvent = RenderPassEvent.AfterRenderingSkybox + 3 };
            m_ScreenPostProcess = new SRP_ComponentBasedPostProcess() { renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing + 1 };
            m_AntiAliasingPostProcess = new PostProcess_AntiAliasing(m_Data.m_AntiAliasing,m_TAA);

            m_Passes = new ISRPBase[] {m_GlobalParameters,
                m_Normal,m_Mask,m_MotionVectorTexture,m_Reflection
                ,m_TAA,m_OpaquePostProcess,m_ScreenPostProcess};
        }

        protected override void Dispose(bool _disposing)
        {
            base.Dispose(_disposing);
            if (!m_Resources)
                return;
            
            m_AntiAliasingPostProcess.Dispose();
            m_Passes.Traversal(_p=>_p.Dispose());
        }

        public override void AddRenderPasses(ScriptableRenderer _renderer, ref RenderingData _renderingData)
        {
            if (!m_Resources)
                return;
            
            if (_renderingData.cameraData.isPreviewCamera)
                return;

            var mask = m_Data.m_Features.IsFlagEnable(EPipeLineExtensionFeature.Mask);
            var normal = m_Data.m_Features.IsFlagEnable(EPipeLineExtensionFeature.Normal);
            var reflection = _renderingData.cameraData.isSceneViewCamera || m_Data.m_Features.IsFlagEnable(EPipeLineExtensionFeature.Reflection);
            var motionVector = m_Data.m_Features.IsFlagEnable(EPipeLineExtensionFeature.MotionVector);
            var antialiasing = m_Data.m_Features.IsFlagEnable(EPipeLineExtensionFeature.Antialiasing);
            
            if(_renderingData.cameraData.camera.TryGetComponent(out CameraOverride param))
            {
                normal = param.m_Normal.IsEnabled(normal);
                reflection = param.m_Reflection.IsEnabled(reflection);
            }
            
            if(mask)
                _renderer.EnqueuePass(m_Mask.Setup(m_Data.m_MaskData));
            if (normal)
                _renderer.EnqueuePass(m_Normal);
            
            if(motionVector)
                _renderer.EnqueuePass(m_MotionVectorTexture);
            if (reflection)
                m_Reflection.EnqueuePass(_renderer);
            
            _renderer.EnqueuePass(m_GlobalParameters);
            EnqueuePostProcess(_renderer,ref _renderingData,param,antialiasing);
        }

        private readonly List<IPostProcessBehaviour> m_PostprocessQueue = new List<IPostProcessBehaviour>();
        private readonly List<IPostProcessBehaviour> m_OpaqueProcessing = new List<IPostProcessBehaviour>();
        private readonly List<IPostProcessBehaviour> m_ScreenProcessing = new List<IPostProcessBehaviour>();
        private CameraOverride m_PostProcessingPreview;
        void EnqueuePostProcess(ScriptableRenderer _renderer,ref RenderingData _data,CameraOverride _override,bool antialiasing)
        {
            m_PostprocessQueue.Clear();
            //Enqueue AntiAliasing
            if (antialiasing)
            {
                if (m_Data.m_AntiAliasing.mode != EAntiAliasing.None)
                    m_PostprocessQueue.Add(m_AntiAliasingPostProcess);
                if(m_Data.m_AntiAliasing.mode == EAntiAliasing.TAA)
                    _renderer.EnqueuePass(m_TAA);
            }

            //Enqueue Global
            if(PostProcessGlobalVolume.HasGlobal)
            {
                var components = PostProcessGlobalVolume.GlobalVolume.GetComponents<IPostProcessBehaviour>();
                for(int j=0;j<components.Length;j++)
                    m_PostprocessQueue.Add(components[j]);
            }
            
            //Enqueue Camera Preview
            if (_data.cameraData.isSceneViewCamera)
            {
                if(m_PostProcessingPreview !=null && m_PostProcessingPreview.m_PostProcessPreview)
                    m_PostprocessQueue.AddRange(m_PostProcessingPreview.GetComponentsInChildren<IPostProcessBehaviour>());
            }
            else
            {
                if (_data.postProcessingEnabled && _data.cameraData.postProcessEnabled)
                {
                    if (_override != null && _override.m_PostProcessPreview)
                        m_PostProcessingPreview = _override;
                    m_PostprocessQueue.AddRange(_data.cameraData.camera.GetComponentsInChildren<IPostProcessBehaviour>());
                }
            }
            
            if (m_PostprocessQueue.Count<=0)
                return; 
            //Sort&Enqeuue
            m_OpaqueProcessing.Clear();
            m_ScreenProcessing.Clear();

            var postProcessCount = m_PostprocessQueue.Count;
            for (int i = 0; i < postProcessCount; i++)
            {
                var postProcess = m_PostprocessQueue[i];
#if UNITY_EDITOR
                postProcess.ValidateParameters();
#endif
                
                if (!postProcess.m_Enabled)
                    continue;
                
                if(postProcess.m_OpaqueProcess)
                    m_OpaqueProcessing.Add(postProcess);
                else
                    m_ScreenProcessing.Add(postProcess);
            }

            if(m_OpaqueProcessing.Count>0)
                _renderer.EnqueuePass(m_OpaquePostProcess.Setup(m_OpaqueProcessing));
            if(m_ScreenProcessing.Count>0)
                _renderer.EnqueuePass(m_ScreenPostProcess.Setup(m_ScreenProcessing));
        }
    }
}