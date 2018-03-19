using UnityEngine;
public class Zone_Music : MonoBehaviour
{
    [SerializeField] private AudioClip zone_music = null;
    private AudioSource reference = null;
    private float const_vol = 0.0f;
    private bool is_changing = false;
    private const int particleLayer = 9;
    private void Update()
    {
        if (null == reference)
            enabled = false;
        else if (is_changing)
        {
            if (reference.volume > 0.0f)
                reference.volume -= const_vol * 0.1f;
            else
            {
                float timestamp = reference.time;
                reference.Stop();
                reference.clip = zone_music;
                reference.Play();
                reference.time = timestamp;
                is_changing = false;
            }
        }
        else if (reference.volume < const_vol)
            reference.volume += const_vol * 0.1f;
        else
        {
            reference.volume = const_vol;
            enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        reference = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        const_vol = reference.volume;
        if (particleLayer == other.gameObject.layer && reference.clip.name != zone_music.name)
        {
            enabled = true;
            is_changing = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (null != reference)
            reference.volume = const_vol;
        enabled = false;
    }
}