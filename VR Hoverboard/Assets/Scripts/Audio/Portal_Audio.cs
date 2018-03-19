using UnityEngine;
public class Portal_Audio : MonoBehaviour
{
    private AudioSource audioSource = null;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        AudioManager.OnEnvVolumeChanged += UpdateVolume;
        UpdateVolume();
    }
    private void OnDestroy() => AudioManager.OnEnvVolumeChanged -= UpdateVolume;
    private void UpdateVolume() => audioSource.volume = AudioManager.EnvVolume;
}