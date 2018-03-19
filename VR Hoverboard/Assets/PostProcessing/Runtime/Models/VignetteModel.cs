namespace UnityEngine.PostProcessing
{
    [System.Serializable]
    public class VignetteModel : PostProcessingModel
    {
        public enum Mode { Classic, Masked }
        [System.Serializable]
        public struct Settings
        {
            [Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
            public Mode mode;
            [ColorUsage(false)]
            [Tooltip("Vignette color. Use the alpha channel for transparency.")]
            public Color color;
            [Tooltip("Sets the vignette center point (screen center is [0.5,0.5]).")]
            public Vector2 center;
            [Range(0.0f, 1.0f), Tooltip("Amount of vignetting on screen.")]
            public float intensity;
            [Range(0.01f, 1.0f), Tooltip("Smoothness of the vignette borders.")]
            public float smoothness;
            [Range(0.0f, 1.0f), Tooltip("Lower values will make a square-ish vignette.")]
            public float roundness;
            [Tooltip("A black and white mask to use as a vignette.")]
            public Texture mask;
            [Range(0.0f, 1.0f), Tooltip("Mask opacity.")]
            public float opacity;
            [Tooltip("Should the vignette be perfectly round or be dependent on the current aspect ratio?")]
            public bool rounded;
            public static Settings DefaultSettings => new Settings
            {
                mode = Mode.Classic,
                color = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                center = new Vector2(0.5f, 0.5f),
                intensity = 0.45f,
                smoothness = 0.2f,
                roundness = 1.0f,
                mask = null,
                opacity = 1.0f,
                rounded = false
            };
        }
        [SerializeField]
        private Settings m_Settings = Settings.DefaultSettings;
        public Settings GetSettings => m_Settings;
        public override void Reset() => m_Settings = Settings.DefaultSettings;
    }
}