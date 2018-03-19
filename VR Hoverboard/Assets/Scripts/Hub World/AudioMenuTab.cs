using UnityEngine;
using TMPro;
public class AudioMenuTab : MenuTab
{
    [SerializeField, Range(2, 1000), Tooltip("Number representing the maximum volume level")] private int maxNumber = 16;
    [Space, SerializeField, LabelOverride("BGM Volume Text")] private TextMeshPro bgmVolume = null;
    [SerializeField, LabelOverride("SFX Volume Text")] private TextMeshPro sfxVolume = null;
    [SerializeField, LabelOverride("ENV Volume Text")] private TextMeshPro envVolume = null;
    [SerializeField]
    private EventSelectedObject bgmUpButton = null, bgmDownButton = null,
        sfxUpButton = null, sfxDownButton = null, envUpButton = null, envDownButton = null,
        confirmButton = null, defaultButton = null, revertButton = null;
    float originalBgmVol;
    float originalEnvVol;
    float originalSfxVol;

    private float VolumeIncrement => 1.0f / maxNumber;
    private void SetBgmText() => bgmVolume.SetText(Mathf.RoundToInt(AudioManager.BgmVolume * maxNumber).ToString());
    private void SetSfxText() => sfxVolume.SetText(Mathf.RoundToInt(AudioManager.SfxVolume * maxNumber).ToString());
    private void SetEnvText() => envVolume.SetText(Mathf.RoundToInt(AudioManager.EnvVolume * maxNumber).ToString());
    private void OnEnable()
    {
        AudioManager.OnBgmVolumeChanged += SetBgmText;
        AudioManager.OnSfxVolumeChanged += SetSfxText;
        AudioManager.OnEnvVolumeChanged += SetEnvText;
        //  confirmButton.OnSelectSuccess += confirmOptions;
        defaultButton.OnSelectSuccess += defaultOptions;
        revertButton.OnSelectSuccess += revertOptions;
        bgmUpButton.OnSelectSuccess += BgmUp;
        bgmDownButton.OnSelectSuccess += BgmDown;
        sfxUpButton.OnSelectSuccess += SfxUp;
        sfxDownButton.OnSelectSuccess += SfxDown;
        envUpButton.OnSelectSuccess += EnvUp;
        envDownButton.OnSelectSuccess += EnvDown;
        SetBgmText();
        SetSfxText();
        SetEnvText();
        originalBgmVol = AudioManager.BgmVolume;
        originalEnvVol = AudioManager.EnvVolume;
        originalSfxVol = AudioManager.SfxVolume;
    }
    private void OnDisable()
    {
        AudioManager.OnBgmVolumeChanged -= SetBgmText;
        AudioManager.OnSfxVolumeChanged -= SetSfxText;
        AudioManager.OnEnvVolumeChanged -= SetEnvText;
        //  confirmButton.OnSelectSuccess -= confirmOptions;
        defaultButton.OnSelectSuccess -= defaultOptions;
        revertButton.OnSelectSuccess -= revertOptions;
        bgmUpButton.OnSelectSuccess -= BgmUp;
        bgmDownButton.OnSelectSuccess -= BgmDown;
        sfxUpButton.OnSelectSuccess -= SfxUp;
        sfxDownButton.OnSelectSuccess -= SfxDown;
        envUpButton.OnSelectSuccess -= EnvUp;
        envDownButton.OnSelectSuccess -= EnvDown;
    }
    // private void confirmOptions()
    // {
    //     
    // }
    private void defaultOptions()
    {
        AudioManager.SfxVolume = 2.0f;
        AudioManager.BgmVolume = 2.0f;
        AudioManager.EnvVolume = 2.0f;
    }
    private void revertOptions()
    {
        AudioManager.SfxVolume = originalSfxVol;
        AudioManager.BgmVolume = originalBgmVol;
        AudioManager.EnvVolume = originalEnvVol;
    }
    private void BgmUp() => AudioManager.BgmVolume += VolumeIncrement;
    private void BgmDown() => AudioManager.BgmVolume -= VolumeIncrement;
    private void SfxUp() => AudioManager.SfxVolume += VolumeIncrement;
    private void SfxDown() => AudioManager.SfxVolume -= VolumeIncrement;
    private void EnvUp() => AudioManager.EnvVolume += VolumeIncrement;
    private void EnvDown() => AudioManager.EnvVolume -= VolumeIncrement;
}