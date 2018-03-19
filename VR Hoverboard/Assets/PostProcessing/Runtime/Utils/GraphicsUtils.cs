namespace UnityEngine.PostProcessing
{
    public static class GraphicsUtils
    {
        static Texture2D s_WhiteTexture;
        public static Texture2D WhiteTexture
        {
            get
            {
                if (null != s_WhiteTexture)
                    return s_WhiteTexture;
                s_WhiteTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                s_WhiteTexture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 1.0f));
                s_WhiteTexture.Apply();
                return s_WhiteTexture;
            }
        }
        public static void Destroy(Object obj)
        {
            if (null != obj)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    Object.Destroy(obj);
                else
                    Object.DestroyImmediate(obj);
#else
                Object.Destroy(obj);
#endif
            }
        }
    }
}