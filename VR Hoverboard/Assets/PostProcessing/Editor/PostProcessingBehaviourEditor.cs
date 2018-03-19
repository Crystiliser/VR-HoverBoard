namespace UnityEditor.PostProcessing
{
    using System.Linq.Expressions;
    using UnityEngine.PostProcessing;
    [CustomEditor(typeof(PostProcessingBehaviour))]
    public class PostProcessingBehaviourEditor : Editor
    {
        private SerializedProperty m_Profile = null;
        public void OnEnable() => m_Profile = FindSetting((PostProcessingBehaviour x) => x.profile);
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Profile);
            serializedObject.ApplyModifiedProperties();
        }
        private SerializedProperty FindSetting<T, TValue>(Expression<System.Func<T, TValue>> expr) => serializedObject.FindProperty(ReflectionUtils.GetFieldPath(expr));
    }
}