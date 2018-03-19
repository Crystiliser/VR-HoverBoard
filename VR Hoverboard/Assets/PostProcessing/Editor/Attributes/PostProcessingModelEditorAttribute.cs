namespace UnityEditor.PostProcessing
{
    public class PostProcessingModelEditorAttribute : System.Attribute
    {
        public readonly System.Type type;
        public PostProcessingModelEditorAttribute(System.Type type) { this.type = type; }
    }
}