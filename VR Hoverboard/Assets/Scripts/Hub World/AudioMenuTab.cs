using UnityEngine;
using TMPro;
public class AudioMenuTab : MenuTab
{
    [SerializeField]
    private TextMeshPro bgmVolume = null, sfxVolume = null, envVolume = null;
    private void SetBgmText()
    {
        if (null != bgmVolume)
            bgmVolume.SetText(Mathf.RoundToInt(AudioLevels.Instance.BgmVolume * 16.0f).ToString());
    }
    private void SetSfxText()
    {
        if (null != sfxVolume)
            sfxVolume.SetText(Mathf.RoundToInt(AudioLevels.Instance.SfxVolume * 16.0f).ToString());
    }
    private void SetEnvText()
    {
        if (null != envVolume)
            envVolume.SetText(Mathf.RoundToInt(AudioLevels.Instance.EnvVolume * 16.0f).ToString());
    }
    private void OnEnable()
    {
        AudioLevels.Instance.OnBgmVolumeChange += SetBgmText;
        AudioLevels.Instance.OnSfxVolumeChange += SetSfxText;
        AudioLevels.Instance.OnEnvVolumeChange += SetEnvText;
        SetBgmText();
        SetSfxText();
        SetEnvText();
    }
    private void OnDisable()
    {
        try
        {
            AudioLevels.Instance.OnBgmVolumeChange -= SetBgmText;
            AudioLevels.Instance.OnSfxVolumeChange -= SetSfxText;
            AudioLevels.Instance.OnEnvVolumeChange -= SetEnvText;
        }
        catch { }
    }
}