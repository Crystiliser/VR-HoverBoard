namespace UnityEditor.PostProcessing
{
    using UnityEngine;
    using UnityEngine.PostProcessing;
    using VignetteMode = UnityEngine.PostProcessing.VignetteModel.Mode;
    using Settings = UnityEngine.PostProcessing.VignetteModel.Settings;
    [PostProcessingModelEditor(typeof(VignetteModel))]
    public class VignetteModelEditor : PostProcessingModelEditor
    {
        private SerializedProperty m_Mode = null;
        private SerializedProperty m_Color = null;
        private SerializedProperty m_Center = null;
        private SerializedProperty m_Intensity = null;
        private SerializedProperty m_Smoothness = null;
        private SerializedProperty m_Roundness = null;
        private SerializedProperty m_Mask = null;
        private SerializedProperty m_Opacity = null;
        private SerializedProperty m_Rounded = null;
        public override void OnEnable()
        {
            m_Mode = FindSetting((Settings x) => x.mode);
            m_Color = FindSetting((Settings x) => x.color);
            m_Center = FindSetting((Settings x) => x.center);
            m_Intensity = FindSetting((Settings x) => x.intensity);
            m_Smoothness = FindSetting((Settings x) => x.smoothness);
            m_Roundness = FindSetting((Settings x) => x.roundness);
            m_Mask = FindSetting((Settings x) => x.mask);
            m_Opacity = FindSetting((Settings x) => x.opacity);
            m_Rounded = FindSetting((Settings x) => x.rounded);
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_Mode);
            EditorGUILayout.PropertyField(m_Color);
            if (m_Mode.intValue < (int)VignetteMode.Masked)
            {
                EditorGUILayout.PropertyField(m_Center);
                EditorGUILayout.PropertyField(m_Intensity);
                EditorGUILayout.PropertyField(m_Smoothness);
                EditorGUILayout.PropertyField(m_Roundness);
                EditorGUILayout.PropertyField(m_Rounded);
            }
            else
            {
                Texture mask = (target as VignetteModel).GetSettings.mask;
                if (null != mask)
                {
                    TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mask)) as TextureImporter;
                    if (null != importer &&
                        (0 != importer.anisoLevel ||
                        importer.mipmapEnabled ||
                        TextureImporterAlphaSource.FromGrayScale != importer.alphaSource ||
                        TextureImporterCompression.Uncompressed != importer.textureCompression ||
                        TextureWrapMode.Clamp != importer.wrapMode))
                    {
                        EditorGUILayout.HelpBox("Invalid mask import settings.", MessageType.Warning);
                        GUILayout.Space(-32);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Fix", GUILayout.Width(60)))
                            {
                                SetMaskImportSettings(importer);
                                AssetDatabase.Refresh();
                            }
                            GUILayout.Space(8);
                        }
                        GUILayout.Space(11);
                    }
                }
                EditorGUILayout.PropertyField(m_Mask);
                EditorGUILayout.PropertyField(m_Opacity);
            }
        }
        private void SetMaskImportSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.SingleChannel;
            importer.alphaSource = TextureImporterAlphaSource.FromGrayScale;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.anisoLevel = 0;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
        }
    }
}