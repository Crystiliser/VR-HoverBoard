namespace UnityEngine.PostProcessing
{
    using System.Collections.Generic;
    public sealed class MaterialFactory : System.IDisposable
    {
        private Dictionary<string, Material> m_Materials = new Dictionary<string, Material>();
        public Material Get(string shaderName)
        {
            Material material;
            if (!m_Materials.TryGetValue(shaderName, out material))
            {
                Shader shader = Shader.Find(shaderName);
                if (null == shader)
                    throw new System.ArgumentException(string.Format("Shader not found ({0})", shaderName));
                material = new Material(shader)
                {
                    name = string.Format("PostFX - {0}", shaderName.Substring(shaderName.LastIndexOf("/") + 1)),
                    hideFlags = HideFlags.DontSave
                };
                m_Materials.Add(shaderName, material);
            }
            return material;
        }
        public void Dispose()
        {
            Dictionary<string, Material>.Enumerator enumerator = m_Materials.GetEnumerator();
            while (enumerator.MoveNext())
                GraphicsUtils.Destroy(enumerator.Current.Value);
            m_Materials.Clear();
        }
    }
}