using UnityEngine;

public class Zone_Music : MonoBehaviour
{
    public AudioClip zone_music;
    private BackgroundMusic reference;
    private float const_vol;
    private bool is_changing = false;

    void Update()
    {
        if (null == reference)
        {
            enabled = false;
        }
        else if (is_changing)
        {
            if (reference.audioSource.volume > 0.0)
            {
                reference.audioSource.volume -= const_vol * 0.1f;
            }
            else
            {
                float timestamp = reference.audioSource.time;
                reference.audioSource.Stop();
                reference.audioSource.clip = zone_music;
                reference.audioSource.Play();
                reference.audioSource.time = timestamp;
                is_changing = false;
            }
        }
        else if (reference.audioSource.volume < const_vol)
        {
            reference.audioSource.volume += const_vol * 0.1f;
        }
        if (reference.audioSource.volume >= const_vol)
        {
            reference.audioSource.volume = const_vol;
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        reference = GameObject.FindGameObjectWithTag("Music").GetComponent<BackgroundMusic>();
        const_vol = reference.audioSource.volume;
        if (9 == other.gameObject.layer && reference.audioSource.clip.name != zone_music.name)
        {
            enabled = true;
            BuildDebugger.WriteLine("changing music to: " + zone_music.name);
            is_changing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (reference != null)
        {
            reference.audioSource.volume = const_vol;
        }
        enabled = false;
    }
}