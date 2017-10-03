using UnityEngine;

public class Portal_Audio : MonoBehaviour
{
    private AudioSource audioSource = null;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        AudioLevels.Instance.OnEnvVolumeChange += UpdateVolume;
        UpdateVolume();
    }

    private void OnDestroy() { try { AudioLevels.Instance.OnEnvVolumeChange -= UpdateVolume; } catch { } }
    private void UpdateVolume() { audioSource.volume = AudioLevels.Instance.EnvVolume; }
}