namespace UnityEngine.PostProcessing
{
    using System.Collections.Generic;
    public sealed class RenderTextureFactory : System.IDisposable
    {
        private HashSet<RenderTexture> m_TemporaryRTs = new HashSet<RenderTexture>();
        public RenderTexture Get(RenderTexture baseRenderTexture) => Get(
            baseRenderTexture.width,
            baseRenderTexture.height,
            baseRenderTexture.depth,
            baseRenderTexture.format,
            baseRenderTexture.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear,
            baseRenderTexture.filterMode,
            baseRenderTexture.wrapMode);
        public RenderTexture Get(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, RenderTextureReadWrite rw = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "FactoryTempTexture")
        {
            RenderTexture rt = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw); // add forgotten param rw
            rt.filterMode = filterMode;
            rt.wrapMode = wrapMode;
            rt.name = name;
            m_TemporaryRTs.Add(rt);
            return rt;
        }
        public void Release(RenderTexture rt)
        {
            if (null != rt)
            {
                if (!m_TemporaryRTs.Contains(rt))
                    throw new System.ArgumentException(string.Format("Attempting to remove a RenderTexture that was not allocated: {0}", rt));
                m_TemporaryRTs.Remove(rt);
                RenderTexture.ReleaseTemporary(rt);
            }
        }
        public void ReleaseAll()
        {
            HashSet<RenderTexture>.Enumerator enumerator = m_TemporaryRTs.GetEnumerator();
            while (enumerator.MoveNext())
                RenderTexture.ReleaseTemporary(enumerator.Current);
            m_TemporaryRTs.Clear();
        }
        public void Dispose() => ReleaseAll();
    }
}