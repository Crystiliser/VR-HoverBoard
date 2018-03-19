namespace UnityEngine.PostProcessing
{
    [System.Serializable]
    public abstract class PostProcessingModel
    {
        [SerializeField, GetSet("enabled")]
        private bool m_Enabled;
        public bool enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }
        public abstract void Reset();
    }
}