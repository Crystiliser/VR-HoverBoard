namespace UnityEditor.PostProcessing
{
    using UnityEngine;
    using UnityEngine.PostProcessing;
    [CustomPropertyDrawer(typeof(MinAttribute))]
    sealed class MinDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinAttribute attribute = (MinAttribute)base.attribute;
            if (SerializedPropertyType.Integer == property.propertyType)
            {
                int v = EditorGUI.IntField(position, label, property.intValue);
                property.intValue = (int)Mathf.Max(v, attribute.min);
            }
            else if (SerializedPropertyType.Float == property.propertyType)
            {
                float v = EditorGUI.FloatField(position, label, property.floatValue);
                property.floatValue = Mathf.Max(v, attribute.min);
            }
            else
                EditorGUI.LabelField(position, label.text, "Use Min with float or int.");
        }
    }
}