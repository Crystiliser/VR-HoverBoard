using UnityEngine;
using UnityEngine.PostProcessing;
using System;
using System.Linq.Expressions;

namespace UnityEditor.PostProcessing
{
    public class PostProcessingModelEditor
    {
        public PostProcessingModel target { get; internal set; }
        public SerializedProperty serializedProperty { get; internal set; }
        protected SerializedProperty m_SettingsProperty = null;
        protected SerializedProperty m_EnabledProperty = null;
        internal bool alwaysEnabled = false;
        internal PostProcessingProfile profile = null;
        internal PostProcessingInspector inspector = null;
        internal void OnPreEnable()
        {
            m_SettingsProperty = serializedProperty.FindPropertyRelative("m_Settings");
            m_EnabledProperty = serializedProperty.FindPropertyRelative("m_Enabled");
            OnEnable();
        }
        public virtual void OnEnable()
        { }
        internal void OnGUI()
        {
            GUILayout.Space(5);
            if (EditorGUIHelper.Header(serializedProperty.displayName, m_SettingsProperty, m_EnabledProperty, Reset))
            {
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledGroupScope(!m_EnabledProperty.boolValue))
                    OnInspectorGUI();
                --EditorGUI.indentLevel;
            }
        }
        private void Reset()
        {
            SerializedObject obj = serializedProperty.serializedObject;
            Undo.RecordObject(obj.targetObject, "Reset");
            target.Reset();
            EditorUtility.SetDirty(obj.targetObject);
        }
        public virtual void OnInspectorGUI()
        { }
        public void Repaint() => inspector.Repaint();
        protected SerializedProperty FindSetting<T, TValue>(Expression<Func<T, TValue>> expr) => m_SettingsProperty.FindPropertyRelative(ReflectionUtils.GetFieldPath(expr));
    }
}