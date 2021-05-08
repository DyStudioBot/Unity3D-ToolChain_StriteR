﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;
using Rendering.ImageEffect;
namespace Rendering.Pipeline
{
    public class SRF_Additional : ScriptableRendererFeature
    {
        #region IDs
        static readonly int ID_FrustumCornersRayBL = Shader.PropertyToID("_FrustumCornersRayBL");
        static readonly int ID_FrustumCornersRayBR = Shader.PropertyToID("_FrustumCornersRayBR");
        static readonly int ID_FrustumCornersRayTL = Shader.PropertyToID("_FrustumCornersRayTL");
        static readonly int ID_FrustumCornersRayTR = Shader.PropertyToID("_FrustumCornersRayTR");
        static readonly int ID_ViewProjectionMatrix = Shader.PropertyToID("_MatrixVP");
        static readonly int ID_InvViewProjectionMatrix = Shader.PropertyToID("_MatrixInvVP");
        #endregion
        [Header("Screen Space Params")]
        public bool m_FrustumCornersRay;
        [Tooltip("World Space Post Process Required")] public bool m_CameraViewProjectionMatrix;
        [Header("External Textures")]
        public bool m_OpaqueBlurTexture=false;
        [MFoldout(nameof(m_OpaqueBlurTexture), true)] public ImageEffectParam_Blurs m_BlurParams = UPipeline.GetDefaultPostProcessData<ImageEffectParam_Blurs>();
        public bool m_NormalTexture=false;
        [MFoldout(nameof(m_CameraViewProjectionMatrix), true)] public bool m_CameraReflectionTexture=false;
        [HideInInspector, SerializeField] ComputeShader m_CameraReflectionComputeShader;

        SRP_OpaqueBlurTexture m_OpaqueBlurPass;
        SRP_NormalTexture m_NormalPass;
        SRP_CameraReflectionTexture m_ReflecitonPass;
        SRP_PerCameraPostProcessing m_PostProcesssing_Opaque;
        SRP_PerCameraPostProcessing m_PostProcesssing_AfterAll;
        public override void Create()
        {
            m_OpaqueBlurPass = new SRP_OpaqueBlurTexture() { renderPassEvent = RenderPassEvent.AfterRenderingSkybox + 1 };
            m_NormalPass = new SRP_NormalTexture() { renderPassEvent = RenderPassEvent.AfterRenderingSkybox + 1 };
            m_ReflecitonPass = new SRP_CameraReflectionTexture() { renderPassEvent = RenderPassEvent.AfterRenderingSkybox + 2 };
            m_PostProcesssing_Opaque = new SRP_PerCameraPostProcessing("Opaque Post Process") { renderPassEvent = RenderPassEvent.AfterRenderingSkybox };
            m_PostProcesssing_AfterAll = new SRP_PerCameraPostProcessing("After All Post Process") { renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing };
#if UNITY_EDITOR
            m_CameraReflectionComputeShader = UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Shaders/Compute/PlanarReflection.compute");
#endif
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_OpaqueBlurPass.Dispose();
            m_NormalPass.Dispose();
            m_ReflecitonPass.Dispose();
            m_PostProcesssing_Opaque.Dispose();
            m_PostProcesssing_AfterAll.Dispose();
        }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isPreviewCamera)
                return;

            bool frustumCornersRay = m_FrustumCornersRay;
            bool viewProjectionMatrix = m_CameraViewProjectionMatrix;
            bool opaqueBlurTexture = m_OpaqueBlurTexture;
            bool cameraNormalTexture = m_NormalTexture;
            bool cameraReflectionTexture = m_CameraReflectionTexture;

            if(!renderingData.cameraData.isSceneViewCamera)
            {
                if (!renderingData.cameraData.camera.gameObject.TryGetComponent(out SRD_AddtionalData data))
                    data = renderingData.cameraData.camera.gameObject.AddComponent<SRD_AddtionalData>();

                frustumCornersRay = data.m_FrustumCornersRay.IsEnabled(frustumCornersRay);
                viewProjectionMatrix = data.m_CameraViewProjectionMatrix.IsEnabled(viewProjectionMatrix);
                opaqueBlurTexture = data.m_OpaqueBlurTexture.IsEnabled(opaqueBlurTexture);
                cameraNormalTexture = data.m_NormalTexture.IsEnabled(cameraNormalTexture);
                cameraReflectionTexture = data.m_ReflectionTexture.IsEnabled(cameraReflectionTexture);
            }

