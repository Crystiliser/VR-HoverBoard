using UnityEngine;

public class AudioLevels : MonoBehaviour
{
    private static AudioLevels instance = null;
    private static bool applicationRunning = true;
    public static AudioLevels Instance { get { if (null == instance && applicationRunning) { instance = new GameObject("AudioManager").AddComponent<AudioLevels>(); } return instance; } }
    public delegate void SettingChangedEvent();
    public SettingChangedEvent OnBgmVolumeChange, OnSfxVolumeChange, OnEnvVolumeChange;
    private float bgmVolume = 1.0f, sfxVolume = 1.0f, envVolume = 1.0f;
    public float BgmVolume { get { return bgmVolume; } set { bgmVolume = Mathf.Clamp01(value); if (null != OnBgmVolumeChange) OnBgmVolumeChange(); } }
    public float SfxVolume { get { return sfxVolume; } set { sfxVolume = Mathf.Clamp01(value); if (null != OnSfxVolumeChange) OnSfxVolumeChange(); } }
    public float EnvVolume { get { return envVolume; } set { envVolume = Mathf.Clamp01(value); if (null != OnEnvVolumeChange) OnEnvVolumeChange(); } }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1.0f);
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);
            envVolume = PlayerPrefs.GetFloat("EnvVolume", 1.0f);
            if (null != OnBgmVolumeChange)
                OnBgmVolumeChange();
            if (null != OnSfxVolumeChange)
                OnSfxVolumeChange();
            if (null != OnEnvVolumeChange)
                OnEnvVolumeChange();
        }
        else if (this != instance)
            Destroy(this);
    }
    private void OnDestroy()
    {
        if (this == instance)
        {
            PlayerPrefs.SetFloat("BgmVolume", bgmVolume);
            PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
            PlayerPrefs.SetFloat("EnvVolume", envVolume);
            PlayerPrefs.Save();
        }
    }
    private void OnApplicationQuit()
    {
        applicationRunning = false;
    }
}