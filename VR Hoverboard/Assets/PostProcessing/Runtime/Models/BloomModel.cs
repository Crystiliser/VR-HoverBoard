namespace UnityEngine.PostProcessing
{
    [System.Serializable]
    public class BloomModel : PostProcessingModel
    {
        [System.Serializable]
        public struct BloomSettings
        {
            [Min(0.0f), Tooltip("Strength of the bloom filter.")]
            public float intensity;
            [Min(0.0f), Tooltip("Filters out pixels under this level of brightness.")]
            public float threshold;
            public float ThresholdLinear
            {
                set { threshold = Mathf.LinearToGammaSpace(value); }
                get { return Mathf.GammaToLinearSpace(threshold); }
            }
            [Range(0.0f, 1.0f), Tooltip("Makes transition between under/over-threshold gradual (0 = hard threshold, 1 = soft threshold).")]
            public float softKnee;
            [Range(1.0f, 7.0f), Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
            public float radius;
            [Tooltip("Reduces flashing noise with an additional filter.")]
            public bool antiFlicker;
            public static BloomSettings DefaultSettings => new BloomSettings
            {
                intensity = 0.5f,
                threshold = 1.1f,
                softKnee = 0.5f,
                radius = 4.0f,
                antiFlicker = false,
            };
        }
        [System.Serializable]
        public struct LensDirtSettings
        {
            [Tooltip("Dirtiness texture to add smudges or dust to the lens.")]
            public Texture texture;
            [Min(0.0f), Tooltip("Amount of lens dirtiness.")]
            public float intensity;
            public static LensDirtSettings DefaultSettings => new LensDirtSettings
            {
                texture = null,
                intensity = 3.0f
            };
        }
        [System.Serializable]
        public struct Settings
        {
            public BloomSettings bloom;
            public LensDirtSettings lensDirt;
            public static Settings DefaultSettings => new Settings
            {
                bloom = BloomSettings.DefaultSettings,
                lensDirt = LensDirtSettings.DefaultSettings
            };
        }
        [SerializeField]
        private Settings m_Settings = Settings.DefaultSettings;
        public Settings settings
        {
            get { return m_Settings; }
            set { m_Settings = value; }
        }
        public override void Reset() => m_Settings = Settings.DefaultSettings;
    }
}