            if (frustumCornersRay)
                UpdateFrustumCornersRay(renderingData.cameraData.camera);
            if (viewProjectionMatrix)
                UpdateViewProjectionMatrix(renderingData.cameraData);
            if (opaqueBlurTexture)
                renderer.EnqueuePass(m_OpaqueBlurPass.Setup(renderer.cameraColorTarget,m_BlurParams));
            if (cameraNormalTexture)
                renderer.EnqueuePass(m_NormalPass);
            if (cameraReflectionTexture)
                UpdateCameraReflectionTexture(renderer);
            UpdatePostProcess(renderer, ref renderingData);
        }
        void UpdateFrustumCornersRay(Camera _camera)
        {
            float fov = _camera.fieldOfView;
            float near = _camera.nearClipPlane;
            float aspect = _camera.aspect;
            Transform cameraTrans = _camera.transform;
            float halfHeight = near * Mathf.Tan(fov * .5f * Mathf.Deg2Rad);
            Vector3 toRight = cameraTrans.right * halfHeight * aspect;
            Vector3 toTop = cameraTrans.up * halfHeight;
            Vector3 topLeft = cameraTrans.forward * near + toTop - toRight;
            float scale = topLeft.magnitude / near;
            topLeft.Normalize();
            topLeft *= scale;
            Vector3 topRight = cameraTrans.forward * near + toTop + toRight;
            topRight.Normalize();
            topRight *= scale;
            Vector3 bottomLeft = cameraTrans.forward * near - toTop - toRight;
            bottomLeft.Normalize();
            bottomLeft *= scale;
            Vector3 bottomRight = cameraTrans.forward * near - toTop + toRight;
            bottomRight.Normalize();
            bottomRight *= scale;
            Shader.SetGlobalVector(ID_FrustumCornersRayBL, bottomLeft);
            Shader.SetGlobalVector(ID_FrustumCornersRayBR, bottomRight);
            Shader.SetGlobalVector(ID_FrustumCornersRayTL, topLeft);
            Shader.SetGlobalVector(ID_FrustumCornersRayTR, topRight);
        }
        void UpdateViewProjectionMatrix(CameraData _cameraData)
        {
            Matrix4x4 view = _cameraData.camera.worldToCameraMatrix;
            Matrix4x4 projection =GL.GetGPUProjectionMatrix(  _cameraData.GetProjectionMatrix(),false);
            Matrix4x4 viewProjection =  projection*view;
            Shader.SetGlobalMatrix(ID_ViewProjectionMatrix,viewProjection);
            Shader.SetGlobalMatrix(ID_InvViewProjectionMatrix, viewProjection.inverse);
        }
        void UpdateCameraReflectionTexture(ScriptableRenderer _renderer)
        {
            if (SRD_ReflectionPlane.m_ReflectionPlanes.Count == 0)
                return;
            SRD_ReflectionPlane plane = SRD_ReflectionPlane.m_ReflectionPlanes[0];
            if (!plane.m_MeshRenderer.isVisible)
                return;
            _renderer.EnqueuePass(m_ReflecitonPass.Setup(_renderer.cameraColorTarget, m_CameraReflectionComputeShader, plane));
        }
        void UpdatePostProcess(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var postEffects = renderingData.cameraData.camera.GetComponents<APostEffectBase>().FindAll(p => p.enabled);
            if (postEffects.Count() == 0)
                return;

            var dictionary = postEffects.ToListDictionary(p => p.m_IsOpaqueProcess);

            foreach (var key in dictionary.Keys)
            {
                int count = dictionary[key].Count();
                if (count == 0)
                    continue;

                if (key)
                    renderer.EnqueuePass(m_PostProcesssing_Opaque.Setup(renderer.cameraColorTarget, dictionary[key]));
                else
                    renderer.EnqueuePass(m_PostProcesssing_AfterAll.Setup(renderer.cameraColorTarget, dictionary[key]));
            }
        }
    }
}