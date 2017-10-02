using UnityEngine;
public class VolumeControl : SelectedObject
{
    enum AudioType
    {
        BackgroundMusic, SoundEffects, Environment
    }
    [SerializeField]
    private float volumeIncrement = 0.0625f;
    [SerializeField]
    private AudioType audioType;
    public override void selectSuccessFunction()
    {
        switch (audioType)
        {
            case AudioType.BackgroundMusic:
                AudioLevels.Instance.BgmVolume += volumeIncrement;
                break;
            case AudioType.SoundEffects:
                AudioLevels.Instance.SfxVolume += volumeIncrement;
                break;
            case AudioType.Environment:
                AudioLevels.Instance.EnvVolume += volumeIncrement;
                break;
            default:
                break;
        }
    }
}