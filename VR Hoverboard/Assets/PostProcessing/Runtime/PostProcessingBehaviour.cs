namespace UnityEngine.PostProcessing
{
    using System.Collections.Generic;
    using UnityEngine.Rendering;
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera)), DisallowMultipleComponent, ExecuteInEditMode]
    [AddComponentMenu("Effects/Post-Processing Behaviour", -1)]
    public class PostProcessingBehaviour : MonoBehaviour
    {
        public PostProcessingProfile profile = null;
        public System.Func<Vector2, Matrix4x4> jitteredMatrixFunc;
        private Dictionary<System.Type, KeyValuePair<CameraEvent, CommandBuffer>> m_CommandBuffers = null;
        private List<PostProcessingComponentBase> m_Components = null;
        private Dictionary<PostProcessingComponentBase, bool> m_ComponentStates = null;
        private MaterialFactory m_MaterialFactory = null;
        private RenderTextureFactory m_RenderTextureFactory = null;
        private PostProcessingContext m_Context = null;
        private Camera m_Camera = null;
        private PostProcessingProfile m_PreviousProfile = null;
        private BloomComponent m_Bloom = null;
        private VignetteComponent m_Vignette = null;
        private List<PostProcessingComponentBase> m_ComponentsToEnable = new List<PostProcessingComponentBase>();
        private List<PostProcessingComponentBase> m_ComponentsToDisable = new List<PostProcessingComponentBase>();
        private void OnEnable()
        {
            m_CommandBuffers = new Dictionary<System.Type, KeyValuePair<CameraEvent, CommandBuffer>>();
            m_MaterialFactory = new MaterialFactory();
            m_RenderTextureFactory = new RenderTextureFactory();
            m_Context = new PostProcessingContext();
            m_Components = new List<PostProcessingComponentBase>();
            m_Bloom = AddComponent(new BloomComponent());
            m_Vignette = AddComponent(new VignetteComponent());
            m_ComponentStates = new Dictionary<PostProcessingComponentBase, bool>();
            foreach (PostProcessingComponentBase component in m_Components)
                m_ComponentStates.Add(component, false);
            useGUILayout = false;
        }
        private void OnPreCull()
        {
            m_Camera = GetComponent<Camera>();
            if (profile == null || m_Camera == null)
                return;
            PostProcessingContext context = m_Context.Reset();
            context.profile = profile;
            context.renderTextureFactory = m_RenderTextureFactory;
            context.materialFactory = m_MaterialFactory;
            context.camera = m_Camera;
            m_Bloom.Init(context, profile.bloom);
            m_Vignette.Init(context, profile.vignette);
            if (m_PreviousProfile != profile)
            {
                DisableComponents();
                m_PreviousProfile = profile;
            }
            CheckObservers();
            DepthTextureMode flags = context.camera.depthTextureMode;
            foreach (PostProcessingComponentBase component in m_Components)
                if (component.active)
                    flags |= component.GetCameraFlags();
            context.camera.depthTextureMode = flags;
        }
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (null == profile || null == m_Camera)
            {
                Graphics.Blit(source, destination);
                return;
            }
            bool uberActive = false;
            Material uberMaterial = m_MaterialFactory.Get("Hidden/Post FX/Uber Shader");
            uberMaterial.shaderKeywords = null;
            RenderTexture src = source, dst = destination;
            Texture autoExposure = GraphicsUtils.WhiteTexture;
            uberMaterial.SetTexture("_AutoExposure", autoExposure);
            if (m_Bloom.active)
            {
                uberActive = true;
                m_Bloom.Prepare(src, uberMaterial, autoExposure);
            }
            uberActive |= TryPrepareUberImageEffect(m_Vignette, uberMaterial);
            if (uberActive)
            {
                if (ColorSpace.Linear != QualitySettings.activeColorSpace)
                    uberMaterial.EnableKeyword("UNITY_COLORSPACE_GAMMA");
                Graphics.Blit(src, dst, uberMaterial, 0);
            }
            if (!uberActive)
                Graphics.Blit(src, dst);
            m_RenderTextureFactory.ReleaseAll();
        }
        private void OnDisable()
        {
            foreach (KeyValuePair<CameraEvent, CommandBuffer> cb in m_CommandBuffers.Values)
            {
                m_Camera.RemoveCommandBuffer(cb.Key, cb.Value);
                cb.Value.Dispose();
            }
            m_CommandBuffers.Clear();
            if (null != profile)
                DisableComponents();
            m_Components.Clear();
            m_MaterialFactory.Dispose();
            m_RenderTextureFactory.Dispose();
        }
        private void CheckObservers()
        {
            foreach (KeyValuePair<PostProcessingComponentBase, bool> cs in m_ComponentStates)
            {
                PostProcessingComponentBase component = cs.Key;
                bool state = component.GetModel().enabled;
                if (state != cs.Value)
                {
                    if (state) m_ComponentsToEnable.Add(component);
                    else m_ComponentsToDisable.Add(component);
                }
            }
            for (int i = 0; i < m_ComponentsToDisable.Count; ++i)
            {
                PostProcessingComponentBase c = m_ComponentsToDisable[i];
                m_ComponentStates[c] = false;
                c.OnDisable();
            }
            for (int i = 0; i < m_ComponentsToEnable.Count; ++i)
            {
                PostProcessingComponentBase c = m_ComponentsToEnable[i];
                m_ComponentStates[c] = true;
                c.OnEnable();
            }
            m_ComponentsToDisable.Clear();
            m_ComponentsToEnable.Clear();
        }
        private void DisableComponents()
        {
            foreach (PostProcessingComponentBase component in m_Components)
            {
                PostProcessingModel model = component.GetModel();
                if (null != model && model.enabled)
                    component.OnDisable();
            }
        }
        private CommandBuffer AddCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
        {
            CommandBuffer cb = new CommandBuffer { name = name };
            KeyValuePair<CameraEvent, CommandBuffer> kvp = new KeyValuePair<CameraEvent, CommandBuffer>(evt, cb);
            m_CommandBuffers.Add(typeof(T), kvp);
            m_Camera.AddCommandBuffer(evt, kvp.Value);
            return kvp.Value;
        }
        private void RemoveCommandBuffer<T>() where T : PostProcessingModel
        {
            KeyValuePair<CameraEvent, CommandBuffer> kvp;
            System.Type type = typeof(T);
            if (m_CommandBuffers.TryGetValue(type, out kvp))
            {
                m_Camera.RemoveCommandBuffer(kvp.Key, kvp.Value);
                m_CommandBuffers.Remove(type);
                kvp.Value.Dispose();
            }
        }
        private CommandBuffer GetCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
        {
            CommandBuffer cb;
            KeyValuePair<CameraEvent, CommandBuffer> kvp;
            if (!m_CommandBuffers.TryGetValue(typeof(T), out kvp))
                cb = AddCommandBuffer<T>(evt, name);
            else if (kvp.Key != evt)
            {
                RemoveCommandBuffer<T>();
                cb = AddCommandBuffer<T>(evt, name);
            }
            else cb = kvp.Value;
            return cb;
        }
        private void TryExecuteCommandBuffer<T>(PostProcessingComponentCommandBuffer<T> component) where T : PostProcessingModel
        {
            if (component.active)
            {
                CommandBuffer cb = GetCommandBuffer<T>(component.GetCameraEvent(), component.GetName());
                cb.Clear();
                component.PopulateCommandBuffer(cb);
            }
            else RemoveCommandBuffer<T>();
        }
        private bool TryPrepareUberImageEffect<T>(PostProcessingComponentRenderTexture<T> component, Material material) where T : PostProcessingModel
        {
            if (component.active)
            {
                component.Prepare(material);
                return true;
            }
            return false;
        }
        private T AddComponent<T>(T component) where T : PostProcessingComponentBase
        {
            m_Components.Add(component);
            return component;
        }
    }
}