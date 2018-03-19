namespace UnityEngine.PostProcessing
{
    public class PostProcessingProfile : ScriptableObject
    {
        public BloomModel bloom = new BloomModel();
        public VignetteModel vignette = new VignetteModel();
    }
}