using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone_Music : MonoBehaviour {
    public AudioClip zone_music;
    private BackgroundMusic reference;
    public float const_vol;
    public bool is_changing = false;

    void Update()
    {
        if (reference == null)
        {
            this.enabled = false;
        }
       else if (is_changing == true)
        {
            if (reference.audioSource.volume > 0.0)
            {
                reference.audioSource.volume -= const_vol / 10;
            }
             else if (reference.audioSource.volume <= 0.0)
           {
            float timestamp = reference.audioSource.time;
            reference.audioSource.Stop();
            reference.audioSource.clip = zone_music;
            reference.audioSource.Play();
            reference.audioSource.time = timestamp;
            is_changing = false;
            }
        }
       
        else if(reference.audioSource.volume < const_vol && is_changing == false)
        {
            
            reference.audioSource.volume += const_vol / 10; ;
        }
       else if (reference.audioSource.volume > const_vol)
        {
            Debug.Log("Setting Volume");
            reference.audioSource.volume = const_vol;
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        reference = GameObject.FindGameObjectWithTag("Music").GetComponent<BackgroundMusic>();
        const_vol = reference.audioSource.volume;
        if (other.gameObject.layer == 9 && reference.audioSource.clip.name != zone_music.name)
        {
            this.enabled = true;
            Debug.Log("changing music to: " + zone_music.name);
            is_changing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (reference != null)
        {
            reference.audioSource.volume = const_vol;
        }
        this.enabled = false;
    }

}
