namespace UnityEngine.PostProcessing
{
    public class PostProcessingContext
    {
        public PostProcessingProfile profile = null;
        public Camera camera = null;
        public MaterialFactory materialFactory = null;
        public RenderTextureFactory renderTextureFactory = null;
        public bool Interrupted { get; set; } = false;
        public PostProcessingContext Reset()
        {
            profile = null;
            camera = null;
            materialFactory = null;
            renderTextureFactory = null;
            Interrupted = false;
            return this;
        }
        public bool IsGBufferAvailable => RenderingPath.DeferredShading == camera.actualRenderingPath;
        public bool IsHDR => camera.allowHDR;
        public int Width => camera.pixelWidth;
        public int Height => camera.pixelHeight;
        public Rect Viewport => camera.rect;
    }
}