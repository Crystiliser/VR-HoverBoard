namespace UnityEditor.PostProcessing
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine.PostProcessing;
    [CustomEditor(typeof(PostProcessingProfile))]
    public class PostProcessingInspector : Editor
    {
        private Dictionary<PostProcessingModelEditor, PostProcessingModel> m_CustomEditors = new Dictionary<PostProcessingModelEditor, PostProcessingModel>();
        private void OnEnable()
        {
            if (null == target)
                return;
            IEnumerable<System.Type> editorTypes = System.Reflection.Assembly.GetAssembly(typeof(PostProcessingInspector)).GetTypes().Where(x => x.IsDefined(typeof(PostProcessingModelEditorAttribute), false));
            Dictionary<System.Type, PostProcessingModelEditor> customEditors = new Dictionary<System.Type, PostProcessingModelEditor>();
            foreach (System.Type editor in editorTypes)
            {
                PostProcessingModelEditorAttribute attr = (PostProcessingModelEditorAttribute)editor.GetCustomAttributes(typeof(PostProcessingModelEditorAttribute), false)[0];
                System.Type effectType = attr.type;
                PostProcessingModelEditor editorInst = (PostProcessingModelEditor)System.Activator.CreateInstance(editor);
                editorInst.alwaysEnabled = false;
                editorInst.profile = target as PostProcessingProfile;
                editorInst.inspector = this;
                customEditors.Add(effectType, editorInst);
            }
            System.Type baseType = target.GetType();
            SerializedProperty property = serializedObject.GetIterator();
            while (property.Next(true))
            {
                if (!property.hasChildren)
                    continue;
                System.Type type = baseType;
                object srcObject = ReflectionUtils.GetFieldValueFromPath(serializedObject.targetObject, ref type, property.propertyPath);
                if (null == srcObject)
                    continue;
                PostProcessingModelEditor editor;
                if (customEditors.TryGetValue(type, out editor))
                {
                    PostProcessingModel effect = (PostProcessingModel)srcObject;
                    m_CustomEditors.Add(editor, effect);
                    editor.target = effect;
                    editor.serializedProperty = property.Copy();
                    editor.OnPreEnable();
                }
            }
        }
        private void OnDisable()
        {
            if (null != m_CustomEditors)
                m_CustomEditors.Clear();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            foreach (KeyValuePair<PostProcessingModelEditor, PostProcessingModel> editor in m_CustomEditors)
                editor.Key.OnGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}