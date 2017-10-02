using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Audio : MonoBehaviour
{
    private AudioSource Source;

    void Start()
    {
        Source = GetComponent<AudioSource>();
        AudioLevels.Instance.OnEnvVolumeChange += UpdateVolume;
        UpdateVolume();
    }

    private void OnDestroy() { try { AudioLevels.Instance.OnEnvVolumeChange -= UpdateVolume; } catch { } }
    private void UpdateVolume() { Source.volume = AudioLevels.Instance.EnvVolume; }
}